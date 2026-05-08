using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media;
using PerfectohubRu.Controls;
using PerfectohubRu.Forms.ViewModles;
using PerfectohubRu.Extensions;

namespace MovieIntro.Controls
{
    public partial class SlideAddAts : UserControl, ISlide
    {
        public SlideAddAts()
        {
            InitializeComponent();
            TokenTextBox.TextChanged += TokenTextBox_TextChanged;
        }

        private MainWindow MainWindow => Window.GetWindow(this) as MainWindow;
        public MainViewModel Model => DataContext as MainViewModel;

        public async Task PlayEnterAnimation()
        {
            this.Visibility = Visibility.Visible;

            this.AnimateFadeIn();
            WelcomeText.AnimateFadeIn();
            TokenTextBox.AnimateFadeIn();
            SubmitButton.AnimateFadeIn();

            ArrowImage.AnimateFadeIn();
            ArrowImage.AnimateScale((0.3, 1), 1.6);
            ResultImage.AnimateFadeIn();
            ResultImage.AnimateScale((0.3, 1), 1.6);
        }

        public async Task PlayExitAnimation() 
        {
            this.AnimateFadeOut();
            WelcomeText.AnimateFadeOut();
            TokenTextBox.AnimateFadeOut();
            SubmitButton.AnimateFadeOut();

            ArrowImage.AnimateFadeOut();
            ArrowImage.AnimateScale((1, 0.3), 1.6);
            ResultImage.AnimateFadeOut();
            ResultImage.AnimateScale((1, 0.3), 1.6);
        }

        private void TokenTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Анимация линии фокуса
            if (!string.IsNullOrWhiteSpace(TokenTextBox.Text))
            {
                FocusLine.Opacity = 1;
                var scaleX = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.3));
                if (FocusLine.RenderTransform is ScaleTransform scale)
                {
                    scale.BeginAnimation(ScaleTransform.ScaleXProperty, scaleX);
                }
            }
            else
            {
                var fadeOut = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.3));
                FocusLine.BeginAnimation(UIElement.OpacityProperty, fadeOut);
            }

            // Скрываем ошибку при вводе
            if (ErrorMessage.Opacity > 0)
            {
                _ = HideError();
            }
        }

        private async void HelpButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.ShowDialog<AtsHelpDialog>();
        }

        private async void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TokenTextBox.Text))
            {
                await ShowError("Нужен токен облачной АТС");
                return;
            }

            var result = await Model.ValidateAndSaveAtsToken();

            if (result.Success)
            {
                await MainWindow.SwitchToNextSlide();
            }
            else
            {
                await ShowError(result.Error);
            }
        }

        private async Task ShowError(string message)
        {
            ErrorMessage.Text = message;
            var fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.3));
            ErrorMessage.BeginAnimation(UIElement.OpacityProperty, fadeIn);

            // Эффект тряски для поля ввода
            var shakeAnimation = new DoubleAnimationUsingKeyFrames();
            shakeAnimation.KeyFrames.Add(new EasingDoubleKeyFrame(-5, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(0))));
            shakeAnimation.KeyFrames.Add(new EasingDoubleKeyFrame(5, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(50))));
            shakeAnimation.KeyFrames.Add(new EasingDoubleKeyFrame(-5, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(100))));
            shakeAnimation.KeyFrames.Add(new EasingDoubleKeyFrame(5, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(150))));
            shakeAnimation.KeyFrames.Add(new EasingDoubleKeyFrame(0, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(200))));

            var translateTransform = new TranslateTransform();
            TokenTextBox.RenderTransform = translateTransform;
            translateTransform.BeginAnimation(TranslateTransform.XProperty, shakeAnimation);

            await Task.Delay(3000);
            await HideError();
        }

        private async Task HideError()
        {
            var fadeOut = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.5));
            ErrorMessage.BeginAnimation(UIElement.OpacityProperty, fadeOut);
            await Task.Delay(500);
            ErrorMessage.Text = "";
        }

        public void ClearToken()
        {
            TokenTextBox.Text = "";
        }

        public void HideImages()
        {
            ResultImage.Opacity = 0;
            ArrowImage.Opacity = 0;
        }
    }
}