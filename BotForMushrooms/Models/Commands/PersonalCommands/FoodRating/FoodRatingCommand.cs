using BotForMushrooms.Models.Commands.CommandExecutors;
using BotForMushrooms.Models.Commands.MenuCommands;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace BotForMushrooms.Models.Commands.PersonalCommands.FoodRating
{
    public class FoodRatingCommand : IActiveTelegramMenu, ICommand<Message>
    {
        public string Name => "food_rating";

        public LinkedList<IActiveMenuELement> Pages { get; } 

        public LinkedListNode<IActiveMenuELement>? CurrentPgae { get; set; }

        public string? CurrentData { get; set; }

        public Message? ListenMessage { get; set; }

        public User ListenUser { get; set; }

        public PersonalCommandExecutor Executor { get; }


        public delegate Task OnUpdateMenuDelegate(string page, ITelegramBotClient client);

        public event OnUpdateMenuDelegate UpdateMenuEvent;

        public FoodRatingCommand(PersonalCommandExecutor executor)
        {
            Executor = executor;

            Pages = new([ new FoodPlacesCommand(this) ]);
            UpdateMenuEvent += OnUpdateMenu;
            ListenUser = Executor.ListenUser;
        }

        public async Task Execute(Message message, ITelegramBotClient client)
        {
            var chatId = message.Chat.Id;

            if (ListenMessage != null)
            {
                Executor.StopListen();
                await client.DeleteMessageAsync(ListenMessage.Chat.Id, messageId: ListenMessage.MessageId);
                Executor.ActiveMenus.Remove(ListenMessage.MessageId);
            }

            ListenMessage = await client.SendTextMessageAsync(chatId, $"Loading <i>{Name}</i>", parseMode: ParseMode.Html);
            Executor.ActiveMenus[ListenMessage.MessageId] = this;
            Console.WriteLine($"Menu Id: {ListenMessage.MessageId}");

            CurrentPgae = Pages.First;

            if(CurrentPgae != null)
            {
                CurrentData = CurrentPgae.Value.CurrentData;

                await GetUpdate(new() { Data = CurrentPgae.Value.Name, From = ListenUser, Message = ListenMessage }, client);
            }
        }

        public async Task GetUpdate(CallbackQuery update, ITelegramBotClient client)
        {
            var data = update.Data;

            if (CurrentPgae.Value.Name.Equals(data))
            {
                await CurrentPgae.Value.Execute(update, client);
            }
            else if (data.StartsWith(CurrentPgae.Value.Name))
            {
                await CurrentPgae.Value.GetUpdate(update, client);
            }
        }

        public async Task OnUpdateMenu(string page, ITelegramBotClient client)
        {
            CallbackQuery callbackQuery = new CallbackQuery() { Data = page, From = ListenUser, Message = ListenMessage };

            await GetUpdate(callbackQuery, client);
        }
    }
}
