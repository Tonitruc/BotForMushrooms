using BotForMushrooms.Models.ChatListeners;
using BotForMushrooms.Models.Commands.CommandExecutros;
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

        public QuizSettingsEnum? NextCommand => QuizSettingsEnum.Difficulty;


        public SetQuizThemeCommand(IQuizGame executor)
        {
            Executor = executor;
            IsSet = false;
        }

        public async Task Execute(Message message, ITelegramBotClient client)
        {
            var chatId = message.Chat.Id;
            var text = message.Text;

            var replyKeyboard = new ReplyKeyboardMarkup(new[]
{
                [ "Все темы 🌍" ],
                ["Игры 💻",  ],
                new KeyboardButton[] { "ТОЛЬКО ФУТБОЛ!!!" }
            })
            {
                ResizeKeyboard = true
            };

            Executor.QuizMessage = await client.SendTextMessageAsync(chatId, "Выберите тему: ", replyMarkup: replyKeyboard);
        }

        public async Task GetUpdate(Message update, ITelegramBotClient client)
        {
            var text = update.Text;
            if(text == null)
            {
                return;
            }

            var themeText = text.Substring(0, text.LastIndexOf(' '));
            QuizThemeEnum theme = themeText switch
            {
                "Все темы" => QuizThemeEnum.AllTheme,
                "Игры" => QuizThemeEnum.VideoGames
            };

            SetCommand(theme.ToString());
        }

        public void SetCommand(string parametr)
        {
            Executor.QuizSettings.Theme = (QuizThemeEnum)Enum.Parse(typeof(QuizThemeEnum), parametr);
            IsSet = true;
        }
    }
}
