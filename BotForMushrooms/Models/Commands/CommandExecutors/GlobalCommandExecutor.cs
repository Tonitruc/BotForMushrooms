using BotForMushrooms.Models.Commands.MenuCommands;
using Telegram.Bot.Types;

namespace BotForMushrooms.Models.Commands.CommandExecutros
{
    public class GlobalCommandExecutor : CommandExecutor<ITelegramMenu>
    {
        public override Dictionary<long, ITelegramMenu> ActiveMenus { get; }

        protected override List<ICommand<Message>> CommandList { get; }
        public override ChatUpdater ChatUpdater { get; }

        public GlobalCommandExecutor(ChatUpdater chatUpdater)
        {
            CommandList = [
                new MustardBoyCommand(),
            ];

            ActiveMenus = [];
            ChatUpdater = chatUpdater;
        }

        public override async Task GetUpdate(Update update)
        {
            if (update.Message != null)
            {
                Message message = update.Message;
                ICommand<Message>? command = CommandList.FirstOrDefault(command => command.Contains(message.Text));
                if (command != null)
                {
                    await command.Execute(message, client);
                }
            }
            else if(update.CallbackQuery != null)
            {
                CallbackQuery callbackQuery = update.CallbackQuery;

            }
        }
    }
}
