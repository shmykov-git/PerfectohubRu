using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace MovieIntro
{
    public partial class SupportChat : Window
    {
        private ObservableCollection<ChatMessage> messages;

        public SupportChat()
        {
            InitializeComponent();

            messages = new ObservableCollection<ChatMessage>();
            MessagesListBox.ItemsSource = messages;

            // Добавляем приветственное сообщение
            AddSystemMessage("Добро пожаловать в чат поддержки! Напишите ваш вопрос, и мы ответим в ближайшее время.");

            // Фокус на поле ввода при открытии
            Loaded += (s, e) => MessageTextBox.Focus();
        }

        // Позиционирование окна справа от основной формы
        public void PositionToRightOfOwner()
        {
            if (Owner != null)
            {
                Left = Owner.Left + Owner.Width + 5;
                Top = Owner.Top;
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Hide();
        }

        private async void SendButton_Click(object sender, RoutedEventArgs e)
        {
            await SendMessage();
        }

        private async void MessageTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && !Keyboard.Modifiers.HasFlag(ModifierKeys.Shift))
            {
                e.Handled = true;
                await SendMessage();
            }
        }

        private async Task SendMessage()
        {
            string message = MessageTextBox.Text.Trim();
            string email = EmailTextBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(message))
            {
                AnimateError(MessageTextBox);
                return;
            }

            if (string.IsNullOrWhiteSpace(email))
            {
                AnimateError(EmailTextBox);
                return;
            }

            // Добавляем сообщение пользователя в чат
            AddUserMessage(message, email);

            // Очищаем поле ввода
            MessageTextBox.Clear();

            // Имитация отправки на сервер
            await Task.Delay(500);

            // Добавляем ответ от поддержки (для теста)
            AddSystemMessage($"Спасибо за ваше сообщение, {email}! Наш специалист свяжется с вами в ближайшее время.");
        }

        private void AddUserMessage(string message, string email)
        {
            messages.Add(new ChatMessage
            {
                Sender = email,
                Message = message,
                Time = DateTime.Now.ToString("HH:mm"),
                IsUserMessage = true
            });

            ScrollToBottom();
        }

        private void AddSystemMessage(string message)
        {
            messages.Add(new ChatMessage
            {
                Sender = "ПОДДЕРЖКА",
                Message = message,
                Time = DateTime.Now.ToString("HH:mm"),
                IsUserMessage = false
            });

            ScrollToBottom();
        }

        private void ScrollToBottom()
        {
            if (MessagesListBox.Items.Count > 0)
            {
                MessagesListBox.ScrollIntoView(MessagesListBox.Items[MessagesListBox.Items.Count - 1]);
            }
        }

        private void AnimateError(TextBox textBox)
        {
            var shakeAnimation = new DoubleAnimationUsingKeyFrames();
            shakeAnimation.KeyFrames.Add(new EasingDoubleKeyFrame(-5, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(0))));
            shakeAnimation.KeyFrames.Add(new EasingDoubleKeyFrame(5, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(50))));
            shakeAnimation.KeyFrames.Add(new EasingDoubleKeyFrame(-5, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(100))));
            shakeAnimation.KeyFrames.Add(new EasingDoubleKeyFrame(5, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(150))));
            shakeAnimation.KeyFrames.Add(new EasingDoubleKeyFrame(0, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(200))));

            var translateTransform = new TranslateTransform();
            textBox.RenderTransform = translateTransform;
            translateTransform.BeginAnimation(TranslateTransform.XProperty, shakeAnimation);

            // Визуальное выделение
            var border = (Border)textBox.Parent;
            if (border != null)
            {
                var originalBrush = border.BorderBrush;
                border.BorderBrush = System.Windows.Media.Brushes.Red;
                Task.Delay(500).ContinueWith(_ =>
                {
                    Dispatcher.Invoke(() => border.BorderBrush = originalBrush);
                });
            }
        }
    }

    // Класс сообщения чата
    public class ChatMessage : INotifyPropertyChanged
    {
        private string _sender;
        private string _message;
        private string _time;
        private bool _isUserMessage;

        public string Sender
        {
            get => _sender;
            set { _sender = value; OnPropertyChanged(); }
        }

        public string Message
        {
            get => _message;
            set { _message = value; OnPropertyChanged(); }
        }

        public string Time
        {
            get => _time;
            set { _time = value; OnPropertyChanged(); }
        }

        public bool IsUserMessage
        {
            get => _isUserMessage;
            set { _isUserMessage = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}