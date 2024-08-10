using BotForMushrooms.Models.ChatListeners;
using BotForMushrooms.Models.Commands.CommandExecutros;
using BotForMushrooms.Models.Commands.GlobalCommands.Quiz.Implements;
using Telegram.Bot.Types;

namespace BotForMushrooms.Models.Commands.GlobalCommands.Quiz
{
    public interface IQuizGame : ICommand<Message>, IListener<Message, GlobalCommandExecutor>
    {
        LinkedList<IQuizSettingCommand> SettingsCommand { get; set; }
        LinkedListNode<IQuizSettingCommand>? CurrentSetting { get; set; }
        Message? QuizMessage { get; set; }
        QuizSettings QuizSettings { get; set; }
        bool QuizIsStart { get; set; }
        Dictionary<string, int> UserScores { get; set; }
        int AmountLeftRounds { get; set; }
        QuizQuestion? CurrentQuestion { get; set; }
    }
}
