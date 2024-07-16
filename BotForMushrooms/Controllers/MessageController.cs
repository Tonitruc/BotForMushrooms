using BotForMushrooms.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Telegram.Bot;
using Telegram.Bot.Types;
using BotForMushrooms.Models.Commands;

namespace BotForMushrooms.Controllers
{
    [ApiController]
    [Route("/")]
    public class BotController : ControllerBase
    {
        private TelegramBotClient bot = Bot.Get().Result;
        private UpdateDistributor<CommandExecutor> updateDistributor = new UpdateDistributor<CommandExecutor>();

        [HttpPost]
        public async Task Post(Update update)
        {
            if (update.Message == null) 
                return;

            await updateDistributor.GetUpdate(update);
        }

        [HttpGet]
        public string Get()
        {
            return "Telegram bot was started";
        }
    }
}
