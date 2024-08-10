using BotForMushrooms.Models.Commands.CommandExecutros;
using BotForMushrooms.Models.Commands.GlobalCommands.Quiz.Implements.AnswerTypes;

using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using System.Text;
using Newtonsoft.Json;

namespace BotForMushrooms.Models.Commands.GlobalCommands.Quiz.Implements
{
    public class QuizCommand : IQuizGame
    {
        public string Name => "quiz_game";

        public GlobalCommandExecutor Executor { get; }

        public QuizSettings QuizSettings { get; set; }

        public int AmountLeftRounds { get; set; }

        public Message? QuizMessage { get; set; }

        public bool QuizIsStart { get; set; }

        public LinkedList<IQuizSettingCommand> SettingsCommand { get; set; }

        public LinkedListNode<IQuizSettingCommand>? CurrentSetting { get; set; }

        public MultipleAnswerUpdater MultipleAnswerUpdater { get; set; }

        public QuizQuestion? CurrentQuestion { get; set; }

        public Dictionary<string, int> UserScores { get; set; }

        public int BanVotes { get; set; }

        public int SkipVotes { get; set; }


        public QuizCommand(GlobalCommandExecutor executor)
        {
            Executor = executor;
            QuizMessage = null;
            QuizSettings = new QuizSettings();
            QuizIsStart = false;
            CurrentQuestion = null;

            SettingsCommand = new([
                new QuizStartMenuCommand(this),
                new SetQuizThemeCommand(this),
                new SetQuizDifficultyCommand(this),
                new SetQuizAnswerTypeCommand(this),
                new SetQuizAmountRoundsCommand(this)
            ]);

            CurrentSetting = SettingsCommand.First;

            MultipleAnswerUpdater = new MultipleAnswerUpdater(this, executor, QuizSettings);

            AmountLeftRounds = -1;

            UserScores = [];
        }

        public async Task Execute(Message message, ITelegramBotClient client)
        {
            var text = message.Text;

            if (text == null)
            {
                return;
            }

            var textParts = text.Substring(0, text.LastIndexOf('@')).Split('_');
            var command = textParts[^1];

            var chatId = message.Chat.Id;

            if (command.Equals("start"))
            {
                Executor.StartQuizGame(this);
                await CurrentSetting.Value.Execute(message, client);
                BanVotes = 0;
                SkipVotes = 0;
                UserScores.Clear();
            }
            else if (command.Equals("stop"))
            {
                Executor.StopQuizGame();
                if(QuizIsStart)
                {
                    MultipleAnswerUpdater.StopQuestion();
                    QuizIsStart = false;
                }
                await client.SendTextMessageAsync(
                    chatId: QuizMessage.Chat.Id,
                    text: "Остановка игры!\n",
                    replyMarkup: new ReplyKeyboardRemove()
                );

                await GetUserScores(client);
            }
            else if (command.Equals("skip"))
            {
                SkipVotes++;
                if (QuizIsStart && (SkipVotes / UserScores.Keys.Count > 0.5))
                {
                    MultipleAnswerUpdater.StopQuestion();
                    SkipVotes = 0;
                }
            }
            else if (command.Equals("ban"))
            {
                BanVotes++;
                if (QuizIsStart && (BanVotes / UserScores.Keys.Count > 0.5))
                {
                    await BanQuestion();
                    MultipleAnswerUpdater.StopQuestion();
                    BanVotes = 0;
                }
            }
        }

        public async Task GetUpdate(Message update, ITelegramBotClient client)
        {
            var chatId = update.Chat.Id;
            if (update.ReplyToMessage == null ||
                update.ReplyToMessage.MessageId != QuizMessage.MessageId)
            {
                return;
            }

            var text = update.Text;

            if (text == null)
            {
                return;
            }

            var command = text.Substring(0, text.LastIndexOf(' '));

            if (!QuizIsStart)
            {
                await CurrentSetting.Value.GetUpdate(update, client);
                if (CurrentSetting.Value.CurrentSetting == QuizSettingsEnum.StartMenu)
                {
                    if (QuizSettings.IsSet && QuizIsStart)
                    {
                        await client.SendTextMessageAsync(
                            chatId: chatId,
                            text: "Игра началась!\n" + QuizSettings,
                            replyMarkup: new ReplyKeyboardRemove()
                        );
                        AmountLeftRounds = (int)QuizSettings.AmountRounds;

                        _ = Task.Run(async () =>
                        {
                            try
                            {
                                int amountRounds = 0;
                                while (QuizIsStart)
                                {
                                    await MultipleAnswerUpdater.Execute(update, client);
                                    BanVotes = 0;
                                    SkipVotes = 0;
                                    AmountLeftRounds--;
                                    amountRounds++;
                                    if (amountRounds == 5)
                                    {
                                        await GetUserScores(client);
                                        amountRounds = 0;
                                    }
                                    if (AmountLeftRounds == 0 && QuizSettings.AmountRounds != QuizAmountRoundsEnum.EternalGame)
                                    {
                                        break;
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.Message);
                            }

                            QuizIsStart = false;
                            await GetUserScores(client);
                        });
                    }
                }

                if (CurrentSetting.Value.IsSet)
                {
                    CurrentSetting = CurrentSetting.Next;
                    if (CurrentSetting == null)
                    {
                        CurrentSetting = SettingsCommand.First;
                    }
                    await CurrentSetting.Value.Execute(update, client);
                }
            }
        }

        public bool Contains(string? command)
        {
            if (command == null)
            {
                return false;
            }

            return command.StartsWith('/' + Name) && command.Contains('@' + AppSettings.ShortName);
        }

        private async Task GetUserScores(ITelegramBotClient client)
        {
            string resMessage = "Общие результаты:\n\n";
            var sortedUserScores = UserScores.OrderBy(kv => kv.Key);
            int place = 1;
            foreach (var user in sortedUserScores)
            {
                resMessage += place + ". " + user.Key + " - " + user.Value + " очк(о/ов)\n";
                place++;
            }

            await client.SendTextMessageAsync(QuizMessage.Chat.Id, resMessage);
        }

        private async Task BanQuestion()
        {
            using HttpClient client = new HttpClient();

            string url = AppSettings.Url + "/api/telegram_quiz/BanQuestion";

            BanQuestion banQuestion = new BanQuestion(CurrentQuestion.Question);

            var json = JsonConvert.SerializeObject(banQuestion);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(url, content);
        }
    }
}
