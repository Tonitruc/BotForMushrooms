using BotForMushrooms.Models.ChatListeners;
using Telegram.Bot.Types;

namespace BotForMushrooms.Models.Commands.GlobalCommands.Quiz
{
    public interface IQuizSettingCommand : ICommand<Message>, IListener<Message, IQuizGame>
    {
        public bool IsSet { get; }
        public QuizSettingsEnum CurrentSetting { get; }
        public void SetCommand(string? parametr);
    }
}
