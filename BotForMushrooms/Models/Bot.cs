using Telegram.Bot;
using BotForMushrooms.Models.Commands;

namespace BotForMushrooms.Models
{
    public static class Bot
    {
        private static TelegramBotClient? client;

        public static List<Command> Commands { get; set; } = [];

        //public static IReadOnlyList<Command> Commands { get => commandsList; }

        public async static Task<TelegramBotClient> Get()
        {
            if(client != null)
            {
                return client;
            }

            client = new TelegramBotClient(AppSettings.Token);
            await client.SetWebhookAsync(AppSettings.Url);
            Commands.Add(new MustardBoyCommand());
            return client;
        }
    }
}
