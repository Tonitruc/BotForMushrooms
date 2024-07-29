using BotForMushrooms.Models.ChatListeners;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace BotForMushrooms.Models.Commands.MenuCommands
{
    public interface IActiveMenuELement : ICommand<CallbackQuery>, IListener<CallbackQuery, IActiveTelegramMenu>
    {
        Task OnUpdateMenuElement(ITelegramBotClient client);
        string? CurrentData { get; set; }
    }
}
