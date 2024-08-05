using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types;
using Telegram.Bot;

namespace BotForMushrooms.Models.Commands.GlobalCommands.Quiz.Implements
{
    public class SetQuizAnswerTypeCommand : IQuizSettingCommand
    {
        public string Name => "set_quiz_answer_type_command";

        public IQuizGame Executor { get; }

        public bool IsSet { get; set; }

        public QuizSettingsEnum CurrentSetting => QuizSettingsEnum.AnswerType;


        public SetQuizAnswerTypeCommand(IQuizGame executor)
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
                [ "Выбор варианта ☑ " ],
                [ "Абсолютный ответ ✍", ],
                [ "Да или нет ?¿", ],
                new KeyboardButton[] { "Все 🧐" }
            })
            {
                ResizeKeyboard = true
            };

            Executor.QuizMessage = await client.SendTextMessageAsync(chatId, "Выберите тип ответа: ", replyMarkup: replyKeyboard);
        }

        public async Task GetUpdate(Message update, ITelegramBotClient client)
        {
            var text = update.Text;
            if (text == null)
            {
                return;
            }

            var answerTypeText = text.Substring(0, text.LastIndexOf(' '));
            QuizAnswerTypeEnum amswerType = answerTypeText switch
            {
                "Выбор варианта" => QuizAnswerTypeEnum.Multiple,
                "Абсолютный ответ" => QuizAnswerTypeEnum.AbsoluteAnswer,
                "Да или нет" => QuizAnswerTypeEnum.YesOrNot,
                "Все" => QuizAnswerTypeEnum.All
            };

            SetCommand(amswerType.ToString());
        }

        public void SetCommand(string parametr)
        {
            Executor.QuizSettings.AnswerType = (QuizAnswerTypeEnum)Enum.Parse(typeof(QuizAnswerTypeEnum), parametr);
            IsSet = true;
        }
    }
}
