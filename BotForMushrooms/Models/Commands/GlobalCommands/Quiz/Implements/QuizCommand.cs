using BotForMushrooms.Models.ChatListeners;
using BotForMushrooms.Models.Commands.CommandExecutros;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace BotForMushrooms.Models.Commands.GlobalCommands.Quiz.Implements
{
    public class QuizCommand : IQuizGame
    {
        public string Name => "quiz_game";

        public GlobalCommandExecutor Executor { get; }

        public QuizSettings QuizSettings { get; set; }

        public Message? QuizMessage { get; set; }

        public bool QuizIsStart { get; set; }

        public LinkedList<IQuizSettingCommand> SettingsCommand { get; set; }

        public LinkedListNode<IQuizSettingCommand>? CurrentSetting { get; set; }

        public QuizCommand(GlobalCommandExecutor executor)
        {
            Executor = executor;
            QuizMessage = null;
            QuizSettings = new QuizSettings();
            QuizIsStart = false;

            SettingsCommand = new([
                new QuizStartMenuCommand(this),
                new SetQuizThemeCommand(this),
                new SetQuizDifficultyCommand(this),
                new SetQuizLanguageCommand(this),
                new SetQuizAnswerTypeCommand(this),
                new SetQuizAmountRoundsEnum(this)
            ]);
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
                Executor.StartListen(this);
                CurrentSetting = SettingsCommand.First;
                await CurrentSetting.Value.Execute(message, client);
            }
            else if (command.Equals("stop"))
            {
                Executor.StopListen();
                QuizIsStart = false;
                await client.SendTextMessageAsync(
                    chatId: QuizMessage.Chat.Id,  //cant be null
                    text: "Остановка игры!\n",
                    replyMarkup: new ReplyKeyboardRemove()
                );
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
                if(CurrentSetting.Value.CurrentSetting == QuizSettingsEnum.StartMenu)
                {
                    if(QuizSettings.IsSet && QuizIsStart)
                    {
                        QuizIsStart = false;

                        await client.SendTextMessageAsync(
                            chatId: chatId,
                            text: "Игра началась!\n" + QuizSettings,
                            replyMarkup: new ReplyKeyboardRemove()
                        );
                    }
                }

                if(CurrentSetting.Value.IsSet)
                {
                    CurrentSetting = CurrentSetting.Next;
                    if(CurrentSetting == null)
                    {
                        CurrentSetting = SettingsCommand.First;
                    }
                    await CurrentSetting.Value.Execute(update, client);
                }
            }
            else
            {

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
    }
}
