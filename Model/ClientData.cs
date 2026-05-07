using Calls.Model.Ats;

namespace PerfectohubRu.Model
{
    public class ClientData
    {
        public ClientState State { get; set; } = ClientState.New;

        public string BeelineAtsToken { get; set; }
        public string Tele2AtsToken { get; set; }
        public string Tele2AtsRefreshToken { get; set; }


        public KnownPhone[] Knowns { get; set; }
        public string[] Actives { get; set; }
    }
}
