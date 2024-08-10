using Microsoft.IdentityModel.Tokens;
using System.Text.Unicode;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace BotForMushrooms.Models.Commands.GlobalCommands.Quiz.Implements
{
    public class SetQuizThemeCommand : IQuizSettingCommand
    {
        public string Name => "set_quiz_theme_command";

        public IQuizGame Executor { get; }

        public bool IsSet { get; set; }

        public QuizSettingsEnum CurrentSetting => QuizSettingsEnum.Theme;

        public SetQuizThemeCommand(IQuizGame executor)
        {
            Executor = executor;
            IsSet = false;
        }

        public async Task Execute(Message message, ITelegramBotClient client)
        {
            IsSet = false;
            var chatId = message.Chat.Id;
            var text = message.Text;
            string alien = char.ConvertFromUtf32(0x1F47D);

            var replyKeyboard = new ReplyKeyboardMarkup(new[]
{
                [ "Все темы 🌍" ],
                [$"Наука {alien}",  "Развлечения 💲"],
                new KeyboardButton[] { "Видеоигры 💻", "Общие знания 🧠" }
            })
            {
                ResizeKeyboard = true
            };

            Executor.QuizMessage = await client.SendTextMessageAsync(chatId, "Выберите тему: ", replyMarkup: replyKeyboard);
        }

        public Task GetUpdate(Message update, ITelegramBotClient client)
        {
            var text = update.Text;
            if(text == null)
            {
                return Task.CompletedTask;
            }

            int lastIndexSpace = text.LastIndexOf(' ');
            if (lastIndexSpace == -1)
            {
                return Task.CompletedTask;
            }

            string themeText = text.Substring(0, lastIndexSpace);

            QuizThemeEnum? theme = themeText switch
            {
                "Все темы" => QuizThemeEnum.AllTheme,
                "Видеоигры" => QuizThemeEnum.VideoGames,
                "Наука" => QuizThemeEnum.Science,
                "Развлечения" => QuizThemeEnum.Entertaiment,
                "Общие знания" => QuizThemeEnum.GeneralKnowlage,
                _ => null
            };

            SetCommand(theme.ToString());

            return Task.CompletedTask;
        }

        public void SetCommand(string? parametr)
        {
            if (string.IsNullOrEmpty(parametr))
            {
                return;
            }

            Executor.QuizSettings.Theme = (QuizThemeEnum)Enum.Parse(typeof(QuizThemeEnum), parametr);
            IsSet = true;
        }
    }
}
