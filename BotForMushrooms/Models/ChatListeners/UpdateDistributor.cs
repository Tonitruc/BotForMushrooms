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
            long chatId;
            string title = string.Empty;

            if (update.Message != null)
            {
                chatId = update.Message.Chat.Id;
                title = update.Message.Chat.Title;
            }
            else if(update.CallbackQuery != null) 
            {
                chatId = update.CallbackQuery.Message.Chat.Id;
                title = update.CallbackQuery.Message.Chat.Title;
            }
            else
            {
                return;
            }

            T? listener = listeners.GetValueOrDefault(chatId);
            listener ??= new T();
            listeners[chatId] = listener;

            Console.WriteLine($"Chat: {title}");

            if (title != "Тест бота")
            {
                return;
            }

            await listener.GetUpdate(update);
        }
    }
}
