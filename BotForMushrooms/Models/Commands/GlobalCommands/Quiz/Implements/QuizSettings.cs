namespace BotForMushrooms.Models.Commands.GlobalCommands.Quiz.Implements
{
    public class QuizSettings
    {
        public QuizAmountRoundsEnum? AmountRounds { get; set; }

        public QuizDifficultyEnum? Difficulty { get; set; }

        public QuizThemeEnum? Theme { get; set; }

        public QuizAnswerTypeEnum? AnswerType { get; set; }

        public bool IsSet => AmountRounds != null && Difficulty != null && Theme != null && AnswerType != null;


        public QuizSettings()
        {
            AmountRounds = null;
            Difficulty = null;
            Theme = null;
            AnswerType = null;
        }

        public override string ToString()
        {
            return string.Format($"Количество раундов: {AmountRounds}\n"
                + $"Сложность: {Difficulty}\n"
                + $"Тема: {Theme}\n"
                + $"Тип ответа: {AnswerType}\n");
        }
    }
}
