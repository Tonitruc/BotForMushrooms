using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BotForMushrooms.Models.Commands.GlobalCommands.Quiz.Implements
{
    public class BanQuestion
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public string QuestionText { get; set; }


        public BanQuestion(string questionText)
        {
            QuestionText = questionText;
        }
    }
}
