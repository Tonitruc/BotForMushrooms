using Telegram.Bot;
using Telegram.Bot.Types;

namespace BotForMushrooms.Models.Commands
{
    public interface ICommand<T> where T : class
    {
        public string Name { get; }
        public Task Execute(T message, ITelegramBotClient client);

        public bool Contains(string? command)
        {
            if (command == null) return false;

            return command.Contains('/' + this.Name) && command.Contains(AppSettings.ShortName);
        }
    }
}
