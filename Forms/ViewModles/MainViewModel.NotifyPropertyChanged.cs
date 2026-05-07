using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PerfectohubRu.Forms.ViewModles
{
    public partial class MainViewModel : INotifyPropertyChanged
    {
        private string _atsToken;
        private bool _isWatermarkVisible = true;

        public string AtsToken
        {
            get => _atsToken;
            set
            {
                if (_atsToken != value)
                {
                    _atsToken = value;
                    OnPropertyChanged();
                    IsWatermarkVisible = string.IsNullOrEmpty(value);
                }
            }
        }

        public bool IsWatermarkVisible
        {
            get => _isWatermarkVisible;
            set
            {
                if (_isWatermarkVisible != value)
                {
                    _isWatermarkVisible = value;
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
