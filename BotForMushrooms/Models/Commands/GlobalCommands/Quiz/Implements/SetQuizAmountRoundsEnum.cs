using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types;
using Telegram.Bot;

namespace BotForMushrooms.Models.Commands.GlobalCommands.Quiz.Implements
{
    public class SetQuizAmountRoundsEnum : IQuizSettingCommand
    {
        public string Name => "set_quiz_amount_rounds_command";

        public IQuizGame Executor { get; }

        public bool IsSet { get; set; }

        public QuizSettingsEnum CurrentSetting => QuizSettingsEnum.AmountRounds;


        public SetQuizAmountRoundsEnum(IQuizGame executor)
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
                [ "10 раундов 🕙 " ],
                [ "20 раундов 2х🕙", ],
                [ "50 раундов 5х🕙", ],
                new KeyboardButton[] { "Бесконечно lim(🕙/0)" }
            })
            {
                ResizeKeyboard = true
            };

            Executor.QuizMessage = await client.SendTextMessageAsync(chatId, "Выберите количество раундов: ", replyMarkup: replyKeyboard);
        }

        public async Task GetUpdate(Message update, ITelegramBotClient client)
        {
            var text = update.Text;
            if (text == null)
            {
                return;
            }

            var answerTypeText = text.Substring(0, text.LastIndexOf(' '));
            QuizAmountRoundsEnum amswerType = answerTypeText switch
            {
                "10 раундов" => QuizAmountRoundsEnum.ShortGame,
                "20 раундовт" => QuizAmountRoundsEnum.MidleGame,
                "50 раундов" => QuizAmountRoundsEnum.LongGame,
                "Бесконечно" => QuizAmountRoundsEnum.EternalGame
            };

            SetCommand(amswerType.ToString());
        }

        public void SetCommand(string parametr)
        {
            Executor.QuizSettings.AmountRounds = (QuizAmountRoundsEnum)Enum.Parse(typeof(QuizAmountRoundsEnum), parametr);
            IsSet = true;
        }
    }
}
