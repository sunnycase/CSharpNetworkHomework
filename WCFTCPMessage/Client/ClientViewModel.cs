using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using Contracts;

namespace Client
{
    class ClientViewModel : INotifyPropertyChanged, IChatClientCallback
    {
        public ObservableCollection<ChatMessage> Messages { get; } = new ObservableCollection<ChatMessage>();
        public ObservableCollection<string> OnlineUsers { get; } = new ObservableCollection<string>();

        private string _userName;
        public string UserName
        {
            get { return _userName; }
            set
            {
                if (SetProperty(ref _userName, value))
                    LoginCommand.RaiseCanExecuteChanged();
            }
        }

        private string _sendText;
        public string SendText
        {
            get { return _sendText; }
            set
            {
                if (SetProperty(ref _sendText, value))
                    SendCommand.RaiseCanExecuteChanged();
            }
        }

        private bool _isLogged = false;

        public RelayCommand LoginCommand { get; }
        public RelayCommand SendCommand { get; }

        private readonly IChatService _chatService;
        private readonly Dispatcher _dispatcher;

        public ClientViewModel()
        {
            LoginCommand = new RelayCommand(OnLoginCommand, () => !string.IsNullOrWhiteSpace(UserName) && !_isLogged);
            SendCommand = new RelayCommand(OnSendCommand, () => !string.IsNullOrWhiteSpace(SendText));
            _chatService = ConnectService();
            _dispatcher = Dispatcher.CurrentDispatcher;
            LoadData();
        }

        public void LogOff()
        {
            _chatService.Logoff(UserName);
        }

        private IChatService ConnectService()
        {
            InstanceContext instanceContext = new InstanceContext(this);
            var channelFactory = new DuplexChannelFactory<IChatService>(instanceContext, "ChatService");
            return channelFactory.CreateChannel();
        }

        private void OnLoginCommand()
        {
            _chatService.Login(UserName);
            _isLogged = true;
            LoginCommand.RaiseCanExecuteChanged();
        }

        private void LoadData()
        {
            OnlineUsers.Clear();
            foreach (var user in _chatService.GetOnlineUsers())
                OnlineUsers.Add(user);
        }

        private void OnSendCommand()
        {
            _chatService.SendMessage(UserName, SendText);
            SendText = string.Empty;
        }

        public void ReceiveMessage(ChatMessage message)
        {
            _dispatcher.InvokeAsync(() => Messages.Add(message));
        }

        public void OnLogin(LoginMessage message)
        {
            _dispatcher.InvokeAsync(() => OnlineUsers.Add(message.Sender));
        }

        public void OnLogoff(LogoffMessage message)
        {
            _dispatcher.InvokeAsync(() => OnlineUsers.Remove(message.Sender));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private bool SetProperty<T>(ref T property, T value, [CallerMemberName]string propertyName = null)
        {
            if (!object.Equals(property, value))
            {
                property = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
                return true;
            }
            return false;
        }
    }
}
