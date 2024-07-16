using Telegram.Bot.Types;

namespace BotForMushrooms.Models.Commands
{
    public interface ITelegramUpdateListener
    {
        Task GetUpdate(Update update);
    }
}
