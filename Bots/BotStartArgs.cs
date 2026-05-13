namespace Calls.Api.Bots
{
    public class BotStartArgs
    {
        public string[] CommandArgs { get; set; }
        public string ChatId { get; set; }
        public bool IsGroupChat { get; set; }
        public string UserName { get; set; }
        public string ChatTitle { get; set; }
    }
}
