using PerfectohubRu.Controls;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace MovieIntro.Controls
{
    public partial class Slide3 : UserControl, ISlide
    {
        public Slide3()
        {
            InitializeComponent();
        }

        public async Task PlayEnterAnimation()
        {
            // Анимация текста
            var textFadeIn = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(1.2));
            var textScaleX = new DoubleAnimation(0.9, 1, TimeSpan.FromSeconds(1.2));
            var textScaleY = new DoubleAnimation(0.9, 1, TimeSpan.FromSeconds(1.2));

            MainText.BeginAnimation(UIElement.OpacityProperty, textFadeIn);
            if (MainText.RenderTransform is ScaleTransform scale)
            {
                scale.BeginAnimation(ScaleTransform.ScaleXProperty, textScaleX);
                scale.BeginAnimation(ScaleTransform.ScaleYProperty, textScaleY);
            }

            await Task.Delay(400);

            // Анимация линии
            //var lineFade = new DoubleAnimation(0, 0.3, TimeSpan.FromSeconds(0.8));
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