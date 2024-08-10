using Telegram.Bot.Types;

namespace BotForMushrooms.Models.Commands
{
    public abstract class TelegramUpdateListener
    {
        public Chat From { get; }

        protected HashSet<string> PollIds { get; } = [];

        protected TelegramUpdateListener(Chat from)
        {
            From = from;
        }

        public abstract Task GetUpdate(Update update);
        public bool IsPollContains(string pollId)
        {
            if (PollIds.Contains(pollId))
            {
                return true;
            }

            return false;
        }

        public void AddPoll(string pollId)
        {
            PollIds.Add(pollId);
        }

        public void RemovePoll(string pollId)
        {
            PollIds.Remove(pollId);
        }
    }
}
