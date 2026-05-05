using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media;

namespace MovieIntro.Controls
{
    public partial class SlideAddAts : UserControl
    {
        public event EventHandler<string> TokenSubmitted;

        public SlideAddAts()
        {
            InitializeComponent();
            SubmitButton.Click += SubmitButton_Click;
            TokenTextBox.TextChanged += TokenTextBox_TextChanged;
        }

        public async Task PlayEnterAnimation()
        {
            this.Opacity = 0;
            this.Visibility = Visibility.Visible;

            // Анимация появления всего слайда
            var scaleAnim = new DoubleAnimation(0.95, 1, TimeSpan.FromSeconds(0.8));
            if (this.RenderTransform is ScaleTransform scale)
            {
                scale.BeginAnimation(ScaleTransform.ScaleXProperty, scaleAnim);
                scale.BeginAnimation(ScaleTransform.ScaleYProperty, scaleAnim);
            }

            var fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.6));
            this.BeginAnimation(UIElement.OpacityProperty, fadeIn);

            await Task.Delay(200);

            // Появление заголовка
            var textFade = new DoubleAnimation(0, 0.9, TimeSpan.FromSeconds(0.8));
            WelcomeText.BeginAnimation(UIElement.OpacityProperty, textFade);

            await Task.Delay(300);

            // Появление поля ввода
            var inputFade = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.5));
            TokenTextBox.BeginAnimation(UIElement.OpacityProperty, inputFade);

            await Task.Delay(200);

            // Появление кнопки
            var buttonFade = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.5));
            SubmitButton.BeginAnimation(UIElement.OpacityProperty, buttonFade);

            await Task.Delay(500);

            // Появление картинок
            await ShowImages();
        }

        private async Task ShowImages()
        {
            // Появление большой картинки
            var imgFadeIn = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(1.6));
            var imgScaleX = new DoubleAnimation(0.3, 1, TimeSpan.FromSeconds(1.6));
            var imgScaleY = new DoubleAnimation(0.3, 1, TimeSpan.FromSeconds(1.6));

            ResultImage.BeginAnimation(UIElement.OpacityProperty, imgFadeIn);
            if (ResultImage.RenderTransform is ScaleTransform imgScale)
            {
                imgScale.BeginAnimation(ScaleTransform.ScaleXProperty, imgScaleX);
                imgScale.BeginAnimation(ScaleTransform.ScaleYProperty, imgScaleY);
            }

            await Task.Delay(300);

            // Появление картинки-стрелки
            var arrowFadeIn = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(1.6));
            var arrowScaleX = new DoubleAnimation(0.3, 1, TimeSpan.FromSeconds(1.6));
            var arrowScaleY = new DoubleAnimation(0.3, 1, TimeSpan.FromSeconds(1.6));

            ArrowImage.BeginAnimation(UIElement.OpacityProperty, arrowFadeIn);

            if (ArrowImage.RenderTransform is ScaleTransform arrowScale)
            {
                arrowScale.BeginAnimation(ScaleTransform.ScaleXProperty, arrowScaleX);
                arrowScale.BeginAnimation(ScaleTransform.ScaleYProperty, arrowScaleY);
            }
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
            var mainWindow = Window.GetWindow(this) as MainWindow;
            mainWindow.ShowDialog<AtsHelpDialog>();
        }

        private async void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TokenTextBox.Text))
            {
                await ShowError("Нужен токен облачной АТС");
                return;
            }

            // Анимация нажатия
            var shrink = new DoubleAnimation(0.95, 1, TimeSpan.FromSeconds(0.2));
            SubmitButton.BeginAnimation(Button.RenderTransformProperty, shrink);

            TokenSubmitted?.Invoke(this, TokenTextBox.Text);
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