using BotForMushrooms.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace BotForMushrooms.Controllers
{
    [ApiController]
    [Route("/")]
    public class BotController : ControllerBase
    {
        [HttpPost]
        public async Task Post(Update update)
        {
            if(update.Message != null && update.Message.Text != null)
            {
                var bot = await Bot.Get();
                var message = update.Message;
                var text = message.Text;
                foreach(var command in Bot.Commands)
                {
                    if(command.Contains(text))
                    {
                        await command.Execute(message, bot);
                        break;
                    }
                }
            }
        }

        [HttpGet]
        public string Get()
        {
            return "Telegram bot was started";
        }
    }
}
