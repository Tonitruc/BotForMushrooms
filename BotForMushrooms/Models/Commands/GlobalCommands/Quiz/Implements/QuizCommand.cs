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

        public const string QuizTitleImage = @"https://drive.google.com/uc?export=view&id=19tl8YYyH2Nu42ix1MOFnlXbSiMWZH97t";

        public GlobalCommandExecutor Executor { get; }

        public QuizSettings QuizSettings { get; set; }

        public Message? QuizMessage { get; set; }

        public bool QuizIsStart { get; set; }

        public Dictionary<QuizSettingsEnum, IQuizSettingCommand> SettingsCommand { get; set; }

        public QuizSettingsEnum CurrentSetting { get; set; }

        public QuizCommand(GlobalCommandExecutor executor)
        {
            Executor = executor;
            QuizMessage = null;
            QuizSettings = new QuizSettings();
            QuizIsStart = false;

            SettingsCommand = new([
                new(QuizSettingsEnum.Theme, new SetQuizThemeCommand(this)),
                new(QuizSettingsEnum.Difficulty, new SetQuizDifficultyCommand(this)),
                new(QuizSettingsEnum.AnswerType, new SetQuizAnswerTypeCommand(this)),
                new(QuizSettingsEnum.AmountRounds, new SetQuizAmountRoundsEnum(this))
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

                var replyKeyboard = new ReplyKeyboardMarkup(new[]
                {
                [ "Настроить игру ⚙" ],
                new KeyboardButton[] { "Использовать предыдущие настройки ⬅" }
                })
                {
                    ResizeKeyboard = true
                };

                var prevGame = QuizSettings.IsSet ? QuizSettings?.ToString() : "Еще не было игр.";

                QuizMessage = await client.SendPhotoAsync(
                    chatId: chatId,
                    caption: "Выбрите настройки для игры:\n\n" + $"Предыдущие настройки:\n{prevGame}",
                    photo: InputFile.FromUri(QuizTitleImage),
                    replyMarkup: replyKeyboard
                );
            }
            else if (command.Equals("stop"))
            {
                Executor.StopListen();
                QuizIsStart = true;
                await client.SendTextMessageAsync(
                    chatId: chatId,
                    text: "Отмена игры!\n",
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

            if (command.Equals("Настроить игру"))
            {
                QuizSettings = new QuizSettings();
                CurrentSetting = QuizSettingsEnum.Theme;
                await SettingsCommand[CurrentSetting].Execute(update, client);
            }
            else if(command.Equals("")
            {

            }
            else if (!QuizSettings.IsSet)
            {
                await SettingsCommand[CurrentSetting].GetUpdate(update, client);
                if (SettingsCommand[CurrentSetting].IsSet)
                {
                    if (SettingsCommand[CurrentSetting].NextCommand != null)
                    {
                        CurrentSetting = SettingsCommand[CurrentSetting].NextCommand.Value;
                        await SettingsCommand[CurrentSetting].Execute(update, client);
                    }
                    else
                    {
                        QuizIsStart = true;
                        await client.SendTextMessageAsync(
                            chatId: chatId,
                            text: "Игра началась!\n" + QuizSettings,
                            replyMarkup: new ReplyKeyboardRemove()
                        );
                    }
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
    }
}
