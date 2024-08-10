using Telegram.Bot.Types;
using Telegram.Bot;

namespace BotForMushrooms.Models.Commands
{
    public class UpdateDistributor
    {
        private readonly Dictionary<long, ChatUpdater> listeners = [];

        public UpdateDistributor() { }

        public async Task GetUpdate(Update update)
        {
            long chatId;
            string title = string.Empty;
            Chat? chat = null;

            if (update.Message != null)
            {
                chat = update.Message.Chat; 
            }
            else if(update.CallbackQuery != null) 
            {
                chat = update.CallbackQuery.Message.Chat;

            }
            else if(update.PollAnswer != null)
            {
                string pollId = update.PollAnswer.PollId;
                foreach(var item in listeners)
                {
                    if(item.Value.IsPollContains(pollId))
                    {
                        chat = item.Value.From;
                        break;
                    }
                }

                if(chat == null)
                {
                    return;
                }
            }
            else
            {
                return;
            }

            chatId = chat.Id;
            title = chat.Title;

            ChatUpdater? listener = listeners.GetValueOrDefault(chatId);
            listener ??= new ChatUpdater(chat);
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
