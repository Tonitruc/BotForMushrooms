﻿using System.Windows.Input;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace BotForMushrooms.Models.Commands
{
    public class CommandExecutor : ITelegramUpdateListener
    {
        private readonly TelegramBotClient client = Bot.Get().Result;

        private static readonly List<Command> commandsList = [];

        public static IReadOnlyList<Command> Commands => commandsList.AsReadOnly();

        public CommandExecutor()
        {
            commandsList.Add(new MustardBoyCommand());
        }

        public async Task GetUpdate(Update update)
        {
            if (update.Message == null || update.Message.Text == null)
                return;

            var message = update.Message;
            var text = message.Text;

            foreach (var command in Commands)
            {
                if (command.Contains(text))
                {
                    await command.Execute(message, client);
                    break;
                }
            }
        }
    }
}