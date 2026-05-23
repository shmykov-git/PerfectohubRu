using PerfectohubRu.Controls;
using PerfectohubRu.Extensions;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace MovieIntro.Controls
{
    public partial class Slide4 : UserControl, ISlide
    {
        public Slide4()
        {
            InitializeComponent();
        }

        public async Task PlayEnterAnimation()
        {
            this.Opacity = 0;
            this.Visibility = Visibility.Visible;

            this.AnimateScale((0.95, 1), 0.6);
            this.AnimateFadeIn(0.6);

            await Task.Delay(200);

            // Анимация иконки
            IconText.AnimateFadeIn(0.8);
            IconText.AnimateScale((0.5, 1), 0.8);

            await Task.Delay(200);

            // Анимация основного текста
            MainText.AnimateFadeIn(1);
            MainText.AnimateScale((0.9, 1), 1);

            await Task.Delay(150);

            // Анимация подзаголовка
            SubText.AnimateFadeIn(0.8);
            SubText.AnimateScale((0.9, 1), 1);

            await Task.Delay(1000);

            // Анимация подзаголовка ... и не только
            SubText2.AnimateFadeIn(1.2);
            SubText2.AnimateScale((0.5, 1), 3);
        }

        public async Task PlayExitAnimation()
        {
            var fadeOut = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.5));
            this.BeginAnimation(UIElement.OpacityProperty, fadeOut);
            await Task.Delay(500);
        }
    }
}