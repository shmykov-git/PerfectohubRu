using Calls.HttpClients;
using Calls.HttpClients.Abstractions;
using Calls.HttpClients.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using PerfectohubRu.Tools;
using Shared.Extensions;
using Shared.HttpClients.Options.Base;
using Shared.Libraries;
using Shared.Model.Enums;
using System;
using System.Net.Http;

namespace PerfectohubRu.Extensions
{
    public static class HttpClientExtensions
    {
        public static IServiceCollection AddPerfectoHttpClients(this IServiceCollection services, IConfiguration configuration)
        {
            return services
                .ConfigureHttpClient<IIntegrationHttpClient, IntegrationHttpClient, IntegrationApiOptions>(configuration)
                .ConfigureHttpClient<IBotHttpClient, BotHttpClient, BotsApiOptions>(configuration)
                .ConfigureHttpClient<IBeelineAtsHttpClient, BeelineAtsHttpClient, BeelineAtsApiOptions>(configuration)
                .ConfigureHttpClient<ITele2AtsHttpClient, Tele2AtsHttpClient, Tele2AtsApiOptions>(configuration)
                .AddSingleton<IAtsHttpClient>(sp =>
                {
                    var data = sp.GetRequiredService<ClientDataProvider>().Data;

                    switch (data.GetAtsType()) 
                    {
                        case AtsType.Beeline:
                            return sp.GetRequiredService<IBeelineAtsHttpClient>();
                        case AtsType.Tele2:
                            return sp.GetRequiredService<ITele2AtsHttpClient>();
                        default:
                            throw new NotSupportedException();
                    }
                })
                ;
        }

        public static IServiceCollection ConfigureHttpClient<TClientKey, TClient, TClientOptions>(this IServiceCollection ss, IConfiguration configuration)
            where TClientOptions : HttpClientOptions
            where TClientKey : class
            where TClient : class, TClientKey
        {
            ss
                .ConfigurePerfectoOptions<TClientOptions>(configuration, Values.ConfigurationSection.HttpClients)
                .AddHttpClient<TClient>((services, client) =>
                {
                    var options = services.GetRequiredService<IOptions<TClientOptions>>().Value;

                    if (options.Host != null)
                        client.BaseAddress = new Uri(options.Host);

                    client.Timeout = TimeSpan.FromSeconds(options.ClientTimeout ?? 10);
                })
                .ConfigurePrimaryHttpMessageHandler(services =>
                {
                    var handler = new HttpClientHandler();

                    var options = services.GetRequiredService<IOptions<TClientOptions>>().Value;

                    if (options.ServerCertificateCustomValidationCallbackReturnTrue ?? false)
                        handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;

                    return handler;
                });

            ss.AddScoped<TClientKey, TClient>(sp => sp.GetRequiredService<TClient>());

            return ss;
        }
    }
}
