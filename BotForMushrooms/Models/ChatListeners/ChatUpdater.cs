using BotForMushrooms.Models.Commands;
using BotForMushrooms.Models.Commands.CommandExecutors;
using BotForMushrooms.Models.Commands.CommandExecutros;
using Telegram.Bot.Types;

namespace BotForMushrooms.Models
{
    public class ChatUpdater : ITelegramUpdateListener
    {
        public Dictionary<long, string> Users { get; } = [];

        public GlobalCommandExecutor GlobalExecutor { get; }

        public Dictionary<long, PersonalCommandExecutor> PersonalExecutors { get; } = [];

        public ChatUpdater() 
        {
            GlobalExecutor = new(this);
        }

        public async Task GetUpdate(Update update)
        {
            await GlobalExecutor.GetUpdate(update);

            long userId;
            User user;

            if(update.Message != null
                && update.Message.From != null)
            {
                userId = update.Message.From.Id;
                user = update.Message.From;

            }
            else if(update.CallbackQuery  != null) 
            {
                userId = update.CallbackQuery.From.Id;
                user = update.CallbackQuery.From;

            }
            else
            {
                return;
            }

            Users[userId] = user.Username ?? $"{userId}";
            if(!PersonalExecutors.ContainsKey(userId))
            {
                PersonalExecutors.Add(userId, new PersonalCommandExecutor(this, user));
            }
            await PersonalExecutors[userId].GetUpdate(update);
        }

        public async Task UpdatePersonalData(string command)
        {
            foreach(var personalExecutor in PersonalExecutors.Values)
            {
                await personalExecutor.OnUpdate(command);
            }
        }
    }
}
