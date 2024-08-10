using BotForMushrooms.Models.ChatListeners;
using BotForMushrooms.Models.Commands.GlobalCommands.Quiz.Implements;
using Telegram.Bot.Types;

namespace BotForMushrooms.Models.Commands.GlobalCommands.Quiz
{
    public interface IQuizAnswerUpdater
    {
        Message? QuiestionMessage { get; set; }
        IQuizGame QuizGame { get; }
    }
}
