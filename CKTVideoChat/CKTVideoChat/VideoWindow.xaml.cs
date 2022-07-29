using OpenTok;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using Telerik.Windows.Controls;
using static OpenTok.Session;

namespace CKTVideoChat
{
    /// <summary>
    /// Interaction logic for TelerikScenario1.xaml
    /// </summary>
    public partial class VideoWindow
    {
        public const string API_KEY = "45966052";
        public const string SESSION_ID = "2_MX40NTk2NjA1Mn5-MTUwNjM0NzEyMDc1MX54YTFvS3g1WEdlQzhyR1NkeWR1T3NzYml-fg";
        public const string TOKEN = "T1==cGFydG5lcl9pZD00NTk2NjA1MiZzaWc9MWFlNWYxNWZiZWU0OTcyODMyYmI4NzM1ZTEwMTk2NjM4OGM3YTliZjpzZXNzaW9uX2lkPTJfTVg0ME5UazJOakExTW41LU1UVXdOak0wTnpFeU1EYzFNWDU0WVRGdlMzZzFXRWRsUXpoeVIxTmtlV1IxVDNOelltbC1mZyZjcmVhdGVfdGltZT0xNTA2MzQ3MTM3Jm5vbmNlPTAuNzE4MjQwOTU2NDk1MjEzNSZyb2xlPXB1Ymxpc2hlciZleHBpcmVfdGltZT0xNTA4OTM5MTMzJmluaXRpYWxfbGF5b3V0X2NsYXNzX2xpc3Q9";

        VideoCapturer Capturer;
        Session Session;
        Publisher Publisher;
        Stream PublisherStream;
        bool Disconnect = false;
        Dictionary<Stream, Subscriber> SubscriberByStream = new Dictionary<Stream, Subscriber>();
        public VideoWindow()
        {           
       
            InitializeComponent();

            this.Icon = new Image()
            {
                Width = 32,
                Source = new BitmapImage(new Uri("../../Images/logo48.png", UriKind.Relative))
            };

            // This shows how to enumarate the available capturer devices on the system to allow the user of the app
            // to select the desired camera. If a capturer is not provided in the publisher constructor the first available 
            // camera will be used.
            var devices = VideoCapturer.EnumerateDevices();
            if (devices.Count > 0)
            {
                var selectedDevice = devices[0];
                Console.WriteLine("Using camera: " + devices[0].Name);
                Capturer = selectedDevice.CreateVideoCapturer(VideoCapturer.Resolution.High);
            }
            else
            {
                Console.WriteLine("Warning: no cameras available, the publisher will be audio only.");
            }

            // We create the publisher here to show the preview when application starts
            // Please note that the PublisherVideo component is added in the xaml file
            Publisher = new Publisher(Context.Instance, renderer: PublisherVideo, capturer: Capturer);
            Publisher.Error += Publisher_Error;
            Publisher.StreamCreated += Publisher_StreamCreated;
            Publisher.StreamDestroyed += Publisher_StreamDestroyed;


            if (API_KEY == "" || SESSION_ID == "" || TOKEN == "")
            {
                MessageBox.Show("Please fill out the API_KEY, SESSION_ID and TOKEN variables in the source code " +
                    "in order to connect to the session", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                ConnectDisconnectButton.IsEnabled = false;
            }
            else
            {
                Session = new Session(Context.Instance, API_KEY, SESSION_ID);

                Session.Connected += Session_Connected;
                Session.Disconnected += Session_Disconnected;
                Session.Error += Session_Error;
                Session.StreamReceived += Session_StreamReceived;
                Session.StreamDropped += Session_StreamDropped;

                Session.Signal += Session_Signal;
            }

            Closed += VideoWindows_Closed;
        }

        private void VideoWindows_Closed(object sender, WindowClosedEventArgs e)
        {
            foreach (var subscriber in SubscriberByStream.Values)
            {
                subscriber.Dispose();
            }
            Publisher?.Dispose();
            Capturer?.Dispose();
            Session?.Dispose();
        }

        private void Publisher_StreamCreated(object sender, Publisher.StreamEventArgs e)
        {
            if (e.Stream != null)
            {
                PublisherStream = e.Stream;
            }
            Console.WriteLine("The publisher started streaming.");

            //send chat signal
            //Session.SendSignal("chat", "Hello.............................");
        }

        private void Publisher_Error(object sender, Publisher.ErrorEventArgs e)
        {
            Console.WriteLine("Publisher error:" + e.ErrorCode);
        }

        private void Publisher_StreamDestroyed(object sender, Publisher.StreamEventArgs e)
        {
            Console.WriteLine("The publisher stopped streaming.");
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            foreach (var subscriber in SubscriberByStream.Values)
            {
                subscriber.Dispose();
            }
            Publisher?.Dispose();
            Capturer?.Dispose();
            Session?.Dispose();
        }

        private void Session_Connected(object sender, EventArgs e)
        {
            try
            {
                Session.Publish(Publisher);
            }
            catch (OpenTokException ex)
            {
                Console.WriteLine("OpenTokException " + ex.ToString());
            }
        }

        private void Session_Disconnected(object sender, EventArgs e)
        {
            Console.WriteLine("Session disconnected");
            SubscriberByStream.Clear();
            SubscriberGrid.Children.Clear();
        }

        private void Session_Error(object sender, Session.ErrorEventArgs e)
        {
            MessageBox.Show("Session error:" + e.ErrorCode, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void Session_Signal(object sender, SignalEventArgs e)
        {
            Console.WriteLine("Session received signal. ");
            Console.WriteLine("Data:" + e.Data);
            Console.WriteLine("Type:" + e.Type);
            Console.WriteLine("From connection with ID:" + e.Connection.Id);
        }

        private void UpdateGridSize(int numberOfSubscribers)
        {
            int rows = Convert.ToInt32(Math.Round(Math.Sqrt(numberOfSubscribers)));
            int cols = rows == 0 ? 0 : Convert.ToInt32(Math.Ceiling(((double)numberOfSubscribers) / rows));
            SubscriberGrid.Columns = cols;
            SubscriberGrid.Rows = rows;
        }

        private void Session_StreamReceived(object sender, Session.StreamEventArgs e)
        {
            Console.WriteLine("Session stream received");

            VideoRenderer renderer = new VideoRenderer();
            SubscriberGrid.Children.Add(renderer);
            UpdateGridSize(SubscriberGrid.Children.Count);
            Subscriber subscriber = new Subscriber(Context.Instance, e.Stream, renderer);
            SubscriberByStream.Add(e.Stream, subscriber);

            try
            {
                Session.Subscribe(subscriber);

            }
            catch (OpenTokException ex)
            {
                Console.WriteLine("OpenTokException " + ex.ToString());
            }


        }

        private void Session_StreamDropped(object sender, Session.StreamEventArgs e)
        {
            Console.WriteLine("Session stream dropped");
            var subscriber = SubscriberByStream[e.Stream];
            if (subscriber != null)
            {
                SubscriberByStream.Remove(e.Stream);
                try
                {
                    Session.Unsubscribe(subscriber);
                }
                catch (OpenTokException ex)
                {
                    Console.WriteLine("OpenTokException " + ex.ToString());
                }

                SubscriberGrid.Children.Remove((UIElement)subscriber.VideoRenderer);
                UpdateGridSize(SubscriberGrid.Children.Count);
            }
        }

        private void Connect_Click(object sender, RoutedEventArgs e)
        {
            if (Disconnect)
            {
                Console.WriteLine("Disconnecting session");
                try
                {
                    Session.Unpublish(Publisher);
                    Session.Disconnect();
                }
                catch (OpenTokException ex)
                {
                    Console.WriteLine("OpenTokException " + ex.ToString());
                }
            }
            else
            {
                Console.WriteLine("Connecting session");
                try
                {
                    Session.Connect(TOKEN);
                }
                catch (OpenTokException ex)
                {
                    Console.WriteLine("OpenTokException " + ex.ToString());
                }
            }
            Disconnect = !Disconnect;
            ConnectDisconnectButton.Content = Disconnect ? "Disconnect" : "Connect";

            ConnectButton.IsEnabled = Disconnect ? false : true;
            if (Disconnect)
            {
                ConnectButton.Foreground = new SolidColorBrush(Color.FromRgb(255, 69, 0));
                ConnectButton.Content = "Chat On";
                DisconnectButton.IsEnabled = true;
            }
            else
            {
                ConnectButton.Foreground = new SolidColorBrush(Color.FromRgb(105, 105, 105));
                ConnectButton.Content = "Connect";
                DisconnectButton.IsEnabled = false;
            }
        }


        private void Mute_Click(object sender, RoutedEventArgs e)
        {
            if (PublisherStream != null)
            {
                try
                {
                    if (PublisherStream.HasAudio)
                    {
                        Publisher.PublishAudio = false;
                        Console.WriteLine("Audio is muted.");
                        muteButton.Content = "Unmute";
                    }
                    else
                    {
                        Publisher.PublishAudio = true;
                        Console.WriteLine("Audio is Unmuted.");
                        muteButton.Content = "Mute";

                    }

                }
                catch (OpenTokException ex)
                {
                    Console.WriteLine("Muting exception " + ex.ToString());
                }
            }

        }

        private void Video_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Disonnect_Click(object sender, RoutedEventArgs e)
        {
            if (Disconnect)
            {
                Console.WriteLine("Disconnecting session");
                try
                {
                    Session.Unpublish(Publisher);
                    Session.Disconnect();
                }
                catch (OpenTokException ex)
                {
                    Console.WriteLine("OpenTokException " + ex.ToString());
                }
            }

            Disconnect = !Disconnect;
            ConnectDisconnectButton.Content = Disconnect ? "Disconnect" : "Connect";

            ConnectButton.IsEnabled = Disconnect ? false : true;

            if (Disconnect)
            {
                ConnectButton.Foreground = new SolidColorBrush(Color.FromRgb(107, 142, 35));
                ConnectButton.Content = "Chat On";
                DisconnectButton.IsEnabled = true;
            }
            else
            {
                ConnectButton.Foreground = new SolidColorBrush(Color.FromRgb(105, 105, 105));
                ConnectButton.Content = "Connect";
                DisconnectButton.IsEnabled = false;
            }

        }
    }
}
