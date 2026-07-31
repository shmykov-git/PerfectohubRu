using Microsoft.Extensions.DependencyInjection;
using MovieIntro.Controls;
using PerfectohubRu.Controls;
using PerfectohubRu.Extensions;
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
        private const int START = 0;
        private const int CAPTION = 3;
        private const int ATSTOKEN = 4;
        private const int ARRANGEMESSAGE = 5;
        private const int BOTTOKEN = 6;
        private const int BOTMESSAGE = 7;
        private const int INTEGRATION = 8;

        private int currentSlideIndex = START;// ATSTOKEN;
        private DispatcherTimer slideTimer;
        private readonly ISlide[] slides;
        private readonly IServiceProvider sp;
        private readonly MainViewModel model;

        public MainWindow()
        {
            InitializeComponent();

            // Получаем ссылки на контролы
            slides = SlidesContainer.Children.Select(e => e as ISlide).ToArray();

            // Клик по окну для пропуска вступления
            this.MouseLeftButtonDown += (s, e) => SkipIntro();
            this.KeyUp += (s, e) => SkipIntro();
            this.Loaded += (s, e) => 
            {
                StartIntroSequence();
                model.PropertyChanged += Model_PropertyChanged;
            };
        }

        private async void Model_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(model.AtsToken))
            {
                await SwitchToSlide(ATSTOKEN);
            }

            if (e.PropertyName == nameof(model.BotToken))
            {
                await SwitchToSlide(BOTTOKEN);
            }
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

        public void OpenSettingsWindow()
        {
            var settings = sp.GetRequiredService<SettingsWindow>();
            settings.Owner = this;

            if (!settings.IsVisible)
            {
                if (!settings.IsLoaded)
                    settings.PositionToRightOfOwner();

                settings.Show();
            }
            else
            {
                settings.Hide();
            }
        }

        private void StatusButton_Click(object sender, RoutedEventArgs e)
        {
            OpenSettingsWindow();
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
            if (currentSlideIndex >= 4)
            {
                slideTimer.Stop();
                return;
            }

            await SwitchToNextSlide();
        }

        public Task SwitchToNextSlide()
        {
            if (currentSlideIndex == 3)
            {
                if (model.Data.State == ClientState.New)
                    return SwitchToSlide(currentSlideIndex + 1);

                if (model.Data.State == ClientState.HasAts)
                    return SwitchToSlide(currentSlideIndex + 2);

                if (model.Data.State == ClientState.HasMessage)
                    return SwitchToSlide(currentSlideIndex + 3);

                if (model.Data.State == ClientState.HasBot)
                    return SwitchToSlide(currentSlideIndex + 4);
            }

            return SwitchToSlide(currentSlideIndex + 1);
        }

        public Task SwitchToPrevSlide()
        {
            return SwitchToSlide(currentSlideIndex - 1);
        }

        private async Task SwitchToSlide(int index)
        {
            if (currentSlideIndex == index)
                return;

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

            if (index >= 5)
            {
                if (!MovePrevButton.IsEnabled)
                {
                    MovePrevButton.IsEnabled = true;
                    MovePrevButton.AnimateFadeIn(0.3);
                }
            }
            else
            {
                if (MovePrevButton.IsEnabled)
                {
                    MovePrevButton.IsEnabled = false;
                    MovePrevButton.AnimateFadeOut(0.3);
                }
            }

            if (index >= 4 && (index - 4) < (int)model.Data.State)
            {
                if (!MoveNextButton.IsEnabled)
                {
                    MoveNextButton.IsEnabled = true;
                    MoveNextButton.AnimateFadeIn(1, 1);
                }
            }
            else
            {
                if (MoveNextButton.IsEnabled)
                {
                    MoveNextButton.IsEnabled = false;
                    MoveNextButton.AnimateFadeOut(0.3);
                }
            }

            if (index >= 4)
            {
                Logo.AnimateFadeIn(2);
                Support.AnimateFadeIn(2);
            }

            UpdateIndicator(index);
            await slide.PlayEnterAnimation();
        }

        private async Task ExitCurrentSlide()
        {
            var slide = slides[currentSlideIndex];
            await slide.PlayExitAnimation();
            slide.Visibility = Visibility.Collapsed;
        }

        private async void UpdateIndicator(int index)
        {
            var indicators = Indicators.Children.Select(e => e as Ellipse).ToArray();

            foreach (Ellipse indicator in indicators)
                if (indicator.Opacity > 0.2)
                    indicator.AnimateFade((indicator.Opacity, 0.2), 1);

            indicators[index].AnimatePulse(0.2, 0.6, 3);
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

        private async void MovePrevButton_Click(object sender, RoutedEventArgs e)
        {
            await SwitchToPrevSlide();
        }

        private async void MoveNextButton_Click(object sender, RoutedEventArgs e)
        {
            await SwitchToNextSlide();
        }
    }
}