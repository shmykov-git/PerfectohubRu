using Calls.HttpClients.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Perfecto.Deploy.Extensions;
using PerfectohubRu.Extensions;
using PerfectohubRu.Model;
using PerfectohubRu.Tools;
using Shared.Exceptions;
using Shared.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerfectohubRu.Forms.ViewModles
{
    public partial class MainViewModel
    {
        private readonly IAtsHttpClient httpClient;
        private readonly CallsManager callsManager;
        private readonly ClientDataProvider dataProvider;
        private readonly IServiceProvider sp;
        private ClientData data;

        public MainViewModel(
            CallsManager callsManager,
            ClientDataProvider clientDataProvider,
            IServiceProvider sp)
        {
            this.callsManager = callsManager;
            this.dataProvider = clientDataProvider;
            this.sp = sp;
            data = clientDataProvider.Data;
            AtsToken = data.GetAtsToken();
            Knowns = data.Knowns.SJoin("\r\n");
            Commons = data.Commons.SJoin("\r\n");
        }

        public ClientData ClientData => data;
        public IServiceProvider Sp => sp;

        private AtsType GetTokenAtsType(string token)
        {
            var dotsCount = token.Length - token.Replace(".", "").Length;
            //var munisesCount = token.Length - token.Replace("-", "").Length;

            if (dotsCount >= 3)
                return AtsType.Tele2;

            return AtsType.Beeline;
        }

        public async void RefreshCallsMessage()
        {
            await Task.Run(async () =>
            {
                try
                {
                    CallsMessage = (await GetCallsMessage()).First();
                }
                catch (Exception ex)
                {
                    data.CriticalError = ex.Message;
                    dataProvider.Save();
                }
            });
        }

        private Task<string[]> GetCallsMessage() => callsManager.GetUniqueCallsMessage(false, false, false, default);

        public void SaveKnowns()
        {
            var phones = Knowns.Replace("\r", "").Split('\n').Select(v => v.Trim()).ToArray();
            ClientData.Knowns = phones;
            dataProvider.Save();
            RefreshCallsMessage();
        }

        public void SaveCommons()
        {
            var phones = Commons.Replace("\r", "").Split('\n').Select(v => v.Trim()).ToArray();
            ClientData.Commons = phones;
            dataProvider.Save();
            RefreshCallsMessage();
        }

        public async Task<OperationResult> ValidateAndSaveAtsToken()
        {
            var token = AtsToken;
            var atsType = GetTokenAtsType(token);

            switch (atsType) 
            {
                case AtsType.Beeline:
                    data.BeelineAtsToken = token;
                    break;

                case AtsType.Tele2:
                    data.Tele2AtsToken = token;
                    break;

                default:
                    return new OperationResult { Error = "Не удалось определить тип токена" };
            }

            dataProvider.Save();

            var httpClient = sp.GetRequiredService<IAtsHttpClient>();

            try
            {
                var actives = await httpClient.GetAbonents();
                data.Actives = actives;

                if (actives.Length == 0)
                    return new OperationResult { Error = $"Требуется настроить список абонентов в АТС" };

                data.State = ClientState.HasAts;
                dataProvider.Save();

                return OperationResult.Successfull();
            }
            catch (HttpClientException)
            {
                return new OperationResult { Error = $"Не удается установить подключение к {data.GetAtsName()}" };
            }
        }
    }
}
