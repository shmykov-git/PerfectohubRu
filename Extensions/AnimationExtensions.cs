using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace PerfectohubRu.Extensions
{

    public static class AnimationExtensions
    {
        //private static void WithStoryboard(this UIElement element, AnimationTimeline animation, int fps, params string[] properties)
        //{
        //    Storyboard storyboard = new Storyboard();
        //    storyboard.Children.Add(animation);

        //    // Устанавливаем частоту кадров для Storyboard
        //    Timeline.SetDesiredFrameRate(storyboard, fps);

        //    // Привязываем анимацию к свойству
        //    Storyboard.SetTarget(animation, element);
            
        //    foreach(var property in properties)
        //        Storyboard.SetTargetProperty(animation, new PropertyPath(property));

        //    // Запускаем
        //    storyboard.Begin();
        //}

        public static async void AnimateFadeIn(this UIElement element, double duration = 1.5, double delay = 0)
        {
            await Task.Delay(TimeSpan.FromSeconds(delay));
            var fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(duration));
            element.BeginAnimation(UIElement.OpacityProperty, fadeIn);
        }

        public static async void AnimateFadeOut(this UIElement element, double duration = 1.5, double delay = 0)
        {
            await Task.Delay(TimeSpan.FromSeconds(delay));
            var fadeOut = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(duration));
            element.BeginAnimation(UIElement.OpacityProperty, fadeOut);
        }

        public static async void AnimateFade(this UIElement element, (double from, double to) range, double duration = 1.5, double delay = 0)
        {
            await Task.Delay(TimeSpan.FromSeconds(delay));
            var fade = new DoubleAnimation(range.from, range.to, TimeSpan.FromSeconds(duration));
            element.BeginAnimation(UIElement.OpacityProperty, fade);
        }

        public static async void AnimateShake(this UIElement element, double scale = 1, double delay = 0)
        {
            await Task.Delay(TimeSpan.FromSeconds(delay));
            var shakeAnimation = new DoubleAnimationUsingKeyFrames();
            shakeAnimation.KeyFrames.Add(new EasingDoubleKeyFrame(-5, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(0))));
            shakeAnimation.KeyFrames.Add(new EasingDoubleKeyFrame(5, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(50 * scale))));
            shakeAnimation.KeyFrames.Add(new EasingDoubleKeyFrame(-5, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(100 * scale))));
            shakeAnimation.KeyFrames.Add(new EasingDoubleKeyFrame(5, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(150 * scale))));
            shakeAnimation.KeyFrames.Add(new EasingDoubleKeyFrame(0, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(200 * scale))));

            var translateTransform = new TranslateTransform();
            element.RenderTransform = translateTransform;
            translateTransform.BeginAnimation(TranslateTransform.XProperty, shakeAnimation);
        }

        public static async void AnimateColorFlash(this Border border, Brush brush = null, double duration = 0.5, double delay = 0)
        {
            await Task.Delay(TimeSpan.FromSeconds(delay));
            var originalBrush = border.BorderBrush;
            border.BorderBrush = brush ?? Brushes.Red;
            await Task.Delay(TimeSpan.FromSeconds(duration));
            border.BorderBrush = originalBrush;
        }

        public static async void AnimateScaleX(this UIElement element, double duration = 0.3, double delay = 0)
        {
            await Task.Delay(TimeSpan.FromSeconds(delay));
            
            var scaleX = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(duration));

            if (element.RenderTransform is ScaleTransform scale)
            {
                scale.BeginAnimation(ScaleTransform.ScaleXProperty, scaleX);
            }
            else if (element.RenderTransform is null)
            {
                element.RenderTransform = scale = new ScaleTransform();
                scale.BeginAnimation(ScaleTransform.ScaleXProperty, scaleX);

                await Task.Delay(TimeSpan.FromSeconds(duration + 0.1));
                element.RenderTransform = null;
            }
        }

        public static async void AnimateScale(this UIElement element, (double from, double to) range, double duration = 0.3, double delay = 0)
        {
            await Task.Delay(TimeSpan.FromSeconds(delay));

            var scale = new DoubleAnimation(range.from, range.to, TimeSpan.FromSeconds(duration));

            //element.WithStoryboard(scale, 15, "RenderTransform.ScaleX", "RenderTransform.ScaleY");

            if (element.RenderTransform is ScaleTransform scaleTransform)
            {
                scaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, scale);
                scaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, scale);
            }
        }

        public static async void AnimatePulse(this UIElement element, (double from, double to) range, double duration = 0.8, int repeat = 2, double delay = 0)
        {
            await Task.Delay(TimeSpan.FromSeconds(delay));

            var pulse = new DoubleAnimation(range.from, range.to, TimeSpan.FromSeconds(duration));
            pulse.AutoReverse = true;
            pulse.RepeatBehavior = new RepeatBehavior(repeat);
            element.BeginAnimation(UIElement.OpacityProperty, pulse);

            await Task.Delay(TimeSpan.FromSeconds(duration * repeat + 0.1));
            element.BeginAnimation(UIElement.OpacityProperty, null);
            element.Opacity = range.to;
        }
    }
}
