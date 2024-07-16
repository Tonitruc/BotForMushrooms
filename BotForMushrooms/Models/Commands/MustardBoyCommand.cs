using Telegram.Bot;
using Telegram.Bot.Types;

namespace BotForMushrooms.Models.Commands
{
    public class MustardBoyCommand : Command
    {
        public override string Name => "mustard_boy";

        public const string ImageUrl = @"https://drive.google.com/uc?export=view&id=1_C4A1100dB7WvcPFWc07h30Y3YGF_9TW";

        public override async Task Execute(Message message, ITelegramBotClient client)
        {
            var chatId = message.Chat.Id;

            var inputFile = new InputFileUrl(new Uri(ImageUrl));
            await client.SendPhotoAsync(chatId, inputFile, caption: "Mustard boy!");

        }
    }
}
