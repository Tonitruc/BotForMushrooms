using BotForMushrooms.Models.ChatListeners;
using BotForMushrooms.Models.Commands.GlobalCommands.HoroscopePredict;
using BotForMushrooms.Models.Commands.GlobalCommands.Quiz;
using BotForMushrooms.Models.Commands.GlobalCommands.Quiz.Implements;
using BotForMushrooms.Models.Commands.MenuCommands;
using Telegram.Bot.Types;

namespace BotForMushrooms.Models.Commands.CommandExecutros
{
    public class GlobalCommandExecutor : CommandExecutor<ITelegramMenu>
    {
        public override Dictionary<long, ITelegramMenu> ActiveMenus { get; }

        protected override List<ICommand<Message>> CommandList { get; }

        public override ChatUpdater ChatUpdater { get; }

        protected IQuizGame? QuizGame { get; set; } = null;

        protected IListener<Message, GlobalCommandExecutor>? Listener { get; set; } = null;

        protected IListener<PollAnswer, GlobalCommandExecutor>? PollAnswerListener { get; set; } = null;


        public GlobalCommandExecutor(ChatUpdater chatUpdater)
        {
            CommandList = [
                new MustardBoyCommand(),
                new HoroscopePredictCommand(this),
                new QuizCommand(this)
            ];

            ActiveMenus = [];
            ChatUpdater = chatUpdater;
        }

        public override async Task GetUpdate(Update update)
        {
            if (update.Message != null)
            {
                Message message = update.Message;
                ICommand<Message>? command = CommandList.FirstOrDefault(command => command.Contains(message.Text));
                if (command != null)
                {
                    await command.Execute(message, client);
                }
                else if(QuizGame != null)
                {
                    await QuizGame.GetUpdate(message, client);
                }
                else if(Listener != null)
                {
                    await Listener.GetUpdate(message, client);
                }
            }
            else if(update.CallbackQuery != null)
            {
                CallbackQuery callbackQuery = update.CallbackQuery;
                string? data = callbackQuery.Data;

                if (data == null)
                {
                    return;
                }

                foreach (var menu in ActiveMenus.Values)
                {
                    if(data.StartsWith(menu.Name))
                    {
                        await menu.GetUpdate(callbackQuery, client);
                        break;
                    }
                }
            }
            else if(update.PollAnswer != null)
            {
                if(PollAnswerListener != null)
                {
                    PollAnswer pollAnswer = update.PollAnswer;
                    await PollAnswerListener.GetUpdate(pollAnswer, client);
                }
            }
        }

        public void StartPollAnswerListen(Poll poll, IListener<PollAnswer, GlobalCommandExecutor> pollAnswerListener)
        {
            ChatUpdater.AddPoll(poll.Id);
            PollAnswerListener = pollAnswerListener;
        }

        public void StopPollAnswerListen(Poll poll)
        {
            PollAnswerListener = null;
            ChatUpdater.RemovePoll(poll.Id);
        }

        public void StartListen(IListener<Message, GlobalCommandExecutor> listener)
        {
            Listener = listener; 
        }

        public void StopListen()
        {
            Listener = null;
        }

        public void StartQuizGame(IQuizGame quizGame)
        {
            QuizGame = quizGame;
        }

        public void StopQuizGame()
        {
            QuizGame = null;
        }
    }
}
