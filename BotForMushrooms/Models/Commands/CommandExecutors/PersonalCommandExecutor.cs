using BotForMushrooms.Models.ChatListeners;
using BotForMushrooms.Models.Commands.PersonalCommands.FoodRating;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace BotForMushrooms.Models.Commands.CommandExecutors
{
    public class PersonalCommandExecutor : CommandExecutor<IPersonalListener>
    {
        public User ListenUser { get; set; }

        protected override List<ICommand<Message>> CommandList { get; }

        public override Dictionary<long, IPersonalListener> ActiveMenus { get; }

        private IListener<Message, PersonalCommandExecutor>? Listener { get; set; } = null;

        public override ChatUpdater ChatUpdater { get; }


        public PersonalCommandExecutor(ChatUpdater charUpdater, User user)
        {
            ListenUser = user;
            ChatUpdater = charUpdater;

            CommandList = [
                new FoodRatingCommand(this)
            ];

            ActiveMenus = [];
        }

        public override async Task GetUpdate(Update update)
        {
            if (update.Message != null)
            {
                Message message = update.Message;

                if (Listener != null)
                {
                    await Listener.GetUpdate(message, client);
                }
                else
                {
                    ICommand<Message>? command = CommandList.FirstOrDefault(command => command.Contains(message.Text));
                    if (command != null)
                    {
                        await command.Execute(message, client);
                    }
                }
            }
            else if (update.CallbackQuery != null)
            {
                CallbackQuery callbackQuery = update.CallbackQuery;
                string data = callbackQuery.Data;
                Console.WriteLine($"Get Update{callbackQuery.Message.MessageId}");

                if (ActiveMenus.ContainsKey(callbackQuery.Message.MessageId))
                {
                    await ActiveMenus[callbackQuery.Message.MessageId].GetUpdate(callbackQuery, client);
                }
            }
        }

        public void StartListen(IListener<Message, PersonalCommandExecutor>? mewListener)
        {
            Listener = mewListener;
        }

        public void StopListen()
        {
            Listener = null; 
        }

        public async Task OnUpdate(string data)
        {
            foreach(var menu in ActiveMenus.Values)
            {
                if(menu.CurrentData.Equals(data))
                {
                    await menu.OnUpdateMenu(data, client);
                }
            }
        }
    }
}
