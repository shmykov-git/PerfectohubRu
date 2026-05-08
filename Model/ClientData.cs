using Calls.Model.Ats;
using Newtonsoft.Json;
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


        [JsonIgnore] public HashSet<string> AllKnowns { get; set; }
        [JsonIgnore] public bool HasCriticalError => CriticalError != null;
    }
}
