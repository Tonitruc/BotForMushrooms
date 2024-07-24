using Telegram.Bot;
using Telegram.Bot.Types;

namespace BotForMushrooms.Models.Commands
{
    public abstract class Command
    {
        public abstract string Name { get; }
        public abstract Task Execute(Message message, ITelegramBotClient client);

        public bool Contains(string? command)
        {
            if (command == null) return false;

            return command.Contains('/' + this.Name) && command.Contains(AppSettings.ShortName);
        }
    }
}
