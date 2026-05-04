using Microsoft.Extensions.Configuration;
using Perfecto.Deploy.Extensions;
using System;
using System.IO;
using System.Reflection;

namespace PerfectohubRu.Extensions
{
    internal static class EmbededResourceExtenstions
    {
        public static IConfigurationRoot AddJsonEmbededResourceAndBuild(this IConfigurationBuilder builder, string resourceName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var embName = $"PerfectohubRu.{resourceName}";

            using (Stream stream = assembly.GetManifestResourceStream(embName))
            {
                if (stream == null)
                {
                    throw new Exception($"Embedded resource '{embName}' not found. " +
                                        $"Available resources: {assembly.GetManifestResourceNames().SJoin(", ")}");
                }

                return builder
                    .AddJsonStream(stream)
                    .Build();
            }
        }
    }
}
