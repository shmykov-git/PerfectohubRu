using Calls.Entities.Json;
using Calls.Model.Ats;
using Newtonsoft.Json;
using Shared.Model.Enums;
using System.Collections.Generic;

namespace PerfectohubRu.Model
{
    public class ClientData
    {
        public ClientState State { get; set; } = ClientState.New;

        public string BeelineAtsToken { get; set; }
        public string Tele2AtsToken { get; set; }
        public string Tele2AtsRefreshToken { get; set; }

        public string CriticalError { get; set; }

        public ActivePhone[] Actives { get; set; } = new ActivePhone[0];
        public string[] Knowns { get; set; } = new string[0];
        public string[] Commons { get; set; } = new string[0];
        
        public string BotToken { get; set; }
        public string BotId { get; set; }
        public BotType BotType { get; set; }
        public Dictionary<string, Chat> Chats { get; set; } = new Dictionary<string, Chat>();

        public int ScheduleIntervalMinutes { get; set; } = 30;

        public IntegrationData IntegrationData { get; set; } = new IntegrationData();

        [JsonIgnore] public HashSet<string> AllKnowns { get; set; }
        [JsonIgnore] public bool HasCriticalError => CriticalError != null;
    }
}
