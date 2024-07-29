using Telegram.Bot;

namespace BotForMushrooms.Models.ChatListeners
{
    public interface IListener<T, K> where T : class
        where K : class
    {
        K Executor { get; }
        Task GetUpdate(T update, ITelegramBotClient client);
    }
}
