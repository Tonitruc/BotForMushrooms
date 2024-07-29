using BotForMushrooms.Models.Commands.CommandExecutors;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace BotForMushrooms.Models.ChatListeners
{
    public interface IPersonalListener : IListener<CallbackQuery, PersonalCommandExecutor> 
    {
        Message? ListenMessage { get; }
        User? ListenUser { get; } 
        string? CurrentData { get; }
        Task OnUpdateMenu(string data, ITelegramBotClient client);
    }
}
