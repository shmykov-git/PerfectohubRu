using PerfectohubRu.Extensions;
using PerfectohubRu.Forms.ViewModles;
using PerfectohubRu.Model;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using static System.Net.Mime.MediaTypeNames;

namespace MovieIntro
{
    public partial class SettingsWindow : Window
    {
        private readonly IServiceProvider sp;
        private readonly MainViewModel model;
        private Color SwichedOn = Colors.DarkGreen;
        private Color SwichedOff = Color.FromRgb(0x99, 0x99, 0x99);

        public SettingsWindow()
        {
            InitializeComponent();
            this.Activated += SettingsWindow_Activated;
        }

        private void ShowIndicators()
        {
            if (!IsVisible) return;

            void PlayIndicator(Ellipse indicator, bool isActive)
            {
                var color = (indicator.Fill as SolidColorBrush).Color;

                if (isActive && color != SwichedOn)
                    indicator.Fill.AnimateColorFlash((SwichedOff, SwichedOn), SolidColorBrush.ColorProperty, 1, 0);

                if (!isActive && color != SwichedOff)
                    indicator.Fill.AnimateColorFlash((SwichedOn, SwichedOff), SolidColorBrush.ColorProperty, 1, 0);
            }

            var s = model.Data.State;

            PlayIndicator(AtsStatusIndicator, s >= ClientState.HasAts);
            PlayIndicator(MessagesStatusIndicator, s >= ClientState.HasMessage);
            PlayIndicator(BotStatusIndicator, s >= ClientState.HasTestedBot);
            PlayIndicator(IntegrationStatusIndicator, s >= ClientState.HasIntegration);
        }

        private void SettingsWindow_Activated(object sender, EventArgs e)
        {
            ShowIndicators();
        }

        public SettingsWindow(IServiceProvider sp, MainViewModel model) : this()
        {
            this.sp = sp;
            this.model = model;
            this.DataContext = model;
            model.PropertyChanged += Model_PropertyChanged;
        }

        private void Model_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "State")
            {
                ShowIndicators();
                RunServerButton.IsEnabled = model.State >= ClientState.HasBot;
            }
        }

        public void PositionToRightOfOwner()
        {
            if (Owner != null)
            {
                Left = Owner.Left + Owner.Width + 5;
                Top = Owner.Top;
            }
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                e.Handled = true;
                this.DragMove();
            }
        }

        private void RunServerButton_Click(object sender, RoutedEventArgs e)
        {
            model.IsServerRun = true;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Hide();
            AtsStatusIndicator.Fill.AnimateColorFlash((Colors.DarkGreen, Color.FromRgb(0x99, 0x99, 0x99)), SolidColorBrush.ColorProperty, 0.3, 0);
            MessagesStatusIndicator.Fill.AnimateColorFlash((Colors.DarkGreen, Color.FromRgb(0x99, 0x99, 0x99)), SolidColorBrush.ColorProperty, 0.3, 0);
            BotStatusIndicator.Fill.AnimateColorFlash((Colors.DarkGreen, Color.FromRgb(0x99, 0x99, 0x99)), SolidColorBrush.ColorProperty, 0.3, 0);
            IntegrationStatusIndicator.Fill.AnimateColorFlash((Colors.DarkGreen, Color.FromRgb(0x99, 0x99, 0x99)), SolidColorBrush.ColorProperty, 0.3, 0);
        }

        private void ScheduleComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Clipboard.SetText(Email.Text);

            CopyEmailMessage.AnimateFadeIn(0.5);
            CopyEmailMessage.AnimateFadeOut(0.5, 2);
        }
    }
}