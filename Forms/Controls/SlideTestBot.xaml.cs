using PerfectohubRu.Controls;
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
        }

        public async Task PlayExitAnimation()
        {
            
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
