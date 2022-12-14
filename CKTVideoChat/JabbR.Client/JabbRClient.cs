using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client;
using Microsoft.AspNet.SignalR.Client.Http;
using Microsoft.AspNet.SignalR.Client.Transports;
using Newtonsoft.Json.Linq;
using CKTChat.Client.Models;

namespace CKTChat.Client
{
    public class JabbRClient : IJabbRClient
    {
        private readonly IAuthenticationProvider _defaultAuthenticationProvider;
        private readonly Func<IClientTransport> _transportFactory;

        private IHubProxy _chat;
        private HubConnection _connection;
        private IAuthenticationProvider _authenticationProvider;

        public JabbRClient(string url) :
            this(url, transportFactory: () => new AutoTransport(new DefaultHttpClient()))
        {
        }

        public JabbRClient(string url, Func<IClientTransport> transportFactory)
        {
            SourceUrl = url;
            _transportFactory = transportFactory;
            TraceLevel = TraceLevels.All;

            _defaultAuthenticationProvider = new DefaultAuthenticationProvider(url);
        }

        public event Action<Message, string> MessageReceived;
        public event Action<IEnumerable<string>> LoggedOut;
        public event Action<User, string, bool> UserJoined;
        public event Action<User, string> UserLeft;
        public event Action<string> Kicked;
        public event Action<string, string, string> PrivateMessage;
        public event Action<User, string> UserTyping;
        public event Action<User, string> GravatarChanged;
        public event Action<string, string, string> MeMessageReceived;
        public event Action<string, User, string> UsernameChanged;
        public event Action<User, string> NoteChanged;
        public event Action<User, string> FlagChanged;
        public event Action<string, string, string> TopicChanged;
        public event Action<User, string> OwnerAdded;
        public event Action<User, string> OwnerRemoved;
        public event Action<string, string, string> AddMessageContent;
        public event Action<Room> JoinedRoom;

        // Global
        public event Action<Room> RoomChanged;
        public event Action<User> UserActivityChanged;
        public event Action<IEnumerable<User>> UsersInactive;

        public string SourceUrl { get; private set; }
        public bool AutoReconnect { get; set; }
        public TextWriter TraceWriter { get; set; }
        public TraceLevels TraceLevel { get; set; }

        public IAuthenticationProvider AuthenticationProvider
        {
            get
            {
                return _authenticationProvider ?? _defaultAuthenticationProvider;
            }
            set
            {
                _authenticationProvider = value;
            }
        }

        public HubConnection Connection
        {
            get
            {
                return _connection;
            }
        }

        public ICredentials Credentials
        {
            get
            {
                return _connection.Credentials;
            }
            set
            {
                _connection.Credentials = value;
            }
        }

        public event Action Disconnected
        {
            add
            {
                _connection.Closed += value;
            }
            remove
            {
                _connection.Closed -= value;
            }
        }

        public event Action<StateChange> StateChanged
        {
            add
            {
                _connection.StateChanged += value;
            }
            remove
            {
                _connection.StateChanged -= value;
            }
        }

        public async Task<LogOnInfo> Connect(string name, string password)
        {
            _connection = await AuthenticationProvider.Connect(name, password);

            if (TraceWriter != null)
            {
                _connection.TraceWriter = TraceWriter;
            }

            _connection.TraceLevel = TraceLevel;

            _chat = _connection.CreateHubProxy("chat");

            SubscribeToEvents();

            await _connection.Start(_transportFactory());

            return await LogOn();
        }

        private async Task<LogOnInfo> LogOn()
        {
            var tcs = new TaskCompletionSource<LogOnInfo>();

            IDisposable logOn = null;

            // Wait for the logOn callback to get triggered
            logOn = _chat.On<IEnumerable<Room>, JArray>(ClientEvents.LogOn, (rooms, privateRooms) =>
            {
                logOn.Dispose();

                tcs.TrySetResult(new LogOnInfo
                {
                    Rooms = rooms,
                    UserId = (string)_chat["id"]
                });
            });

            // Join JabbR
            await _chat.Invoke("Join");

            return await tcs.Task;
        }

        public Task<User> GetUserInfo()
        {
            return _chat.Invoke<User>("GetUserInfo");
        }

        public Task LogOut()
        {
            return _chat.Invoke("LogOut");
        }

        public Task<bool> Send(string message, string roomName)
        {
            return _chat.Invoke<bool>("Send", message, roomName);
        }

        public Task<bool> Send(ClientMessage message)
        {
            return _chat.Invoke<bool>("Send", message);
        }

        public async Task Send(ClientMessage message, TimeSpan timeout)
        {
            using (var cts = new CancellationTokenSource(timeout))
            {
                await Send(message, cts.Token);
            }
        }

        public async Task Send(ClientMessage message, CancellationToken cancel)
        {
            var tcs = new TaskCompletionSource<object>();
            IDisposable subscription = null;

            subscription = _chat.On<string, Message, string>(ClientEvents.ReplaceMessage, (id, _, __) =>
            {
                subscription.Dispose();

                if (id == message.Id)
                {
                    tcs.TrySetResult(null);
                }
            });

            IDisposable registration = cancel.Register(() =>
            {
                subscription.Dispose();
                tcs.TrySetCanceled();
            });

            await _chat.Invoke<bool>("Send", message);
            await tcs.Task;

            registration.Dispose();
        }

        public Task PostNotification(ClientNotification notification, bool executeContentProviders)
        {
            return _chat.Invoke("PostNotification", notification, executeContentProviders);
        }

        public Task PostNotification(ClientNotification notification)
        {
            return _chat.Invoke("PostNotification", notification);
        }

        public async Task CreateRoom(string roomName)
        {
            var tcs = new TaskCompletionSource<object>();

            IDisposable createRoom = null;

            createRoom = _chat.On<Room>(ClientEvents.RoomCreated, room =>
            {
                createRoom.Dispose();

                tcs.SetResult(null);
            });

            await SendCommand("create {0}", roomName);

            await tcs.Task;
        }

        public async Task JoinRoom(string roomName)
        {
            var tcs = new TaskCompletionSource<object>();

            IDisposable joinRoom = null;

            joinRoom = _chat.On<Room>(ClientEvents.JoinRoom, room =>
            {
                joinRoom.Dispose();

                tcs.SetResult(null);
            });

            await SendCommand("join {0}", roomName);

            await tcs.Task;
        }

        public Task LeaveRoom(string roomName)
        {
            return SendCommand("leave {0}", roomName);
        }

        public Task SetFlag(string countryCode)
        {
            return SendCommand("flag {0}", countryCode);
        }

        public Task SetNote(string noteText)
        {
            return SendCommand("note {0}", noteText);
        }

        public Task SendPrivateMessage(string userName, string message)
        {
            return SendCommand("msg {0} {1}", userName, message);
        }

        public Task Kick(string userName, string roomName)
        {
            return SendCommand("kick {0} {1}", userName, roomName);
        }

        public Task<bool> CheckStatus()
        {
            return _chat.Invoke<bool>("CheckStatus");
        }

        public Task SetTyping(string roomName)
        {
            return _chat.Invoke("Typing", roomName);
        }

        public Task<IEnumerable<Message>> GetPreviousMessages(string fromId)
        {
            return _chat.Invoke<IEnumerable<Message>>("GetPreviousMessages", fromId);
        }

        public Task<Room> GetRoomInfo(string roomName)
        {
            return _chat.Invoke<Room>("GetRoomInfo", roomName);
        }

        public Task<IEnumerable<Room>> GetRooms()
        {
            return _chat.Invoke<IEnumerable<Room>>("GetRooms");
        }

        public void Disconnect()
        {
            _connection.Stop();
        }

        private void SubscribeToEvents()
        {
            if (AutoReconnect)
            {
                Disconnected += OnDisconnected;
            }

            _chat.On<Message, string>(ClientEvents.AddMessage, (message, room) =>
            {
                Execute(MessageReceived, messageReceived => messageReceived(message, room));
            });

            _chat.On<IEnumerable<string>>(ClientEvents.LogOut, rooms =>
            {
                Execute(LoggedOut, loggedOut => loggedOut(rooms));
            });

            _chat.On<User, string, bool>(ClientEvents.AddUser, (user, room, isOwner) =>
            {
                Execute(UserJoined, userJoined => userJoined(user, room, isOwner));
            });

            _chat.On<User, string>(ClientEvents.Leave, (user, room) =>
            {
                Execute(UserLeft, userLeft => userLeft(user, room));
            });

            _chat.On<string>(ClientEvents.Kick, room =>
            {
                Execute(Kicked, kicked => kicked(room));
            });

            _chat.On<Room>(ClientEvents.UpdateRoom, (room) =>
            {
                Execute(RoomChanged, roomChanged => roomChanged(room));
            });

            _chat.On<User, string>(ClientEvents.UpdateActivity, (user, roomName) =>
            {
                Execute(UserActivityChanged, userActivityChanged => userActivityChanged(user));
            });

            _chat.On<string, string, string>(ClientEvents.SendPrivateMessage, (from, to, message) =>
            {
                Execute(PrivateMessage, privateMessage => privateMessage(from, to, message));
            });

            _chat.On<IEnumerable<User>>(ClientEvents.MarkInactive, (users) =>
            {
                Execute(UsersInactive, usersInactive => usersInactive(users));
            });

            _chat.On<User, string>(ClientEvents.SetTyping, (user, room) =>
            {
                Execute(UserTyping, userTyping => userTyping(user, room));
            });

            _chat.On<User, string>(ClientEvents.GravatarChanged, (user, room) =>
            {
                Execute(GravatarChanged, gravatarChanged => gravatarChanged(user, room));
            });

            _chat.On<string, string, string>(ClientEvents.MeMessageReceived, (user, content, room) =>
            {
                Execute(MeMessageReceived, meMessageReceived => meMessageReceived(user, content, room));
            });

            _chat.On<string, User, string>(ClientEvents.UsernameChanged, (oldUserName, user, room) =>
            {
                Execute(UsernameChanged, usernameChanged => usernameChanged(oldUserName, user, room));
            });

            _chat.On<User, string>(ClientEvents.NoteChanged, (user, room) =>
            {
                Execute(NoteChanged, noteChanged => noteChanged(user, room));
            });

            _chat.On<User, string>(ClientEvents.FlagChanged, (user, room) =>
            {
                Execute(FlagChanged, flagChanged => flagChanged(user, room));
            });

            _chat.On<string, string, string>(ClientEvents.TopicChanged, (roomName, topic, who) =>
            {
                Execute(TopicChanged, topicChanged => topicChanged(roomName, topic, who));
            });

            _chat.On<User, string>(ClientEvents.OwnerAdded, (user, room) =>
            {
                Execute(OwnerAdded, ownerAdded => ownerAdded(user, room));
            });

            _chat.On<User, string>(ClientEvents.OwnerRemoved, (user, room) =>
            {
                Execute(OwnerRemoved, ownerRemoved => ownerRemoved(user, room));
            });

            _chat.On<string, string, string>(ClientEvents.AddMessageContent, (messageId, extractedContent, roomName) =>
            {
                Execute(AddMessageContent, addMessageContent => addMessageContent(messageId, extractedContent, roomName));
            });

            _chat.On<Room>(ClientEvents.JoinRoom, (room) =>
            {
                Execute(JoinedRoom, joinedRoom => joinedRoom(room));
            });
        }

        private async void OnDisconnected()
        {
            await TaskAsyncHelper.Delay(TimeSpan.FromSeconds(5));

            try
            {
                await _connection.Start(_transportFactory());

                // Join JabbR
                await _chat.Invoke("Join", false);
            }
            catch (Exception ex)
            {
                _connection.Trace(TraceLevels.Events, ex.Message);
            }
        }

        private void Execute<T>(T handlers, Action<T> action) where T : class
        {
            Task.Factory.StartNew(() =>
            {
                if (handlers != null)
                {
                    try
                    {
                        action(handlers);
                    }
                    catch (Exception ex)
                    {
                        _connection.Trace(TraceLevels.Events, ex.Message);
                    }
                }
            });
        }

        private Task SendCommand(string command, params object[] args)
        {
            return _chat.Invoke("Send", String.Format("/" + command, args), "");
        }
    }
}
