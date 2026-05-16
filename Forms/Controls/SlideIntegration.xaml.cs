using PerfectohubRu.Controls;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MovieIntro.Controls
{
    public partial class SlideIntegration : UserControl, ISlide
    {
        public SlideIntegration()
        {
            InitializeComponent();
        }

        private void SendMessageButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DoneButton_Click(object sender, RoutedEventArgs e)
        {
            
        }

        public async Task PlayEnterAnimation()
        {
            
        }

        public async Task PlayExitAnimation()
        {
            
        }
    }
}