using Microsoft.Extensions.DependencyInjection;
using MovieIntro.Controls;
using PerfectohubRu.Controls;
using PerfectohubRu.Forms.ViewModles;
using PerfectohubRu.Model;
using PerfectohubRu.Tools;
using System;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace MovieIntro
{
    public partial class MainWindow : Window
    {
        private int currentSlideIndex = 5; // 4-atsToken, 5-arrangeMessage
        private DispatcherTimer slideTimer;
        private readonly ISlide[] slides;
        private readonly IServiceProvider sp;
        private readonly MainViewModel model;

        public MainWindow()
        {
            InitializeComponent();

            // Получаем ссылки на контролы
            slides = new ISlide[] { Slide1Control, Slide2Control, Slide3Control, SlideCallsInfoControl, SlideAddAtsControl, SlideArrangeMessageControl };

            // Клик по окну для пропуска вступления
            this.MouseLeftButtonDown += (s, e) => SkipIntro();
            this.KeyUp += (s, e) => SkipIntro();

            StartIntroSequence();
        }

        public MainWindow(IServiceProvider sp, MainViewModel model) : this()
        {
            this.sp = sp;
            this.model = model;

            foreach (UserControl slide in slides)
                slide.DataContext = model;
        }

        public void ShowDialog<TWindow>() where TWindow : Window
        {
            var window = sp.GetRequiredService<TWindow>();
            window.Owner = this;
            window.ShowDialog();
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                e.Handled = true;
                this.DragMove();
            }
        }

        private void ChatButton_Click(object sender, RoutedEventArgs e)
        {
            var supportChat = sp.GetRequiredService<SupportChat>();
            supportChat.Owner = this;

            if (!supportChat.IsVisible)
            {
                supportChat.PositionToRightOfOwner();
                supportChat.Show();
            }
            else
            {
                supportChat.Activate();
            }
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Hyperlink_Click(object sender, MouseButtonEventArgs e)
        {
            UrlHelper.OpenUrl("https://perfectohub.ru");
        }

        private async void StartIntroSequence()
        {
            slideTimer = new DispatcherTimer();
            slideTimer.Interval = TimeSpan.FromSeconds(5); // 3 секунды на слайд

            await ShowSlide(currentSlideIndex);
            slideTimer.Tick += SlideTimer_Tick;
            slideTimer.Start();
        }

        private async void SlideTimer_Tick(object sender, EventArgs e)
        {
            if (currentSlideIndex == slides.Length - 1)
            {
                slideTimer.Stop();
                return;
            }

            await SwitchToNextSlide();
        }

        public Task SwitchToNextSlide()
        {
            if (currentSlideIndex == 4 && model.ClientData.State == ClientState.New)
            {
                return SwitchToSlide(currentSlideIndex + 1);
            }
            else
            {
                if (model.ClientData.State == ClientState.HasAts)
                    return SwitchToSlide(currentSlideIndex + 2);
            }

            return SwitchToSlide(currentSlideIndex + 1);
        }

        private async Task SwitchToSlide(int index)
        {
            // Анимируем выход текущего слайда
            await ExitCurrentSlide();

            // Переключаем на следующий
            currentSlideIndex = index;
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

            if (activeIndex >= 4)
            {
                var showAnimate = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(2));
                Logo.BeginAnimation(Ellipse.OpacityProperty, showAnimate);
                Support.BeginAnimation(Ellipse.OpacityProperty, showAnimate);
            }
        }

        private async void SkipIntro()
        {
            if (slideTimer != null && slideTimer.IsEnabled)
            {
                slideTimer.Stop();
                slideTimer.IsEnabled = false;

                await SwitchToSlide(slides.Length - 1);
            }
        }
    }
}