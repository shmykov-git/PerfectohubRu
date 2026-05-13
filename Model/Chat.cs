using Newtonsoft.Json;
using Shared.Model.Enums;

namespace Calls.Entities.Json
{
    public class Chat
    {
        public string ChatId { get; set; }
        public bool IsGroup { get; set; }
        public BotType BotType { get; set; }
        public string UserName { get; set; }
        public string Title { get; set; }

        [JsonIgnore]
        public string Sender => UserName ?? Title;
    }
}
