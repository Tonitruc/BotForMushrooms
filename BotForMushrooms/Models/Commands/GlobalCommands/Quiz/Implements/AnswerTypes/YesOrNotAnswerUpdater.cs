using BotForMushrooms.Models.Commands.CommandExecutros;
using BotForMushrooms.Models.Commands.GlobalCommands.Quiz.Implements.QuizApi;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;
using Telegram.Bot;
using BotForMushrooms.Models.ChatListeners;

namespace BotForMushrooms.Models.Commands.GlobalCommands.Quiz.Implements.AnswerTypes
{
    public class YesOrNotAnswerUpdater : IQuizAnswerUpdater, IListener<PollAnswer, GlobalCommandExecutor>
    {
        public string Name => "quiz_yes_or_not_answer_updater";

        public const int MaxPollMessageLength = 300;

        public GlobalCommandExecutor Executor { get; }

        public string QuizThemeFromApi { get; set; }

        public Message? QuestionMessage { get; set; }

        public QuizSettings QuizSettings { get; set; }

        public Message? CurrentPoll { get; set; }

        public Dictionary<long, (int, string)> Points { get; set; }

        public int CorrectIndex { get; set; }

        public IQuizGame QuizGame { get; }

        private CancellationTokenSource? CancellationTokenSource { get; set; }

        public YesOrNotAnswerUpdater(IQuizGame quizGame, GlobalCommandExecutor executor, QuizSettings quizSettings)
        {
            Executor = executor;
            QuizSettings = quizSettings;
            QuizGame = quizGame;
            QuizThemeFromApi = string.Empty;
            Points = [];
        }

        public async Task Execute(Message message, ITelegramBotClient client)
        {
            TriviaQuizApi triviaQuizApi = new TriviaQuizApi();

            QuizSettings quizSettings = QuizSettings with { AnswerType = QuizAnswerTypeEnum.YesOrNot };
            QuizQuestion? question = await triviaQuizApi.GetQuestion(quizSettings);
            question = question ?? throw new NullReferenceException("Bad Request from TriviaApi");
            QuizGame.CurrentQuestion = question;

            List<string> answers = GetAnswerList(question);

            string amountRounds = QuizSettings.AmountRounds != QuizAmountRoundsEnum.EternalGame ? QuizGame.AmountLeftRounds.ToString() : "∞";
            await client.SendTextMessageAsync(chatId: message.Chat.Id, $"Осталось раундов: [{amountRounds}]");

            string resMessage = $"Время: 30 секунд; Очки: +1; Тема вопроса: {question.Theme}\n\nВопрос: " + question.Question;
            if (resMessage.Length > MaxPollMessageLength)
            {
                await client.SendTextMessageAsync(chatId: message.Chat.Id, resMessage);
                resMessage = "Так как вопрос слишком большой, он находится в прошлом сообщении!";
            }

            var pollMessage = await client.SendPollAsync(
                chatId: message.Chat.Id,
                question: resMessage,
                options: answers,
                isAnonymous: false,
                type: PollType.Quiz,
                correctOptionId: CorrectIndex
            );

            CurrentPoll = pollMessage;

            Executor.StartPollAnswerListen(CurrentPoll.Poll, this);

            CancellationTokenSource = new CancellationTokenSource();
            CancellationToken cts = CancellationTokenSource.Token;
            await Task.Run(async () =>
            {
                try
                {
                    await Task.Delay(30000, cts);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                await client.StopPollAsync(message.Chat.Id, pollMessage.MessageId);

            }, cts);

            Executor.StopPollAnswerListen(CurrentPoll.Poll);
            await GetResult(client);
        }

        public Task GetUpdate(PollAnswer update, ITelegramBotClient client)
        {
            Points.Add(update.User.Id, (update.OptionIds.Contains(CorrectIndex) ? 1 : 0, update.User.Username));
            QuizGame.UserScores.TryAdd(update.User.Id, (0, update.User.Username));
            return Task.CompletedTask;
        }

        public async Task GetResult(ITelegramBotClient client)
        {
            var chatId = CurrentPoll.Chat.Id;

            string resultString = string.Empty;
            foreach (var userPoints in Points)
            {
                resultString += userPoints.Value.Item2 + ": +" + userPoints.Value.Item1 + ' ' + (userPoints.Value.Item1 == 1 ? "🟢" : "🔴") + '\n';
            }

            await client.SendTextMessageAsync(
                chatId: chatId,
                text: "<i>Результаты вопроса:</i> \n\n" + resultString,
                parseMode: ParseMode.Html
                );

            foreach (var userPoints in Points)
            {
                var pair = QuizGame.UserScores[userPoints.Key];
                pair.Item1 += userPoints.Value.Item1;
                QuizGame.UserScores[userPoints.Key] = pair;
            }

            Points = [];
        }

        public void StopQuestion()
        {
            if (CancellationTokenSource == null)
            {
                return;
            }

            CancellationTokenSource.Cancel();
        }

        private List<string> GetAnswerList(QuizQuestion quizQuestion)
        {
            List<string> answerList = [];

            CorrectIndex = quizQuestion.CorrectAnswer == "True" ? 0 : 1;
            answerList.AddRange(["True", "False"]);

            return answerList;
        }
    }
}
