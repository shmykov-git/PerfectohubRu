using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MovieIntro
{
    // Класс сообщения чата
    public class ChatMessage : INotifyPropertyChanged
    {
        private string _sender;
        private string _message;
        private string _time;
        private bool _isUserMessage;

        public string Sender
        {
            get => _sender;
            set { _sender = value; OnPropertyChanged(); }
        }

        public string Message
        {
            get => _message;
            set { _message = value; OnPropertyChanged(); }
        }

        public string Time
        {
            get => _time;
            set { _time = value; OnPropertyChanged(); }
        }

        public bool IsUserMessage
        {
            get => _isUserMessage;
            set { _isUserMessage = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}