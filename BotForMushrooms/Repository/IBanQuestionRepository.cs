using BotForMushrooms.Models;
using BotForMushrooms.Models.Commands.GlobalCommands.Quiz.Implements;

namespace BotForMushrooms.Repository
{
    public interface IBanQuestionRepository
    {
        IEnumerable<BanQuestion> GetAll();
        BanQuestion? Get(long id);
        BanQuestion? GetByQuestionText(string questionText);
        BanQuestion? Add(BanQuestion banQuestion);
        bool Delete(long id);
        bool DeleteByQuestionText(string questionText);
    }
}
