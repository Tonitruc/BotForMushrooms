using BotForMushrooms.Models.ChatListeners;
using BotForMushrooms.Models.Commands.CommandExecutors;
using Telegram.Bot.Types;

namespace BotForMushrooms.Models.Commands.MenuCommands
{
    public interface IActiveMenuCommand : ICommand<CallbackQuery>, IListener<Message, PersonalCommandExecutor>
    {
        IActiveMenuELement ParentMenuElement { get; }   
    }
}
