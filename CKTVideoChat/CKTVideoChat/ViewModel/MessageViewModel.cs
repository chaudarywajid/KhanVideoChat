using CKTChat.Client.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CKTVideoChat.ViewModel
{
    public class MessageViewModelCollection : INotifyPropertyChanged
    {
        public MessageViewModelCollection()
        {
            vModel = new ObservableCollection<MessageViewModel>();
        }
        private ObservableCollection<MessageViewModel> vModel;

        public ObservableCollection<MessageViewModel> VModel
        {
            get
            {
                return vModel;
            }
            set
            {
                vModel = value;
                NotifyPropertyChanged("VModel");
            }
        }
        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this,
                              new PropertyChangedEventArgs(propertyName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
    public class MessageViewModel : INotifyPropertyChanged
    {
        private bool htmlEncoded;
        private string id;
        private string content;
        private DateTimeOffset when;
        private User user;
        private string roomName;
        private string name;
        public bool HtmlEncoded
        {
            get
            {
                return htmlEncoded;
            }
            set
            {
                htmlEncoded = value;
                NotifyPropertyChanged("HtmlEncoded");
            }
        }

        public string Id
        {
            get
            {
                return id;
            }
            set
            {
                id = value;
                NotifyPropertyChanged("Id");
            }
        }
        public string Content
        {
            get
            {
                return content;
            }
            set
            {
                content = value;
                NotifyPropertyChanged("Content");
            }
        }
        public DateTimeOffset When
        {
            get
            {
                return when;
            }
            set
            {
                when = value;
                NotifyPropertyChanged("When");
            }
        }
        public User User
        {
            get
            {
                return user;
            }
            set
            {
                user = value;
                NotifyPropertyChanged("User");
            }
        }

        public string RoomName
        {
            get
            {
                return roomName;
            }
            set
            {
                roomName = value;
                NotifyPropertyChanged("RoomName");
            }
        }

        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
                NotifyPropertyChanged("Name");
            }
        }

        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this,
                              new PropertyChangedEventArgs(propertyName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
