using System;

namespace Calls.HttpClients
{
    public class MessageOut
    {
        public string ChatId { get; set; }
        public bool IsGroupChat { get; set; }
        public string UserName { get; set; }
        public string ChatTitle { get; set; }
        public string Text { get; set; }
        public DateTime? Time { get; set; }
    }
}
