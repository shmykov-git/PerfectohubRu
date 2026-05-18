using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;

namespace PerfectohubRu.Forms.ViewModles
{

    public partial class MainViewModel : INotifyPropertyChanged
    {
        private string _atsToken = "";
        private string _botToken = "";
        private string _callsMessage = "";
        private string _integrationMessage = "";
        private string _integrationMessageResult = "";
        private Brush _integrationMessageResultColor = new SolidColorBrush(Color.FromRgb(0xCC, 0xCC, 0xCC));
        private string _knowns = "";
        private string _commons = "";
        private ScheduleItem[] _scheduleItems = new ScheduleItem[0];
        private ScheduleItem _selectedSchedule = null;

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

        public string BotToken
        {
            get => _botToken;
            set
            {
                if (_botToken != value)
                {
                    _botToken = value;
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

        public string IntegrationMessage
        {
            get => _integrationMessage;
            set
            {
                if (_integrationMessage != value)
                {
                    _integrationMessage = value;
                    OnPropertyChanged();
                }
            }
        }

        public string IntegrationMessageResult
        {
            get => _integrationMessageResult;
            set
            {
                if (_integrationMessageResult != value)
                {
                    _integrationMessageResult = value;
                    OnPropertyChanged();
                }
            }
        }

        public Brush IntegrationMessageResultColor
        {
            get => _integrationMessageResultColor;
            set
            {
                if (_integrationMessageResultColor != value)
                {
                    _integrationMessageResultColor = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Url
        {
            get => data.IntegrationData.Url;
            set
            {
                if (data.IntegrationData.Url != value)
                {
                    data.IntegrationData.Url = value;
                    dataProvider.Save();
                    OnPropertyChanged();
                }
            }
        }

        public string Username
        {
            get => data.IntegrationData.Username;
            set
            {
                if (data.IntegrationData.Username != value)
                {
                    data.IntegrationData.Username = value;
                    dataProvider.Save();
                    OnPropertyChanged();
                }
            }
        }

        public string Password
        {
            get => data.IntegrationData.Password;
            set
            {
                if (data.IntegrationData.Password != value)
                {
                    data.IntegrationData.Password = value;
                    dataProvider.Save();
                    OnPropertyChanged();
                }
            }
        }

        public bool IsHtml
        {
            get => data.IntegrationData.IsHtml;
            set
            {
                if (data.IntegrationData.IsHtml != value)
                {
                    data.IntegrationData.IsHtml = value;
                    dataProvider.Save();
                    OnPropertyChanged();
                }
            }
        }

        public bool IsText
        {
            get => !data.IntegrationData.IsHtml;
            set
            {
                if (!data.IntegrationData.IsHtml != value)
                {
                    data.IntegrationData.IsHtml = !value;
                    dataProvider.Save();
                    OnPropertyChanged();
                }
            }
        }

        public ScheduleItem[] ScheduleItems
        {
            get => _scheduleItems;
            set
            {
                _scheduleItems = value;
                OnPropertyChanged();
            }
        }

        public ScheduleItem SelectedSchedule
        {
            get => _selectedSchedule;
            set
            {
                _selectedSchedule = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
