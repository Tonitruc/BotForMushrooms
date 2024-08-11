using BotForMushrooms.Models.ChatListeners;
using BotForMushrooms.Models.Commands.CommandExecutros;
using BotForMushrooms.Models.Commands.GlobalCommands.Quiz.Implements;
using Telegram.Bot.Types;

namespace BotForMushrooms.Models.Commands.GlobalCommands.Quiz
{
    public interface IQuizAnswerUpdater : ICommand<Message>
    {
        Message? QuestionMessage { get; set; }
        IQuizGame QuizGame { get; }

        void StopQuestion();
    }
}
