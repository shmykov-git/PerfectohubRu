using PerfectohubRu.Controls;
using PerfectohubRu.Extensions;
using PerfectohubRu.Forms.ViewModles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MovieIntro.Controls
{
    /// <summary>
    /// Interaction logic for SlideTestBot.xaml
    /// </summary>
    public partial class SlideTestBot : UserControl, ISlide
    {
        public MainViewModel Model => DataContext as MainViewModel;

        public SlideTestBot()
        {
            InitializeComponent();
        }

        public async Task PlayEnterAnimation()
        {
            this.AnimateFadeIn();
            await Task.Delay(500);
            Row1.AnimatePulse(0, 0.5, 1, 0.5);
            Row2.AnimatePulse(0, 0.5, 1, 1);
            Row3.AnimatePulse(0, 0.5, 1, 1.5);
            Row4.AnimatePulse(0, 0.5, 1, 2);
            DoneButton.AnimatePulse(0, 1, 3, 3);
        }

        public async Task PlayExitAnimation()
        {
            this.AnimateFadeOut();
        }

        private void DoneButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void ScheduleComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            await Model.SaveSchedule(ScheduleComboBox.SelectedItem as ScheduleItem);
        }
    }
}
