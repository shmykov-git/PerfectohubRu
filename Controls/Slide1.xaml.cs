using PerfectohubRu.Controls;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace MovieIntro.Controls
{
    public partial class Slide1 : UserControl, ISlide
    {
        public Slide1()
        {
            InitializeComponent();
        }

        public async Task PlayEnterAnimation()
        {
            return;
            // Анимация для картинки
            var imgFadeIn = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.8));
            var imgScaleX = new DoubleAnimation(0.5, 1, TimeSpan.FromSeconds(0.8));
            var imgScaleY = new DoubleAnimation(0.5, 1, TimeSpan.FromSeconds(0.8));

            LogoImage.BeginAnimation(UIElement.OpacityProperty, imgFadeIn);
            if (LogoImage.RenderTransform is ScaleTransform imgScale)
            {
                imgScale.BeginAnimation(ScaleTransform.ScaleXProperty, imgScaleX);
                imgScale.BeginAnimation(ScaleTransform.ScaleYProperty, imgScaleY);
            }

            await Task.Delay(200);

            // Анимация для текста
            var textFadeIn = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(1.2));
            var textScaleX = new DoubleAnimation(0.8, 1, TimeSpan.FromSeconds(1.2));
            var textScaleY = new DoubleAnimation(0.8, 1, TimeSpan.FromSeconds(1.2));

            TitleText.BeginAnimation(UIElement.OpacityProperty, textFadeIn);
            if (TitleText.RenderTransform is ScaleTransform textScale)
            {
                textScale.BeginAnimation(ScaleTransform.ScaleXProperty, textScaleX);
                textScale.BeginAnimation(ScaleTransform.ScaleYProperty, textScaleY);
            }
        }

        public async Task PlayExitAnimation()
        {
            var fadeOut = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.5));
            this.BeginAnimation(UIElement.OpacityProperty, fadeOut);
            await Task.Delay(500);
        }
    }
}