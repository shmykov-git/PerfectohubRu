using PerfectohubRu.Controls;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace MovieIntro.Controls
{
    public partial class Slide2 : UserControl, ISlide
    {
        public Slide2()
        {
            InitializeComponent();
        }

        public async Task PlayEnterAnimation()
        {
            // Анимация для подзаголовка
            var subtitleFade = new DoubleAnimation(0, 0.6, TimeSpan.FromSeconds(0.8));
            SubtitleText.BeginAnimation(UIElement.OpacityProperty, subtitleFade);

            await Task.Delay(300);

            // Анимация для главного текста
            var textFadeIn = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(1));
            var textScaleX = new DoubleAnimation(0.9, 1, TimeSpan.FromSeconds(1));
            var textScaleY = new DoubleAnimation(0.9, 1, TimeSpan.FromSeconds(1));

            MainText.BeginAnimation(UIElement.OpacityProperty, textFadeIn);
            if (MainText.RenderTransform is ScaleTransform scale)
            {
                scale.BeginAnimation(ScaleTransform.ScaleXProperty, textScaleX);
                scale.BeginAnimation(ScaleTransform.ScaleYProperty, textScaleY);
            }

            await Task.Delay(200);

            // Анимация линии
            //var lineFade = new DoubleAnimation(0, 0.3, TimeSpan.FromSeconds(0.5));
            //Line.BeginAnimation(UIElement.OpacityProperty, lineFade);
        }

        public async Task PlayExitAnimation()
        {
            var fadeOut = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.5));
            this.BeginAnimation(UIElement.OpacityProperty, fadeOut);
            await Task.Delay(500);
        }
    }
}