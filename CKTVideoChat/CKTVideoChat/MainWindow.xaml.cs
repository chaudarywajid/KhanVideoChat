using CKTChat.Client;
using CKTChat.Client.Models;
using CKTVideoChat.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using Telerik.Windows.Controls;

namespace CKTVideoChat
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        string server = "https://cktchat.azurewebsites.net";
        string roomName = "Room1";
        string userName = "admin";
        string password = "admin4321";
        JabbRClient client;

        MessageViewModelCollection messageColl;
        ObservableCollection<User> usersCollOnline;
        ObservableCollection<Room> roomsColl;

        private VideoWindow window { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            this.Icon = new Image()
                {
                    Width = 32,
                    Source = new BitmapImage(new Uri("../../Images/logo48.png", UriKind.Relative))
                };
            //connect with listbox
            messageColl = new MessageViewModelCollection();
            xChatRadListBox.ItemsSource = messageColl.VModel;
            messageColl.VModel.Add(new MessageViewModel { Content = "Welcome to Shaheer Chat" });

            usersCollOnline = new ObservableCollection<User>();
            xUserRadListBox.ItemsSource = usersCollOnline;

            roomsColl = new ObservableCollection<Room>();
            xRoomRadListBox.ItemsSource = roomsColl;

            InitStartClient();
        }

        public void InitStartClient()
        {
            client = new JabbRClient(server);

            // Uncomment to see tracing
            //client.TraceWriter = Console.Out;

            // Subscribe to new messages
            client.MessageReceived += (message, room) =>
            {
                Debug.WriteLine("[{0}] {1}: {2}", message.When, message.User.Name, message.Content);

                Dispatcher.InvokeAsync(() => messageColl.VModel.Add(new MessageViewModel
                {
                    Content = message.Content,
                    Name = message.User.Name + ": ",
                    HtmlEncoded = message.HtmlEncoded,
                    Id = message.Id,
                    RoomName = room,
                    User = message.User,
                    When = message.When
                }));
            };

            client.UserJoined += (user, room, isOwner) =>
            {
                Debug.WriteLine("{0} joined {1}", user.Name, room);
                Dispatcher.InvokeAsync(async () =>
                {
                    await client.Send(string.Format("{0} joined @ {1}", user.Name, room), room);
                    if (user != null)
                    {
                        User tempuser = usersCollOnline.Where(u => u.Name == user.Name).FirstOrDefault();

                        if (tempuser == null)
                        {
                            usersCollOnline.Add(user);
                        }
                    }
                });

            };

            client.UserLeft += (user, room) =>
            {
                Debug.WriteLine("{0} left {1}", user.Name, room);
            };

            client.PrivateMessage += (from, to, message) =>
            {
                Debug.WriteLine("*PRIVATE* {0} -> {1} ", from, message);
            };

            client.JoinedRoom += (room) =>
            {
                Debug.WriteLine("Room jonied {0}", room.Name);
            };

            RunClientAsync(server, roomName, userName, password, client);
        }

        public async Task DisconnectClientAsync()
        {
            await client.LeaveRoom(roomName);
            client.Disconnect();
        }

        async void RunClientAsync(string server, string roomName, string userName, string password, IJabbRClient client)
        {
            LogOnInfo info = null;

            try
            {
                await EnsureAccount(server, userName, password);

                // Connect to chat
                info = await client.Connect(userName, password);

                Debug.WriteLine("Logged on successfully. You are currently in the following rooms:");
                foreach (var room in info.Rooms)
                {
                    Debug.WriteLine(room.Name);
                    Debug.WriteLine(room.Private);

                }

                if (info.Rooms != null)
                    Debug.WriteLine("Total Rooms:" + info.Rooms.Count());

                Debug.WriteLine("User id is {0}. Don't share this!", info.UserId);

                // Get my user info
                User myInfo = await client.GetUserInfo();
                Debug.WriteLine(myInfo.Name);
                Debug.WriteLine(myInfo.LastActivity);
                Debug.WriteLine(myInfo.Status);
                Debug.WriteLine(myInfo.Country);



                // Join a room called room1 public always
                await client.JoinRoom(roomName);
                txtUserTitle.Text = string.Format("User: {0} joined @ {1} from {2}", myInfo.Name, roomName, myInfo.Country);

                // Send a client side message
                //var message = new ClientMessage
                //{
                //    Id = Guid.NewGuid().ToString(),
                //    Content = "Hey",
                //    Room = roomName
                //};

                // Send the message to the server and wait for the ack
                // await client.Send(message, TimeSpan.FromSeconds(2));

                // Get info about the test room
                Room roomInfo = await client.GetRoomInfo(roomName);

                var rooms = await client.GetRooms();

                if (rooms != null && rooms.Count() > 0)
                {
                    foreach (var room in rooms)
                    {
                        roomsColl.Add(room);
                        Room roomDetail = await client.GetRoomInfo(room.Name);
                        if (roomDetail != null && roomDetail.Users != null && roomDetail.Users.Count() > 0)
                        {
                            foreach (var user in roomDetail.Users)
                            {
                                if (user != null)
                                {
                                    User tempuser = usersCollOnline.Where(u => u.Name == user.Name).FirstOrDefault();

                                    if (tempuser == null)
                                    {
                                        usersCollOnline.Add(new User
                                        {
                                            Name = user.Name,
                                            Active = user.Active,
                                            AfkNote = user.AfkNote,
                                            Country = user.Country,
                                            Flag = user.Flag,
                                            Hash = user.Hash,
                                            IsAdmin = user.IsAdmin,
                                            IsAfk = user.IsAfk,
                                            LastActivity = user.LastActivity,
                                            Note = user.Note,
                                            Status = user.Status,

                                        });
                                    }
                                }
                            }
                        }
                    }

                }

                Debug.WriteLine("Users");



                //foreach (var u in roomInfo.Users)
                //{
                //    if (u.Name != userName)
                //    {
                //        await client.SendPrivateMessage(u.Name, "hey there, this is private right?");
                //    }
                //}

                // Set the flag
                //  await client.SetFlag("pk");

                // Set the user note
                // await client.SetNote("This is testing a note");

                // Mark the client as typing
                // await client.SetTyping(roomName);

                // Clear the note
                //await client.SetNote(null);

                // Say hello to the room
                // await client.Send("Hello world", roomName);

                if (roomInfo.Owners.Contains(userName))
                {
                    // Post a notification (You must be room owner)
                    //await client.PostNotification(new ClientNotification
                    //{
                    //    Source = "Github",
                    //    Content = "This is a fake github notification from the client",
                    //    ImageUrl = "data:image/png;base64,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",
                    //    Room = "test"
                    //});
                }



            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error: " + ex.GetBaseException().Message);
            }
            finally
            {

            }
        }

        private static async Task EnsureAccount(string server, string userName, string password)
        {
            var client = new HttpClient
            {
                BaseAddress = new Uri(server)
            };
            client.DefaultRequestHeaders.Add("sec-jabbr-client", "1");

            var content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "username", userName },
                { "email", "foo@bar.com" },
                { "password", password },
                { "confirmPassword", password }
            });

            HttpResponseMessage response = await client.PostAsync("/account/create", content);

            response.EnsureSuccessStatusCode();
        }

        private void Send_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtChatMessage.Text))
            {
                client.Send(txtChatMessage.Text, roomName);
            }
        }

        private void txtChatMessage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                //enter key is down
                if (!string.IsNullOrEmpty(txtChatMessage.Text))
                {
                    client.Send(txtChatMessage.Text, roomName);
                }
            }

        }

        private void imgVideo_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.window = new VideoWindow();           
            this.window.Owner = this;
            this.window.IsTopmost = true;
            this.window.ShowDialog();
        }

        private void btnVideo_Click(object sender, RoutedEventArgs e)
        {
            this.window = new VideoWindow();
            this.window.Width = 900;
            this.window.Height = 700;
            this.window.Owner = this;
            this.window.IsTopmost = true;
            this.window.ShowDialog();

        }

        ///open video window 
        //private VideoWindow window { get; set; }
        // this.window = new VideoWindow();
        //window.Icon = new Image()
        //{
        //    Source = new BitmapImage(new Uri("/Images/logo.png", UriKind.Relative))
        //    };
        //    this.window.Owner = this;
    }
}
