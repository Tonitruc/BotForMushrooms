using BotForMushrooms.Models.ChatListeners;
using Telegram.Bot.Types;

namespace BotForMushrooms.Models.Commands.MenuCommands
{
    public interface ITelegramMenu : IGlobalListener, ICommand<Message>
    {
    }
}
