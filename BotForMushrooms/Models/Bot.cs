using Telegram.Bot;
using BotForMushrooms.Models.Commands;

namespace BotForMushrooms.Models
{
    public static class Bot
    {
        private static TelegramBotClient? client;

        public static List<Command> commandsList { get; } = [];

        public static IReadOnlyList<Command> Commands => commandsList.AsReadOnly();

        public async static Task<TelegramBotClient> Get()
        {
            if(client != null)
            {
                return client;
            }

            client = new TelegramBotClient(AppSettings.Token);
            await client.SetWebhookAsync(AppSettings.Url);

            commandsList.Add(new MustardBoyCommand());

            return client;
        }
    }
}
