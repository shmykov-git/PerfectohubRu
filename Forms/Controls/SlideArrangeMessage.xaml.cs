using PerfectohubRu.Controls;
using PerfectohubRu.Extensions;
using PerfectohubRu.Forms.ViewModles;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace MovieIntro.Controls
{
    public partial class SlideArrangeMessage : UserControl, ISlide
    {
        private MainWindow ParentWindow => Window.GetWindow(this) as MainWindow;
        private MainViewModel Model => DataContext as MainViewModel;

        public SlideArrangeMessage()
        {
            InitializeComponent();
            this.Loaded += SlideArrangeMessage_Loaded;
        }

        private void SlideArrangeMessage_Loaded(object sender, RoutedEventArgs e)
        {
            Model.RefreshCallsMessage();
        }

        public async Task PlayEnterAnimation()
        {
            //this.Opacity = 0;
            this.Visibility = Visibility.Visible;
            this.AnimateFadeIn();

            TitleText.AnimateFadeIn(2, 1);

            var d1 = 1.2;
            InfoMessageText.AnimateFadeIn(1.6, d1);
            InfoMessageText.AnimateScale((0.5, 1), 2, d1);
            KnownsTextBox.AnimateFadeIn(1.6, d1);
            KnownsTextBox.AnimateScale((0.5, 1), d1);
            CommonsTextBox.AnimateFadeIn(1.6, d1);
            CommonsTextBox.AnimateScale((0.5, 1), d1);

            var d2 = 3;
            LeftIndicatorButton.AnimateFadeIn(delay: d1);
            KnownsIndicatorButton.AnimateFadeIn(delay: d1);
            CommonsIndicatorButton.AnimateFadeIn(delay: d1);
            RefreshMessageButton.AnimateFadeIn(delay: d2);
            SaveKnownsButton.AnimateFadeIn(delay: d2);
            SaveCommonsButton.AnimateFadeIn(delay: d2);

            // Появление кнопки
            DoneButton.AnimateFadeIn(0.5, 1);

            // Фокус на поле ввода
            CommonsTextBox.Focus();
        }

        private async void DoneButton_Click(object sender, RoutedEventArgs e)
        {

        }

        public async Task PlayExitAnimation()
        {
            this.AnimateFadeOut();

            LeftIndicatorButton.AnimateFadeOut();
            KnownsIndicatorButton.AnimateFadeOut();
            CommonsIndicatorButton.AnimateFadeOut();
            RefreshMessageButton.AnimateFadeOut();
            SaveKnownsButton.AnimateFadeOut();
            SaveCommonsButton.AnimateFadeOut();
        }

        private async void RefreshMessageButton_Click(object sender, RoutedEventArgs e)
        {
            Model.RefreshCallsMessage();
            //InfoMessageText.AnimateFadeIn();
            InfoMessageText.AnimateShake(delay:1);
        }

        private async void SaveKnownsButton_Click(object sender, RoutedEventArgs e)
        {
            Model.SaveKnowns();
            Model.RefreshCallsMessage();
            InfoMessageText.AnimateFadeIn(delay:1);
            KnownsTextBox.AnimateShake();
        }

        private void SaveCommonsButton_Click(object sender, RoutedEventArgs e)
        {
            Model.SaveCommons();
            Model.RefreshCallsMessage();
            InfoMessageText.AnimateFadeIn(delay: 1);
            CommonsTextBox.AnimateShake();
        }
    }
}