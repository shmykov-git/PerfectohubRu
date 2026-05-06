using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MovieIntro;
using PerfectohubRu.Extensions;
using PerfectohubRu.Model;
using System;
using System.Windows;

namespace PerfectohubRu
{
    public partial class App : Application
    {
        public IServiceProvider Services { get; private set; }

        public App()
        {
            // todo: before publish, move to code
            // AppConfigCode.Apply();

            var configuration = new ConfigurationBuilder()
                .AddJsonEmbededResourceAndBuild("appsettings.json");

            var services = new ServiceCollection();
            
            // Регстрация сервисов
            services
                .Configure<AppSettings>(configuration)
                .AddSingleton(configuration);

            // Регистрация окон
            services
                .AddSingleton<MainWindow>()
                .AddTransient<AtsHelpDialog>()
                .AddSingleton<SupportChat>()
                ;

            Services = services.BuildServiceProvider();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var mainWindow = Services.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }
    }
}
