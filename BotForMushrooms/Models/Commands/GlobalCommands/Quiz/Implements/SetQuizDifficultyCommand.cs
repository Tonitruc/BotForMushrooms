using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types;
using Telegram.Bot;
using Microsoft.IdentityModel.Tokens;

namespace BotForMushrooms.Models.Commands.GlobalCommands.Quiz.Implements
{
    public class SetQuizDifficultyCommand : IQuizSettingCommand
    {
        public string Name => "set_quiz_difficulty_command";

        public IQuizGame Executor { get; }

        public bool IsSet { get; set; }

        public QuizSettingsEnum CurrentSetting => QuizSettingsEnum.Difficulty;


        public SetQuizDifficultyCommand(IQuizGame executor)
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
                [ "Легко 🟢" ],
                [ "Средне 🟡", ],
                [ "Сложно 🔴", ],
                new KeyboardButton[] { "Все 🔵" }
            })
            {
                ResizeKeyboard = true
            };

            Executor.QuizMessage = await client.SendTextMessageAsync(chatId, "Выберите сложность: ", replyMarkup: replyKeyboard);
        }

        public async Task GetUpdate(Message update, ITelegramBotClient client)
        {
            var text = update.Text;
            if (text == null)
            {
                return;
            }

            var difficultyText = text.Substring(0, text.LastIndexOf(' '));
            QuizDifficultyEnum? difficulty = difficultyText switch
            {
                "Легко" => QuizDifficultyEnum.Easy,
                "Средне" => QuizDifficultyEnum.Medium,
                "Сложно" => QuizDifficultyEnum.Hard,
                "Все" => QuizDifficultyEnum.All,
                 _ => null
            };

            SetCommand(difficulty.ToString());
        }

        public void SetCommand(string? parametr)
        {
            if (parametr.IsNullOrEmpty())
            {
                return;
            }

            Executor.QuizSettings.Difficulty = (QuizDifficultyEnum)Enum.Parse(typeof(QuizDifficultyEnum), parametr);
            IsSet = true;
        }
    }
}