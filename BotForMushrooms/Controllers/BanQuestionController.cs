using BotForMushrooms.Models;
using BotForMushrooms.Models.Commands.GlobalCommands.Quiz.Implements;
using BotForMushrooms.Repository;
using Microsoft.AspNetCore.Mvc;

namespace BotForMushrooms.Controllers
{
    [Route("api/telegram_quiz/[controller]")]
    public class BanQuestionController : Controller
    {
        IBanQuestionRepository BanQuestionRepository;

        public BanQuestionController(IBanQuestionRepository todoRepository)
        {
            BanQuestionRepository = todoRepository;
        }

        [HttpGet(Name = "GetAllBanQuestion")]
        public IEnumerable<BanQuestion> GetAll()
        {
            return BanQuestionRepository.GetAll();
        }

        [HttpGet("{id}", Name = "GetBanQuestion")]
        public IActionResult Get(long Id)
        {
            BanQuestion? foodPlace = BanQuestionRepository.Get(Id);

            if (foodPlace == null)
            {
                return NotFound();
            }

            return new ObjectResult(foodPlace);
        }

        [HttpPost]
        public IActionResult Add([FromBody] BanQuestion? banQuestion)
        {
            if(banQuestion == null)
            {
                return BadRequest();
            }

            BanQuestionRepository.Add(banQuestion);

            return CreatedAtRoute("Get", new { id = banQuestion.Id }, banQuestion);
        }

        [HttpDelete("id/{id:long}", Name = "DeleteBanQuestionById")]
        public IActionResult DeleteById(long id)
        {
            if (!BanQuestionRepository.Delete(id))
            {
                return NotFound();
            }

            return Ok();
        }

        [HttpDelete("name/{banQuestionText}", Name = "DeleteBanQuestionByName")]
        public IActionResult DeleteByName(string banQuestionText)
        {
            if (!BanQuestionRepository.DeleteByQuestionText(banQuestionText))
            {
                return NotFound();
            }

            return Ok();
        }
    }
}
