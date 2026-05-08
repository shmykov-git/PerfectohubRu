using PerfectohubRu.Tools;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace MovieIntro
{
    public partial class BotHelpDialog : Window
    {
        public BotHelpDialog()
        {
            InitializeComponent();
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Max_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            UrlHelper.OpenUrl("https://docs.google.com/presentation/d/1ySZdy_DxLJusWZKPx1YWOth2Y4kQqFVyOicioSBHJeU/edit?usp=drive_link");
        }

        private void Telegram_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            UrlHelper.OpenUrl("https://docs.google.com/presentation/d/1o_d4yehUSNha3KgGbcfRYUP7lhzd8bjBd6_S_zObWQM/edit?usp=drive_link");
        }
    }
}