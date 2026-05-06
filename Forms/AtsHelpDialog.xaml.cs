using PerfectohubRu.Tools;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace MovieIntro
{
    public partial class AtsHelpDialog : Window
    {
        public AtsHelpDialog()
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

        private void BeelineLink_Click(object sender, MouseButtonEventArgs e)
        {
            // Замените URL на актуальную ссылку на облачную АТС Билайн
            string url = "https://moskva.beeline.ru/business/telephony/cloud-ats/";
            UrlHelper.OpenUrl(url);
        }

        private void Tele2Link_Click(object sender, MouseButtonEventArgs e)
        {
            // Замените URL на актуальную ссылку на облачную АТС Tele2
            string url = "https://msk.t2.ru/";
            UrlHelper.OpenUrl(url);
        }
    }
}