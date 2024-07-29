using BotForMushrooms.Models.Commands.CommandExecutors;
using BotForMushrooms.Models.Commands.MenuCommands;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace BotForMushrooms.Models.Commands.PersonalCommands.FoodRating
{
    public class DeleteFoodPlaceCommand : IActiveMenuCommand
    {
        public string Name => "delete_food_place";

        public PersonalCommandExecutor Executor { get; }

        public IActiveMenuELement ParentMenuElement { get; }

        public DeleteFoodPlaceCommand(PersonalCommandExecutor executor, IActiveMenuELement parentMenuElement)
        {
            Executor = executor;
            ParentMenuElement = parentMenuElement;
        }

        public async Task Execute(CallbackQuery message, ITelegramBotClient client)
        {
            Executor.StartListen(this);

            string username = message.From.Username;

            string resText = $"<i>Operation: [{Name}]</i>\n\n"
                        + $"@{username}, введите название места пропитания: ";

            await client.SendTextMessageAsync(message.Message.Chat.Id, resText, parseMode: ParseMode.Html);
        }

        public async Task GetUpdate(Message update, ITelegramBotClient client)
        {
            using HttpClient httpClient = new HttpClient();

            var response = await httpClient.DeleteAsync($"{AppSettings.Url}/api/FoodPlace/name/{update.Text}");

            Executor.StopListen();
            if (response.IsSuccessStatusCode)
            {
                await client.SendTextMessageAsync(update.Chat.Id, "Место пропитания успешно удалено", parseMode: ParseMode.Html);
                await ParentMenuElement.OnUpdateMenuElement(client);
            }
            else
            {
                await client.SendTextMessageAsync(update.Chat.Id, $"Место пропитания с именем <i>{update.Text}</i> не существует.", parseMode: ParseMode.Html);
            }

        }
    }
}
