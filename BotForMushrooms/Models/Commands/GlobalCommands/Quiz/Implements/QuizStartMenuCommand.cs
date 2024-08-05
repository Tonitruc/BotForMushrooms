using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace BotForMushrooms.Models.Commands.GlobalCommands.Quiz.Implements
{
    public class QuizStartMenuCommand : IQuizSettingCommand
    {
        public string Name => "quiz_start_menu_command";

        public const string QuizTitleImage = @"https://drive.google.com/uc?export=view&id=19tl8YYyH2Nu42ix1MOFnlXbSiMWZH97t";

        public bool IsSet { get; set; }

        public QuizSettingsEnum CurrentSetting => QuizSettingsEnum.StartMenu;

        public IQuizGame Executor { get; }

        public QuizStartMenuCommand(IQuizGame executor)
        {
            Executor = executor;
        }

        public async Task Execute(Message message, ITelegramBotClient client)
        {
            IsSet = false;
            var chatId = message.Chat.Id;

            var replyKeyboard = new ReplyKeyboardMarkup(new[]
            {
                [ "Старт ▶" ],
                new KeyboardButton[] { "Настроить игру ⚙" }
                })
            {
                ResizeKeyboard = true
            };

            var prevGame = Executor.QuizSettings.IsSet ? Executor.QuizSettings?.ToString() : "Еще не было игр.";

            Executor.QuizMessage = await client.SendPhotoAsync(
                chatId: chatId,
                caption: $"Настройки игры:\n{prevGame}",
                photo: InputFile.FromUri(QuizTitleImage),
                replyMarkup: replyKeyboard
            );
        }

        public async Task GetUpdate(Message update, ITelegramBotClient client)
        {
            var text = update.Text;
            if (text == null)
            {
                return;
            }

            string command = text.Substring(0, text.LastIndexOf(' '));

            SetCommand(command);
        }

        public void SetCommand(string parametr)
        {
            if(parametr.Equals("Старт"))
            {
                if(Executor.QuizSettings.IsSet)
                {
                    Executor.QuizIsStart = true;
                }
            }
            else if(parametr.Equals("Настроить игру"))
            {
                IsSet = true;
            }
        }
    }
}
