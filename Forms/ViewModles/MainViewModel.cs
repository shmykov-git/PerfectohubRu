using Calls.HttpClients.Abstractions;
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
        private readonly IBeelineAtsHttpClient beelineAtsHttpClient;
        private readonly ITele2AtsHttpClient tele2AtsHttpClient;
        private readonly ClientDataProvider clientDataProvider;
        private ClientData clientData;

        public MainViewModel(
            IBeelineAtsHttpClient beelineAtsHttpClient, 
            ITele2AtsHttpClient tele2AtsHttpClient,
            ClientDataProvider clientDataProvider
            )
        {
            this.beelineAtsHttpClient = beelineAtsHttpClient;
            this.tele2AtsHttpClient = tele2AtsHttpClient;
            this.clientDataProvider = clientDataProvider;
            clientData = clientDataProvider.State;
            AtsToken = clientData.GetAtsToken();
        }

        public ClientData ClientData => clientData;

        private AtsType GetTokenAtsType(string token)
        {
            var dotsCount = token.Length - token.Replace(".", "").Length;
            //var munisesCount = token.Length - token.Replace("-", "").Length;

            if (dotsCount >= 3)
                return AtsType.Tele2;

            return AtsType.Beeline;
        }

        public IAtsHttpClient GetAtsHttpClient()
        {
            switch (clientData.GetAtsType())
            {
                case AtsType.Tele2:
                    return tele2AtsHttpClient;
                case AtsType.Beeline:
                    return beelineAtsHttpClient;
                default:
                    throw new ArgumentException("Unknonw http client");
            }
        }

        public async Task<OperationResult> ValidateAndSaveAtsToken()
        {
            var token = AtsToken;
            var atsType = GetTokenAtsType(token);

            switch (atsType) 
            {
                case AtsType.Beeline:
                    clientData.BeelineAtsToken = token;
                    break;

                case AtsType.Tele2:
                    clientData.Tele2AtsToken = token;
                    break;

                default:
                    return new OperationResult { Error = "Не удалось определить тип токена" };
            }

            clientDataProvider.SaveClientState();

            var httpClient = GetAtsHttpClient();

            try
            {
                var knowns = await httpClient.GetAbonents();
                clientData.Knowns = knowns;

                if (knowns.Length == 0)
                    return new OperationResult { Error = $"Требуется настроить список абонентов в АТС" };

                clientData.State = ClientState.HasAts;
                clientDataProvider.SaveClientState();

                return OperationResult.Successfull();
            }
            catch (HttpClientException)
            {
                return new OperationResult { Error = $"Не удается установить подключение к {clientData.GetAtsName()}" };
            }
        }
    }
}
