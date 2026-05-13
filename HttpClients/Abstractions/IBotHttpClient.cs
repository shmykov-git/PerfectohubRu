using Shared.Model.Enums;
using System.Threading.Tasks;

namespace Calls.HttpClients
{
    public interface IBotHttpClient
    {
        Task<RegisterResult> Register(BotType type, string botToken, string oldBotId);
        Task<MessageOut[]> GetOutMessages(string botId);
        Task<RegisterResult> SendMessage(string botId, string chatId, MessageIn message);
    }
}
