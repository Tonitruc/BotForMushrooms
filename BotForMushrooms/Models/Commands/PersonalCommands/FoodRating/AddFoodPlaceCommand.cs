using BotForMushrooms.Models.Commands.CommandExecutors;
using BotForMushrooms.Models.Commands.MenuCommands;
using System.Text;
using System.Text.Json;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace BotForMushrooms.Models.Commands.PersonalCommands.FoodRating
{
    public class AddFoodPlaceCommand : IActiveMenuCommand
    {
        public string Name => "add_food_place";

        public PersonalCommandExecutor Executor { get; }

        public IActiveMenuELement ParentMenuElement { get; }

        public AddFoodPlaceCommand(PersonalCommandExecutor executor, IActiveMenuELement parentMenuElement)
        {
            Executor = executor;
            ParentMenuElement = parentMenuElement;
        }

        public async Task Execute(CallbackQuery callbackQuery, ITelegramBotClient client)
        {
            Executor.StartListen(this);

            string? username = Executor.ListenUser.Username;

            string resText = $"<i>Operation: [{Name}]</i>\n\n"
                        + $"@{username}, введите название места пропитания: ";

            await client.SendTextMessageAsync(callbackQuery.Message.Chat.Id, resText, parseMode: ParseMode.Html);
        }

        public async Task GetUpdate(Message update, ITelegramBotClient client)
        {
            using HttpClient httpClient = new HttpClient();

            FoodPlace newFoodPlace = new FoodPlace(update.Text);

            var json = JsonSerializer.Serialize(newFoodPlace);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync($"{AppSettings.Url}/api/FoodPlace", content);

            Executor.StopListen();
            if (response.IsSuccessStatusCode)
            {
                await client.SendTextMessageAsync(update.Chat.Id, "Место пропитания успешно добавлено", parseMode: ParseMode.Html);
                await ParentMenuElement.OnUpdateMenuElement(client);
            }
            else
            {
                await client.SendTextMessageAsync(update.Chat.Id, $"Место пропитания с именем <i>{update.Text}</i> уже занято.", parseMode: ParseMode.Html);
            }

        }
    }
}
