using System.Windows.Input;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace BotForMushrooms.Models.Commands
{
    public abstract class CommandExecutor<T> : ITelegramUpdateListener where T : class
    {
        protected readonly TelegramBotClient client = Bot.Get().Result;

        protected abstract List<ICommand<Message>> CommandList { get; }

        public abstract Dictionary<long, T> ActiveMenus { get; }

        public abstract Task GetUpdate(Update update);

        public abstract ChatUpdater ChatUpdater { get; }
    }
}
