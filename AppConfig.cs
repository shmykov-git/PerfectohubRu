using System;

namespace PerfectohubRu
{
    static class AppConfig
    {
        public static void Apply()
        {
            // Получаем информацию о текущем домене приложения
            var setup = AppDomain.CurrentDomain.SetupInformation;

            // Указываем целевую версию framework
            // Этот параметр аналогичен supportedRuntime в .config
            setup.TargetFrameworkName = ".NETFramework,Version=v4.8";

            // Дополнительно: принудительно загружаем версию 4.8, если доступна
            // Это полезно, если на машине установлена более новая версия
            setup.DisallowBindingRedirects = false;
        }
    }
}
