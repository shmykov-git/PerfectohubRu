using PerfectohubRu.Controls;
using PerfectohubRu.Extensions;
using PerfectohubRu.Forms.ViewModles;
using System;
using System.Security.Policy;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MovieIntro.Controls
{
    public partial class SlideIntegration : UserControl, ISlide
    {
        private MainWindow MainWindow => Window.GetWindow(this) as MainWindow;
        private MainViewModel Model => DataContext as MainViewModel;
        
        public SlideIntegration()
        {
            InitializeComponent();
        }

        public async Task PlayEnterAnimation()
        {
            Model.RefreshIntegrationMessage();

            this.AnimateFadeIn(1);
            
            TitleText.AnimateFadeIn(2, 1);
            SendMessageButton.AnimatePulse(0.3, 1, 3, 1);
            DoneButton.AnimatePulse(0.3, 1, 3, 3);
        }

        public async Task PlayExitAnimation()
        {
            this.AnimateFadeOut(0.3);
        }
        private void SendMessageButton_Click(object sender, RoutedEventArgs e)
        {
            Model.RefreshIntegrationMessage();
        }

        private async void DoneButton_Click(object sender, RoutedEventArgs e)
        {
            await Model.ApproveIntegration();
            MainWindow.OpenSettingsWindow();
        }
    }
}