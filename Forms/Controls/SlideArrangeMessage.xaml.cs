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
        private MainWindow MainWindow => Window.GetWindow(this) as MainWindow;
        private MainViewModel Model => DataContext as MainViewModel;

        public SlideArrangeMessage()
        {
            InitializeComponent();
        }

        public async Task PlayEnterAnimation()
        {
            Model.RefreshCallsMessage();

            this.Visibility = Visibility.Visible;
            this.AnimateFadeIn(0.3);

            TitleText.AnimateFadeIn(2, 1);

            var dText = 2;
            var dAction = 2;
            var dHelp = 4;
            var dDone = 4;

            InfoMessageText.AnimateScale((0.5, 1), 1, dText);
            CommonsTextBox.AnimateScale((0.5, 1), 1, dText);
            KnownsTextBox.AnimateScale((0.5, 1), 1, dText);

            InfoMessageText.AnimateFadeIn(1, dText);
            KnownsTextBox.AnimateFadeIn(1, dText);
            CommonsTextBox.AnimateFadeIn(1, dText);

            LeftIndicatorButton.AnimateFadeIn(delay: dHelp);
            KnownsIndicatorButton.AnimateFadeIn(delay: dHelp);
            CommonsIndicatorButton.AnimateFadeIn(delay: dHelp);
            RefreshMessageButton.AnimateFadeIn(delay: dAction);
            SaveKnownsButton.AnimateFadeIn(delay: dAction);
            SaveCommonsButton.AnimateFadeIn(delay: dAction);

            // Появление кнопки
            DoneButton.AnimatePulse(0.3, 1, 3, dDone);

            // Фокус на поле ввода
            CommonsTextBox.Focus();
        }

        public async Task PlayExitAnimation()
        {
            this.AnimateFadeOut();

            InfoMessageText.AnimateFadeOut();
            KnownsTextBox.AnimateFadeOut();
            CommonsTextBox.AnimateFadeOut();

            LeftIndicatorButton.AnimateFadeOut();
            KnownsIndicatorButton.AnimateFadeOut();
            CommonsIndicatorButton.AnimateFadeOut();
            RefreshMessageButton.AnimateFadeOut();
            SaveKnownsButton.AnimateFadeOut();
            SaveCommonsButton.AnimateFadeOut();
        }

        private async void DoneButton_Click(object sender, RoutedEventArgs e)
        {
            await Model.ApproveMessage();
            await MainWindow.SwitchToNextSlide();
        }

        private async void RefreshMessageButton_Click(object sender, RoutedEventArgs e)
        {
            Model.RefreshCallsMessage();
            //InfoMessageText.AnimateFadeIn();
            InfoMessageText.AnimateShake(delay:1);
        }

        private async void SaveKnownsButton_Click(object sender, RoutedEventArgs e)
        {
            Model.ApproveKnowns();
            Model.RefreshCallsMessage();
            InfoMessageText.AnimateFadeIn(delay:1);
            KnownsTextBox.AnimateShake();
        }

        private void SaveCommonsButton_Click(object sender, RoutedEventArgs e)
        {
            Model.ApproveCommons();
            Model.RefreshCallsMessage();
            InfoMessageText.AnimateFadeIn(delay: 1);
            CommonsTextBox.AnimateShake();
        }
    }
}