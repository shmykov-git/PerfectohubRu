using PerfectohubRu.Controls;
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
            InputTextBox.TextChanged += InputTextBox_TextChanged;
            this.Loaded += SlideArrangeMessage_Loaded;
        }

        private async void SlideArrangeMessage_Loaded(object sender, RoutedEventArgs e)
        {
            await FadeInElement(InfoMessageText);
            Model.RefreshCallsMessage();
            await AnimateIndicators();
        }

        // Установка информационного сообщения
        public void SetInfoMessage(string message)
        {
            InfoMessageText.Text = message;
        }

        // Получение введенного текста
        public string GetInputText()
        {
            return InputTextBox.Text;
        }

        // Очистка поля ввода
        public void ClearInput()
        {
            InputTextBox.Clear();
        }

        private async Task FadeInElement(UIElement element, double seconds = 1.5)
        {
            var fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(seconds));
            element.BeginAnimation(UIElement.OpacityProperty, fadeIn);
        }

        private async Task AnimateIndicators()
        {
            await FadeInElement(LeftIndicatorButton);
            await FadeInElement(RightIndicatorButton);
            await Task.Delay(2000);
            await FadeInElement(RefreshMessageButton);
            await FadeInElement(SaveKnownsButton);            
        }

        public async Task PlayEnterAnimation()
        {
            this.Opacity = 0;
            this.Visibility = Visibility.Visible;

            // Анимация масштаба всего слайда
            var scaleAnim = new DoubleAnimation(0.95, 1, TimeSpan.FromSeconds(0.8));
            if (this.RenderTransform is ScaleTransform scale)
            {
                scale.BeginAnimation(ScaleTransform.ScaleXProperty, scaleAnim);
                scale.BeginAnimation(ScaleTransform.ScaleYProperty, scaleAnim);
            }

            // Появление слайда
            var fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.6));
            this.BeginAnimation(UIElement.OpacityProperty, fadeIn);

            await Task.Delay(200);

            // Появление заголовка
            var titleFade = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.6));
            TitleText.BeginAnimation(UIElement.OpacityProperty, titleFade);

            await Task.Delay(200);

            // Появление левой панели (информация)
            var infoFadeIn = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(1.6));
            var infoScaleX = new DoubleAnimation(0.5, 1, TimeSpan.FromSeconds(1.6));
            var infoScaleY = new DoubleAnimation(0.5, 1, TimeSpan.FromSeconds(1.6));

            InfoMessageText.BeginAnimation(UIElement.OpacityProperty, infoFadeIn);
            if (InfoMessageText.RenderTransform is ScaleTransform infoScale)
            {
                infoScale.BeginAnimation(ScaleTransform.ScaleXProperty, infoScaleX);
                infoScale.BeginAnimation(ScaleTransform.ScaleYProperty, infoScaleY);
            }

            await Task.Delay(100);

            // Появление правой панели (ввод)
            var inputFadeIn = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(1.6));
            var inputScaleX = new DoubleAnimation(0.5, 1, TimeSpan.FromSeconds(1.6));
            var inputScaleY = new DoubleAnimation(0.5, 1, TimeSpan.FromSeconds(1.6));

            InputTextBox.BeginAnimation(UIElement.OpacityProperty, inputFadeIn);
            if (InputTextBox.RenderTransform is ScaleTransform inputScale)
            {
                inputScale.BeginAnimation(ScaleTransform.ScaleXProperty, inputScaleX);
                inputScale.BeginAnimation(ScaleTransform.ScaleYProperty, inputScaleY);
            }

            await Task.Delay(200);

            // Появление кнопки
            var buttonFade = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.5));
            DoneButton.BeginAnimation(UIElement.OpacityProperty, buttonFade);

            // Фокус на поле ввода
            await Task.Delay(300);
            InputTextBox.Focus();
        }

        private void InputTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Анимация линии фокуса при вводе текста
            if (!string.IsNullOrWhiteSpace(InputTextBox.Text))
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
        }

        private async void DoneButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(InputTextBox.Text))
            {
                await AnimateEmptyInput();
                return;
            }



            //// Анимация нажатия
            //var shrink = new DoubleAnimation(0.95, 1, TimeSpan.FromSeconds(0.2));
            //DoneButton.BeginAnimation(Button.RenderTransformProperty, shrink);

            //// Вызываем событие с введенным текстом
            //MessageCompleted?.Invoke(this, InputTextBox.Text);
        }

        private async Task AnimateEmptyInput()
        {
            // Тряска правой панели
            var shakeAnimation = new DoubleAnimationUsingKeyFrames();
            shakeAnimation.KeyFrames.Add(new EasingDoubleKeyFrame(-5, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(0))));
            shakeAnimation.KeyFrames.Add(new EasingDoubleKeyFrame(5, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(50))));
            shakeAnimation.KeyFrames.Add(new EasingDoubleKeyFrame(-5, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(100))));
            shakeAnimation.KeyFrames.Add(new EasingDoubleKeyFrame(5, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(150))));
            shakeAnimation.KeyFrames.Add(new EasingDoubleKeyFrame(0, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(200))));

            var translateTransform = new TranslateTransform();
            InputTextBox.RenderTransform = translateTransform;
            translateTransform.BeginAnimation(TranslateTransform.XProperty, shakeAnimation);

            // Красная подсветка границы
            var parent = InputTextBox.Parent as Grid;
            if (parent != null && parent.Parent is Border border)
            {
                var originalBrush = border.BorderBrush;
                border.BorderBrush = Brushes.Red;
                await Task.Delay(500);
                border.BorderBrush = originalBrush;
            }
        }

        public async Task PlayExitAnimation()
        {
            var fadeOut = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.5));
            this.BeginAnimation(UIElement.OpacityProperty, fadeOut);
            await Task.Delay(500);
        }

        private async void RefreshMessageButton_Click(object sender, RoutedEventArgs e)
        {
            await FadeInElement(InfoMessageText);
            Model.RefreshCallsMessage();
        }

        private async void SaveKnownsButton_Click(object sender, RoutedEventArgs e)
        {
            Model.SaveKnowns();
            await FadeInElement(InfoMessageText);
            Model.RefreshCallsMessage();
        }
    }
}