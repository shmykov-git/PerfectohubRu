using Mapster;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MovieIntro;
using PerfectohubRu.Extensions;
using PerfectohubRu.Forms.ViewModles;
using PerfectohubRu.Model;
using PerfectohubRu.Tools;
using Shared.Extensions;
using System;
using System.Reflection;
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
                .AddSingleton<IConfiguration>(configuration)
                .AddSingleton<ClientDataProvider>()
                .AddCallsHttpClients(configuration)
                .AddSharedMapster(Assembly.GetExecutingAssembly())
                ;

            // Регистрация окон
            services
                .AddSingleton<MainWindow>()
                .AddTransient<AtsHelpDialog>()
                .AddSingleton<SupportChat>()
                ;

            // Регистрация моделей окон
            services
                .AddSingleton<MainViewModel>()
                ;

            Services = services.BuildServiceProvider();

            Services
                .UseMapster(Assembly.GetExecutingAssembly())
                ;
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var mainWindow = Services.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }
    }
}
