using BotForMushrooms.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Telegram.Bot;
using Telegram.Bot.Types;
using BotForMushrooms.Models.ChatListeners;
using BotForMushrooms.Models.Commands;

namespace BotForMushrooms.Controllers
{
    [ApiController]
    [Route("/")]
    public class BotController : ControllerBase
    {
        private static readonly TelegramBotClient bot = Bot.Get().Result;
        private static readonly UpdateDistributor<ChatUpdater> updateDistributor = new UpdateDistributor<ChatUpdater>();

        [HttpPost]
        public async Task Post(Update update)
        {
            await updateDistributor.GetUpdate(update);
        }

        [HttpGet]
        public string Get()
        {
            return "Telegram bot was started";
        }
    }
}
