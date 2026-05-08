using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PerfectohubRu.Forms.ViewModles
{
    public partial class MainViewModel : INotifyPropertyChanged
    {
        private string _atsToken = "";
        private string _callsMessage = "";
        private string _knowns = "";
        private string _commons = "";

        public string AtsToken
        {
            get => _atsToken;
            set
            {
                if (_atsToken != value)
                {
                    _atsToken = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Knowns
        {
            get => _knowns;
            set
            {
                if (_knowns != value)
                {
                    _knowns = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Commons
        {
            get => _commons;
            set
            {
                if (_commons != value)
                {
                    _commons = value;
                    OnPropertyChanged();
                }
            }
        }

        public string CallsMessage
        {
            get => _callsMessage;
            set
            {
                if (_callsMessage != value)
                {
                    _callsMessage = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
