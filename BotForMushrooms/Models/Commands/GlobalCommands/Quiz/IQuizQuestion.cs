namespace BotForMushrooms.Models.Commands.GlobalCommands.Quiz
{
    public interface IQuizQuestion
    {
        public string Source { get; set; }

        public string Question { get; set; }

        public string CorrectAnswer { get; }

        public Dictionary<string, string> IncorrectQuestions { get; }

        public QuizAnswerTypeEnum AnswerType { get; set; }
    }
}
