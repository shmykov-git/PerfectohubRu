using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Perfecto.Deploy.Extensions;
using System;
using System.Linq;

namespace Shared.Extensions
{
    public static class OptionsExtensions
    {
        private readonly static string[] optionsPostfixes = new string[] { "Options", "Settings" };

        public static TOptions GetOptions<TOptions>(this IConfiguration configuration, string parentSectionName = null)
        {
            var sectionName = GetOptionsSectionName<TOptions>(parentSectionName);
            var section = configuration.GetSection(sectionName);

            return section.Get<TOptions>();
        }

        public static IServiceCollection ConfigurePerfectoOptions<TOptions>(this IServiceCollection services, IConfiguration configuration, string parentSectionName = null) where TOptions : class
        {
            var sectionName = GetOptionsSectionName<TOptions>(parentSectionName);
            var section = configuration.GetSection(sectionName);

            var dd = configuration.GetSection(sectionName);

            return services.Configure<TOptions>(section);
        }

        public static string GetOptionsSectionName<TOptions>(string parentSectionName = null)
        {
            var typeName = typeof(TOptions).Name;

            var postfix = optionsPostfixes.FirstOrDefault(typeName.EndsWith);

            if (postfix == null)
                throw new ApplicationException($"Options class name {typeName} should end in one of '{optionsPostfixes.SJoin(", ")}'");

            var typeSectionName = typeName.Substring(0, typeName.Length - postfix.Length);

            var sectionName = parentSectionName == null
                ? typeSectionName
                : $"{parentSectionName}:{typeSectionName}";

            return sectionName;
        }
    }
}
