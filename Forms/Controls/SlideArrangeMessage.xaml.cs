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
            InfoMessageText.AnimateFadeIn();
            Model.RefreshCallsMessage();
            AnimateIndicators();
        }

        // Установка информационного сообщения
        public void SetInfoMessage(string message)
        {
            InfoMessageText.Text = message;
        }

        // Получение введенного текста
        public string GetInputText()
        {
            return KnownsTextBox.Text;
        }

        private void AnimateIndicators()
        {
            LeftIndicatorButton.AnimateFadeIn();
            KnownsIndicatorButton.AnimateFadeIn();
            CommonsIndicatorButton.AnimateFadeIn();
            RefreshMessageButton.AnimateFadeIn(delay:2);
            SaveKnownsButton.AnimateFadeIn(delay: 2);
            SaveCommonsButton.AnimateFadeIn(delay: 2);
        }

        public async Task PlayEnterAnimation()
        {
            //this.Opacity = 0;
            this.Visibility = Visibility.Visible;

            TitleText.AnimateFadeIn(2);

            var d1 = 0.2;
            InfoMessageText.AnimateFadeIn(1.6, d1);
            InfoMessageText.AnimateScale((0.5, 1), 2, d1);
            KnownsTextBox.AnimateFadeIn(1.6, d1);
            KnownsTextBox.AnimateScale((0.5, 1), d1);
            CommonsTextBox.AnimateFadeIn(1.6, d1);
            CommonsTextBox.AnimateScale((0.5, 1), d1);

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
            var fadeOut = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.5));
            this.BeginAnimation(UIElement.OpacityProperty, fadeOut);
            await Task.Delay(500);
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