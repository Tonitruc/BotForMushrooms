using BotForMushrooms.Models;
using BotForMushrooms.Models.Commands.GlobalCommands.Quiz.Implements;

namespace BotForMushrooms.Repository.Implements
{
    public class BanQuestionRepository : IBanQuestionRepository
    {
        private BanQuestionDbContext Context;

        public BanQuestionRepository(BanQuestionDbContext context)
        {
            Context = context;
        }

        public IEnumerable<BanQuestion> GetAll()
        {
            return Context.BanQuestions;
        }

        public BanQuestion? Get(long Id)
        {
            return Context.BanQuestions.Find(Id);
        }

        public BanQuestion? GetByQuestionText(string questionText)
        {
            return Context.BanQuestions.FirstOrDefault(x => x.QuestionText == questionText);
        }

        public BanQuestion? Add(BanQuestion banQuestion)
        {
            Context.BanQuestions.Add(banQuestion);
            Context.SaveChanges();
            return banQuestion;
        }

        public bool Delete(long Id)
        {
            BanQuestion? existBanQuestion = Get(Id);

            if (existBanQuestion != null)
            {
                Context.BanQuestions.Remove(existBanQuestion);
                Context.SaveChanges();
                return true;
            }

            return false;
        }

        public bool DeleteByQuestionText(string questionText)
        {
            BanQuestion? existFoodPlace = GetByQuestionText(questionText);

            return Delete(existFoodPlace?.Id ?? -1);
        }
    }
}
