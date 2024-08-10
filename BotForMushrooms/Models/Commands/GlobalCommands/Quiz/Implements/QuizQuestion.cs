namespace BotForMushrooms.Models.Commands.GlobalCommands.Quiz.Implements
{
    public class QuizQuestion
    {
        public string Question { get; set; }

        public QuizAnswerTypeEnum AnswerType { get; set; }

        public string CorrectAnswer { get; set; }

        public List<string> IncorrectAnswers { get; set; }

        public string? Theme { get; set; }

        public QuizQuestion(string question, QuizAnswerTypeEnum answerType, string correctAnswer, List<string> incorrectAnswers)
        {
            Question = question;
            AnswerType = answerType;
            CorrectAnswer = correctAnswer;
            IncorrectAnswers = incorrectAnswers;
            Theme = null;
        }

    }
}
