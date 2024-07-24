using Telegram.Bot.Types;
using Telegram.Bot;

namespace BotForMushrooms.Models.Commands
{
    public class UpdateDistributor<T> where T : ITelegramUpdateListener, new()
    {
        private readonly Dictionary<long, T> listeners = [];

        public UpdateDistributor() { }

        public async Task GetUpdate(Update update)
        {
            if (update.Message == null)
                return;

            long chatId = update.Message.Chat.Id;
            T? listener = listeners.GetValueOrDefault(chatId);
            listener ??= new T();
            listeners[chatId] = listener;

            await listener.GetUpdate(update);
        }
    }
}
