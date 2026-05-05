using PerfectohubRu.Controls;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace MovieIntro.Controls
{
    public partial class SlideCallsInfo : UserControl, ISlide
    {
        private Storyboard pulseStoryboard;

        public SlideCallsInfo()
        {
            InitializeComponent();
            CreatePulseAnimation();
        }

        private void CreatePulseAnimation()
        {
            pulseStoryboard = new Storyboard();

            var pulseAnimation = new DoubleAnimation
            {
                From = 0.2,
                To = 1,
                Duration = TimeSpan.FromSeconds(1),
                AutoReverse = true,
                RepeatBehavior = RepeatBehavior.Forever
            };

            Storyboard.SetTarget(pulseAnimation, PulseDot);
            Storyboard.SetTargetProperty(pulseAnimation, new PropertyPath(UIElement.OpacityProperty));
            pulseStoryboard.Children.Add(pulseAnimation);

            var scaleAnimation = new DoubleAnimation
            {
                From = 0.5,
                To = 1.5,
                Duration = TimeSpan.FromSeconds(1),
                AutoReverse = true,
                RepeatBehavior = RepeatBehavior.Forever
            };

            Storyboard.SetTarget(scaleAnimation, PulseDot);
            Storyboard.SetTargetProperty(scaleAnimation,
                new PropertyPath("(UIElement.RenderTransform).(ScaleTransform.ScaleX)"));
            pulseStoryboard.Children.Add(scaleAnimation);

            var scaleYAnimation = new DoubleAnimation
            {
                From = 0.5,
                To = 1.5,
                Duration = TimeSpan.FromSeconds(1),
                AutoReverse = true,
                RepeatBehavior = RepeatBehavior.Forever
            };

            Storyboard.SetTarget(scaleYAnimation, PulseDot);
            Storyboard.SetTargetProperty(scaleYAnimation,
                new PropertyPath("(UIElement.RenderTransform).(ScaleTransform.ScaleY)"));
            pulseStoryboard.Children.Add(scaleYAnimation);
        }

        public async Task PlayEnterAnimation()
        {
            this.Opacity = 0;
            this.Visibility = Visibility.Visible;

            var scaleAnim = new DoubleAnimation(0.95, 1, TimeSpan.FromSeconds(0.8));
            if (this.RenderTransform is ScaleTransform scale)
            {
                scale.BeginAnimation(ScaleTransform.ScaleXProperty, scaleAnim);
                scale.BeginAnimation(ScaleTransform.ScaleYProperty, scaleAnim);
            }

            var fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.6));
            this.BeginAnimation(UIElement.OpacityProperty, fadeIn);

            await Task.Delay(200);

            // Анимация иконки
            var iconFadeIn = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.8));
            var iconScaleX = new DoubleAnimation(0.5, 1, TimeSpan.FromSeconds(0.8));
            var iconScaleY = new DoubleAnimation(0.5, 1, TimeSpan.FromSeconds(0.8));

            IconText.BeginAnimation(UIElement.OpacityProperty, iconFadeIn);
            if (IconText.RenderTransform is ScaleTransform iconScale)
            {
                iconScale.BeginAnimation(ScaleTransform.ScaleXProperty, iconScaleX);
                iconScale.BeginAnimation(ScaleTransform.ScaleYProperty, iconScaleY);
            }

            await Task.Delay(200);

            // Анимация основного текста
            var textFadeIn = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(1));
            var textScaleX = new DoubleAnimation(0.9, 1, TimeSpan.FromSeconds(1));
            var textScaleY = new DoubleAnimation(0.9, 1, TimeSpan.FromSeconds(1));

            MainText.BeginAnimation(UIElement.OpacityProperty, textFadeIn);
            if (MainText.RenderTransform is ScaleTransform textScale)
            {
                textScale.BeginAnimation(ScaleTransform.ScaleXProperty, textScaleX);
                textScale.BeginAnimation(ScaleTransform.ScaleYProperty, textScaleY);
            }

            await Task.Delay(150);

            // Анимация подзаголовка
            var subFadeIn = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.8));
            var subScaleX = new DoubleAnimation(0.9, 1, TimeSpan.FromSeconds(0.8));
            var subScaleY = new DoubleAnimation(0.9, 1, TimeSpan.FromSeconds(0.8));

            SubText.BeginAnimation(UIElement.OpacityProperty, subFadeIn);
            if (SubText.RenderTransform is ScaleTransform subScale)
            {
                subScale.BeginAnimation(ScaleTransform.ScaleXProperty, subScaleX);
                subScale.BeginAnimation(ScaleTransform.ScaleYProperty, subScaleY);
            }

            await Task.Delay(200);

            // Анимация линии
            //var lineFade = new DoubleAnimation(0, 0.3, TimeSpan.FromSeconds(0.6));
            //Line.BeginAnimation(UIElement.OpacityProperty, lineFade);

            //await Task.Delay(300);

            // Запуск пульсации
            PulseDot.Opacity = 1;
            pulseStoryboard.Begin();
        }

        public async Task PlayExitAnimation()
        {
            pulseStoryboard?.Stop();

            var fadeOut = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.5));
            this.BeginAnimation(UIElement.OpacityProperty, fadeOut);
            await Task.Delay(500);
        }
    }
}