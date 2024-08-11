using BotForMushrooms.Models.ChatListeners;
using BotForMushrooms.Models.Commands.CommandExecutros;
using BotForMushrooms.Models.Commands.GlobalCommands.Quiz.Implements.QuizApi;

using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;
using Telegram.Bot;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace BotForMushrooms.Models.Commands.GlobalCommands.Quiz.Implements.AnswerTypes
{
    public class AbsoluteAnswerUpdater : IQuizAnswerUpdater, IListener<Message, GlobalCommandExecutor>
    {
        public string Name => "quiz_absolute_answer_updater";

        public const int PeriodOfTimeBetweenTip = 15;

        public GlobalCommandExecutor Executor { get; }

        public string QuizThemeFromApi { get; set; }

        public Message? QuestionMessage { get; set; }

        public QuizSettings QuizSettings { get; set; }

        public int Points { get; set; }

        private Dictionary<int, char> CloseLetters { get; }

        private int AmountLetters { get; set; }

        private string Tip { get; set; } 

        private bool IsAnswer { get; set; }

        public IQuizGame QuizGame { get; }

        private CancellationTokenSource? CancellationTokenSource { get; set; }

        public AbsoluteAnswerUpdater(IQuizGame quizGame, GlobalCommandExecutor executor, QuizSettings quizSettings)
        {
            Executor = executor;
            QuizSettings = quizSettings;
            QuizGame = quizGame;
            QuizThemeFromApi = string.Empty;
            Points = 10;
            CloseLetters = [];
            Tip = string.Empty;
        }

        public async Task Execute(Message message, ITelegramBotClient client)
        {
            TriviaQuizApi triviaQuizApi = new TriviaQuizApi();

            QuizSettings quizSettings = QuizSettings with { AnswerType = QuizAnswerTypeEnum.Multiple };
            QuizQuestion? question = await triviaQuizApi.GetQuestion(quizSettings);
            question = question ?? throw new NullReferenceException("Bad Request from TriviaApi");
            QuizGame.CurrentQuestion = question;          

            string amountRounds = QuizSettings.AmountRounds != QuizAmountRoundsEnum.EternalGame ? QuizGame.AmountLeftRounds.ToString() : "∞";
            await client.SendTextMessageAsync(chatId: message.Chat.Id, $"Осталось раундов: [{amountRounds}]");

            string resMessage = $"Min очки: +1; Max очки: +12; Тема вопроса: {question.Theme}\n\nВопрос: " + $"<b>{question.Question}</b>";

            Executor.StartListen(this);

            CancellationTokenSource = new CancellationTokenSource();
            CancellationToken cts = CancellationTokenSource.Token;
            await Task.Run(async () =>
            {
                Tip = string.Empty;
                IsAnswer = false;
                int amountTips;
                if(question.CorrectAnswer.Count(c => !char.IsWhiteSpace(c)) <= 3)
                {
                    amountTips = question.CorrectAnswer.Length;
                    Points = 7;
                }
                else
                {
                    amountTips = 4;
                    Points = 12;
                }

                try
                {
                    QuestionMessage = await client.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: resMessage,
                        parseMode: ParseMode.Html
                    );
                    await Task.Delay(PeriodOfTimeBetweenTip * 1000, cts);

                    for (int i = 0; i < amountTips; i++)
                    {
                        Tip = GetTips(i, amountTips);
                        resMessage = $"Очки: +{Points}\n\nВопрос: <b>{question.Question}\n\n</b>" + $"Подсказка:\n <i>{Tip}</i>";

                        QuestionMessage = await client.SendTextMessageAsync(
                            chatId: message.Chat.Id,
                            text: resMessage,
                            parseMode: ParseMode.Html
                        );
                        await Task.Delay(PeriodOfTimeBetweenTip * 1000, cts);
                    }
                    await GetResult(message.From, client);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    if(!IsAnswer)
                    {
                        await GetResult(message.From, client);
                    }
                }
                Executor.StopListen();
            }, cts);
        }

        public async Task GetUpdate(Message update, ITelegramBotClient client)
        {
            var user = update.From;
            QuizGame.UserScores.TryAdd(user.Id, (0, user.Username));
            if (update.Text == QuizGame.CurrentQuestion.CorrectAnswer)
            {
                IsAnswer = true;
                await GetResult(user, client, IsAnswer);
                StopQuestion();
            }
        }

        public async Task GetResult(User user, ITelegramBotClient client, bool isAnswer = false)
        {
            var chatId = QuestionMessage.Chat.Id;

            string resultString = $"\U0001f7e2 Верный ответ - <b> {QuizGame.CurrentQuestion.CorrectAnswer} </b>\n\n";
            if(isAnswer)
            {
                resultString += $"{user.Username}   +{Points}";
                var pair = QuizGame.UserScores[user.Id];
                pair.Item1 += Points;
                QuizGame.UserScores[user.Id] = pair;
            }
            else
            {
                resultString += "🔴 Никто не ответил правильно";
            }

            await client.SendTextMessageAsync(
                chatId: chatId,
                text: "<i>Результаты вопроса:</i> \n\n" + resultString,
                parseMode: ParseMode.Html
                );

            CloseLetters.Clear();
        }

        public void StopQuestion()
        {
            if (CancellationTokenSource == null)
            {
                return;
            }

            CancellationTokenSource.Cancel();
        }

        private string GetTips(int numTip, int amountTips)
        {
            StringBuilder newTip = new(Tip);
            int amountOpenLetters = 0;
            
            if (numTip == 0)
            {
                AmountLetters = 0;
                int letterIndex = 0;
                for (int i = 0; i < QuizGame.CurrentQuestion.CorrectAnswer.Length; i++)
                {
                    if (char.IsLetter(QuizGame.CurrentQuestion.CorrectAnswer[i]))
                    {
                        newTip.Append("_ ");
                        CloseLetters.Add(letterIndex, QuizGame.CurrentQuestion.CorrectAnswer[i]);
                        letterIndex += 2;
                        AmountLetters++;
                    }
                    else if(char.IsWhiteSpace(QuizGame.CurrentQuestion.CorrectAnswer[i]))
                    {
                        newTip.Append("  ");
                        letterIndex += 2;
                    }
                    else
                    {
                        newTip.Append("_ ");
                        CloseLetters.Add(letterIndex, QuizGame.CurrentQuestion.CorrectAnswer[i]);
                        letterIndex += 2;
                        AmountLetters++;
                    }
                }
                Points -= 3;
            }
            else if(numTip == 1)
            {
                if(AmountLetters > 3)
                {
                    amountOpenLetters = (int)Math.Round(0.3 * AmountLetters);
                    Points -= 3;
                }
                else
                {
                    amountOpenLetters = 1;
                    Points -= 2;
                }
            }
            else if(numTip == 2)
            {
                if (AmountLetters > 3)
                {
                    amountOpenLetters = (int)Math.Round(0.3 * AmountLetters);
                    Points -= 3;
                }
                else
                {
                    amountOpenLetters = 1;
                    Points -= 1;
                }
            }
            else if(numTip == 3)
            {
                amountOpenLetters = (int)Math.Round(0.3 * AmountLetters);
                Points -= 2;
            }

            Random rand = new Random();
            for(int i = 0; i < amountOpenLetters; i++)
            {
                int openLetterNum = rand.Next(CloseLetters.Count);
                var closeLettersList = CloseLetters.ToList();

                newTip[closeLettersList[openLetterNum].Key] = closeLettersList[openLetterNum].Value;
                CloseLetters.Remove(closeLettersList[openLetterNum].Key);
            }
            return newTip.ToString();
        }
    }
}
