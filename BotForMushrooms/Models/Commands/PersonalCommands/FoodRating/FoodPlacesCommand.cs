using BotForMushrooms.Models.Commands.MenuCommands;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace BotForMushrooms.Models.Commands.PersonalCommands.FoodRating
{
    public class FoodPlacesCommand : IActiveMenuELement
    {
        public string Name => "food_places_list";

        public IActiveTelegramMenu Executor { get; }

        public Dictionary<string, ICommand<CallbackQuery>> ActiveButton { get; }
        public string? CurrentData { get; set; } 

        public FoodPlacesCommand(IActiveTelegramMenu executor)
        {
            Executor = executor;

            ActiveButton = new() {
                { "add", new AddFoodPlaceCommand(Executor.Executor, this) },
                { "delete", new DeleteFoodPlaceCommand(Executor.Executor, this) }
            };

            CurrentData = Name;
        }

        public async Task Execute(CallbackQuery message, ITelegramBotClient client)
        {
            using HttpClient httpClient = new HttpClient();

            var response = await httpClient.GetAsync($"{AppSettings.Url}/api/FoodPlace");
            response.EnsureSuccessStatusCode();
            var jsonString = await response.Content.ReadAsStringAsync();

            List<FoodPlace> foodPlaces = JsonConvert.DeserializeObject<List<FoodPlace>>(jsonString) ?? [];

            string resMessage = "Список мест пропитания: ";
            if (foodPlaces.IsNullOrEmpty())
            {
                resMessage = "Список пуст, добавьте место пропитания:";
            }

            var buttons = foodPlaces.Select(fp => new[] { InlineKeyboardButton.WithCallbackData($"{fp.Name}\t\t\t{fp.Rating} ⭐", $"{Name}:{fp.Id}") }).ToList();
            buttons.Add([InlineKeyboardButton.WithCallbackData("\U0001f7e2 Добавить", $"{Name}:add"), InlineKeyboardButton.WithCallbackData("🔴 Удалить", $"{Name}:delete")]);

            resMessage = $"For user @{Executor.ListenUser.Username}\n\n" + resMessage;
            await client.EditMessageTextAsync(
                chatId: message.Message.Chat.Id,
                messageId: message.Message.MessageId,
                text: resMessage,
                parseMode: ParseMode.Html,
                replyMarkup: new InlineKeyboardMarkup(buttons)
            );
        }

        public async Task GetUpdate(CallbackQuery update, ITelegramBotClient client)
        {
            var data = update.Data;

            var dataParts = data.Split(":");
            string command = dataParts[1];

            await ActiveButton[command].Execute(update, client);
        }

        public async Task OnUpdateMenuElement(ITelegramBotClient client)
        {
            await Executor.Executor.ChatUpdater.UpdatePersonalData(CurrentData);
        }
    }
}
