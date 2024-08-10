using BotForMushrooms.Models;
using BotForMushrooms.Models.Commands.GlobalCommands.Quiz.Implements;
using Microsoft.EntityFrameworkCore;

namespace BotForMushrooms.Repository
{
    public class BanQuestionDbContext : DbContext
    {
        public BanQuestionDbContext(DbContextOptions<BanQuestionDbContext> options) : base(options)
        { }

        public DbSet<BanQuestion> BanQuestions { get; set; }
    }
}
