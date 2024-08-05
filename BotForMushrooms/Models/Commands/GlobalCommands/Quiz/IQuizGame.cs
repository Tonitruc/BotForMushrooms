using BotForMushrooms.Models.ChatListeners;
using BotForMushrooms.Models.Commands.CommandExecutros;
using BotForMushrooms.Models.Commands.GlobalCommands.Quiz.Implements;
using Telegram.Bot.Types;

namespace BotForMushrooms.Models.Commands.GlobalCommands.Quiz
{
    public interface IQuizGame : ICommand<Message>, IListener<Message, GlobalCommandExecutor>
    {
        public Dictionary<QuizSettingsEnum, IQuizSettingCommand> SettingsCommand { get; set; }
        public QuizSettingsEnum CurrentSetting { get; set; }
        public Message? QuizMessage { get; set; }
        public QuizSettings QuizSettings { get; set; }
    }
}
