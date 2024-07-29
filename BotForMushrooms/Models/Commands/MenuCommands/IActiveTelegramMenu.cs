using BotForMushrooms.Models.ChatListeners;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace BotForMushrooms.Models.Commands.MenuCommands
{
    public interface IActiveTelegramMenu : IPersonalListener, ICommand<Message>
    {
        LinkedList<IActiveMenuELement>? Pages { get; }
        LinkedListNode<IActiveMenuELement>? CurrentPgae { get; set; }
    }
}
