using PerfectohubRu.Extensions;
using PerfectohubRu.Model;
using Shared.Model.Enums;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
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
        private Brush _serverResultMessageColor = new SolidColorBrush(Colors.Red);
        private string _knowns = "";
        private string _commons = "";
        private ScheduleItem[] _scheduleItems = new ScheduleItem[0];
        private ScheduleItem _selectedSchedule = null;
        private string _serverResultMessage;

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

        public bool HasAtsRefreshToken => data.GetAtsType() == AtsType.Tele2;

        public string AtsRefreshToken
        {
            get => data.Tele2AtsRefreshToken;
            set
            {
                if (data.Tele2AtsRefreshToken != value)
                {
                    data.Tele2AtsRefreshToken = value;
                    dataProvider.Save();
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

        private void NotifySeverGroup()
        {
            OnPropertyChanged(nameof(StopServerVisibility));
            OnPropertyChanged(nameof(RunServerVisibility));
            OnPropertyChanged(nameof(RunServerForground));
            OnPropertyChanged(nameof(RunServerIsEnabled));
            OnPropertyChanged(nameof(IsServerRun));            
        }

        public ClientState State
        {
            get => data.State;
            set
            {
                if (data.State != value)
                {
                    data.State = value;
                    dataProvider.Save();
                    OnPropertyChanged();
                    NotifySeverGroup();
                }
            }
        }

        public bool IsServerRun
        {
            get => data.IsServerRun;
            set
            {
                if (data.IsServerRun != value)
                {
                    data.IsServerRun = value;
                    dataProvider.Save();
                    OnPropertyChanged();
                    NotifySeverGroup();
                }
            }
        }

        public bool CanStopServer
        {
            get => data.CanStopServer;
            set
            {
                if (data.CanStopServer != value)
                {
                    data.CanStopServer = value;
                    dataProvider.Save();
                    OnPropertyChanged();
                    NotifySeverGroup();
                }
            }
        }

        public Visibility StopServerVisibility => CanStopServer ? Visibility.Visible : Visibility.Collapsed;
        public Visibility RunServerVisibility => CanStopServer ? Visibility.Collapsed : Visibility.Visible;
        public bool RunServerIsEnabled => !IsServerRun && State >= ClientState.HasBot && IsValidClientId();

        public Brush RunServerForground => RunServerIsEnabled
            ? new SolidColorBrush(Color.FromRgb(0xCC, 0xCC, 0xCC))
            : new SolidColorBrush(Color.FromRgb(0x66, 0x66, 0x66));

        public string ClientId
        {
            get => data.ClientId;
            set
            {
                if (data.ClientId != value)
                {
                    data.ClientId = value;
                    dataProvider.Save();
                    OnPropertyChanged();
                    NotifySeverGroup();
                    ValidateClientId();
                }
            }
        }

        public string ClientPassword
        {
            get => data.ClientPassword;
            set
            {
                if (data.ClientPassword != value)
                {
                    data.ClientPassword = value;
                    dataProvider.Save();
                    OnPropertyChanged();
                    ValidatePassword();
                }
            }
        }

        public string ServerResultMessage
        {
            get => _serverResultMessage;
            set
            {
                _serverResultMessage = value;
                OnPropertyChanged();
            }
        }

        public Brush ServerResultMessageColor
        {
            get => _serverResultMessageColor;
            set
            {
                if (_serverResultMessageColor != value)
                {
                    _serverResultMessageColor = value;
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
