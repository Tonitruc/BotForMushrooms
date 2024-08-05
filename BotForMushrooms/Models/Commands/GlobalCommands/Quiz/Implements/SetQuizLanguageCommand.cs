using Microsoft.IdentityModel.Tokens;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types;
using Telegram.Bot;

namespace BotForMushrooms.Models.Commands.GlobalCommands.Quiz.Implements
{
    public class SetQuizLanguageCommand : IQuizSettingCommand
    {
        public string Name => "set_quiz_language_command";

        public IQuizGame Executor { get; }

        public bool IsSet { get; set; }

        public QuizSettingsEnum CurrentSetting => QuizSettingsEnum.AnswerType;


        public SetQuizLanguageCommand(IQuizGame executor)
        {
            Executor = executor;
            IsSet = false;
        }

        public async Task Execute(Message message, ITelegramBotClient client)
        {
            IsSet = false;
            var chatId = message.Chat.Id;
            var text = message.Text;

            var replyKeyboard = new ReplyKeyboardMarkup(new[]
{
                [ "Русский ⬜🟦🟥" ],
                [ "English 🍵", ],
                new KeyboardButton[] { "Все 🧐" }
            })
            {
                ResizeKeyboard = true
            };

            Executor.QuizMessage = await client.SendTextMessageAsync(chatId, "Выберите язык: ", replyMarkup: replyKeyboard);
        }

        public async Task GetUpdate(Message update, ITelegramBotClient client)
        {
            var text = update.Text;
            if (text == null)
            {
                return;
            }

            var answerTypeText = text.Substring(0, text.LastIndexOf(' '));
            QuizLanguageEnum? amswerType = answerTypeText switch
            {
                "Русский" => QuizLanguageEnum.Russian,
                "English" => QuizLanguageEnum.English,
                "Все" => QuizLanguageEnum.AllLanguage,
                _ => null
            };

            SetCommand(amswerType.ToString());
        }

        public void SetCommand(string? parametr)
        {
            if (parametr.IsNullOrEmpty())
            {
                return;
            }

            Executor.QuizSettings.Language = (QuizLanguageEnum)Enum.Parse(typeof(QuizLanguageEnum), parametr);
            IsSet = true;
        }
    }
}
