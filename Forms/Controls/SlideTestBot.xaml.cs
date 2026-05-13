using PerfectohubRu.Controls;
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
    }
}
