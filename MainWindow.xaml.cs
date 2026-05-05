using MovieIntro.Controls;
using PerfectohubRu.Controls;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace MovieIntro
{
    public partial class MainWindow : Window
    {
        private int currentSlideIndex = 0;
        private DispatcherTimer slideTimer;
        private readonly ISlide[] slides;

        public MainWindow()
        {
            InitializeComponent();

            // Получаем ссылки на контролы
            slides = new ISlide[] { Slide1Control, Slide2Control, Slide3Control, SlideCallsInfoControl, SlideAddAtsControl };

            // Подписываемся на событие отправки токена
            SlideAddAtsControl.TokenSubmitted += SlideAddAts_TokenSubmitted;

            // Клик по окну для пропуска вступления
            this.MouseLeftButtonDown += (s, e) => SkipIntro();

            StartIntroSequence();
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private async void StartIntroSequence()
        {
            slideTimer = new DispatcherTimer();
            slideTimer.Interval = TimeSpan.FromSeconds(10); // 3 секунды на слайд

            await ShowSlide(0);
            slideTimer.Tick += SlideTimer_Tick;
            slideTimer.Start();
        }

        private async void SlideTimer_Tick(object sender, EventArgs e)
        {
            if (currentSlideIndex == slides.Length - 1) // После 4-го слайда останавливаем таймер
            {
                slideTimer.Stop();
                return;
            }

            await SwitchToNextSlide();
        }

        private async Task SwitchToNextSlide()
        {
            // Анимируем выход текущего слайда
            await ExitCurrentSlide();

            // Переключаем на следующий
            currentSlideIndex++;
            await ShowSlide(currentSlideIndex);
        }

        private async Task ShowSlide(int index)
        {
            var slide = slides[index];

            slide.Visibility = Visibility.Visible;
            await slide.PlayEnterAnimation();
            UpdateIndicator(index);
        }

        private async Task ExitCurrentSlide()
        {
            var slide = slides[currentSlideIndex];
            await slide.PlayExitAnimation();
            slide.Visibility = Visibility.Collapsed;
        }

        private void UpdateIndicator(int activeIndex)
        {
            var i = 0;

            foreach (Ellipse indicator in Indicators.Children)
            {
                indicator.Opacity = i == activeIndex ? 1.0 : 0.2;

                if (i == activeIndex)
                {
                    var pulse = new DoubleAnimation(0.5, 1, TimeSpan.FromSeconds(0.8));
                    pulse.AutoReverse = true;
                    pulse.RepeatBehavior = new RepeatBehavior(2);
                    indicator.BeginAnimation(Ellipse.OpacityProperty, pulse);
                }
                else
                {
                    indicator.BeginAnimation(Ellipse.OpacityProperty, null);
                }

                i++;
            }
        }

        private async void SlideAddAts_TokenSubmitted(object sender, string token)
        {
            // Здесь обрабатываем полученный токен
            MessageBox.Show($"Токен получен: {token}", "Успешно",
                           MessageBoxButton.OK, MessageBoxImage.Information);

            // Можно добавить анимацию успеха и закрыть окно
            var fadeOut = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.5));
            SlidesContainer.BeginAnimation(UIElement.OpacityProperty, fadeOut);
            await Task.Delay(500);
            Close();
        }

        private async void SkipIntro()
        {
            if (slideTimer != null && slideTimer.IsEnabled)
            {
                slideTimer.Stop();

                var fadeOut = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.3));
                SlidesContainer.BeginAnimation(UIElement.OpacityProperty, fadeOut);
                await Task.Delay(300);

                // Показываем сразу 4-й слайд
                foreach (var slide in slides.Take(slides.Length - 1))
                    slide.Visibility = Visibility.Collapsed;

                SlidesContainer.Opacity = 1;

                currentSlideIndex = slides.Length - 1;
                await ShowSlide(currentSlideIndex);
            }
        }
    }
}