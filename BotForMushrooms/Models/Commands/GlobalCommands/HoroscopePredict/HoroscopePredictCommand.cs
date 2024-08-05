using BotForMushrooms.Models.Commands.CommandExecutros;
using BotForMushrooms.Models.Commands.MenuCommands;
using BotForMushrooms.Models.Commands.TranslatorEntity;
using Newtonsoft.Json.Linq;

using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace BotForMushrooms.Models.Commands.GlobalCommands.HoroscopePredict
{
    public class HoroscopePredictCommand : ITelegramMenu
    {
        public string Name => "horoscope_perdict";

        public string ApiUrl => "https://horoscope-app-api.vercel.app/api/v1/get-horoscope";

        public ZodiacNameEnum ZodiacName { get; set; }

        public static string[] ZodiacStickers = {
            "U+1F40F", "U+1F404", "U+1F46D", "U+1F99E", "U+1F981", "U+1F483",
            "U+2696", "U+1F982", "U+1F3F9", "U+1F410", "U+1F4A6", "U+1F41F"
        };

        public Dictionary<ZodiacNameEnum, string> RussianZodiacName { get; } = [];

        public GlobalCommandExecutor Executor { get; }

        public long? MessageId { get; set; } = null;

        public HoroscopePredictCommand(GlobalCommandExecutor executor)
        {
            Executor = executor;

            string? zodiacNames = string.Empty;

            foreach (ZodiacNameEnum zodiacName in Enum.GetValues(typeof(ZodiacNameEnum)))
            {
                zodiacNames += zodiacName.ToString() + " ";
            }

            zodiacNames = Translator.TranslateText(zodiacNames).Result;

            if(zodiacNames != null)
            {
                string[] russianZodiacNames = zodiacNames.Split(' ');
                for (int i = 0; i < ZodiacStickers.Length ; i++)
                {
                    RussianZodiacName.Add((ZodiacNameEnum)i, russianZodiacNames[i]);
                }
            }
        }

        public async Task Execute(Message message, ITelegramBotClient client)
        {
            List<InlineKeyboardButton[]> buttons = [];

            foreach (ZodiacNameEnum zodiacName in Enum.GetValues(typeof(ZodiacNameEnum)))
            {
                int utf32 = Convert.ToInt32(ZodiacStickers[(int)zodiacName].Replace("U+", ""), 16);
                string character = char.ConvertFromUtf32(utf32);
                buttons.Add([InlineKeyboardButton.WithCallbackData($"{RussianZodiacName[zodiacName]} {character}", $"{Name}:{zodiacName}") ]);
            }

            var chatId = message.Chat.Id;
            if (MessageId != null)
            {
                await client.DeleteMessageAsync(chatId, (int)MessageId.Value);
            }


            string test = ZodiacStickers[0];

            Message newMessage = await client.SendTextMessageAsync(chatId, $"Выберите знак зодиака: ", replyMarkup: new InlineKeyboardMarkup(buttons));
            MessageId = newMessage.MessageId;

            Executor.ActiveMenus.Add(MessageId.Value, this);
        }

        public async Task GetUpdate(CallbackQuery update, ITelegramBotClient client)
        {
            long chatId = update.Message.Chat.Id;
            string? data = update.Data;
            string? username = update.From.Username;

            if(data == null)
            {
                return;
            }

            string[] dataParts = data.Split(':');
            ZodiacName = (ZodiacNameEnum)Enum.Parse(typeof(ZodiacNameEnum), dataParts[1], true);

            HoroscopeResponse? horoscopeResponse = await GetPredicrion();

            string? resMessage = "Ваше будущее очень расплывчато";
            if (horoscopeResponse != null)
            {
                resMessage = await Translator.TranslateText(horoscopeResponse.HoroscopeData);
            }

            int utf32 = Convert.ToInt32(ZodiacStickers[(int)ZodiacName].Replace("U+", ""), 16);
            string character = char.ConvertFromUtf32(utf32);
            resMessage = $"@{username}\t\tЗнак зодиака: {ZodiacName} {character}\n\n" + resMessage;
            await client.SendTextMessageAsync(chatId, resMessage);
        }

        private async Task<HoroscopeResponse?> GetPredicrion()
        {
            string url = $"{ApiUrl}/daily?sign={ZodiacName}&day=TODAY";

            HttpClient client = new HttpClient();

            HttpResponseMessage response = await client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();

                JObject json = JObject.Parse(responseBody);

                string horoscopeData = json["data"]["horoscope_data"].ToString();

                return new HoroscopeResponse(horoscopeData);
            }
            else
            {
                return null;
            }
        }
    }
}
