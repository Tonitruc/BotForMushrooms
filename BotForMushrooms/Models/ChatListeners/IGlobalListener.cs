using BotForMushrooms.Models.Commands.CommandExecutros;
using Telegram.Bot.Types;

namespace BotForMushrooms.Models.ChatListeners
{
    public interface IGlobalListener : IListener<CallbackQuery, GlobalCommandExecutor>
    {
        long? MessageId { get; set; }
    }
}
