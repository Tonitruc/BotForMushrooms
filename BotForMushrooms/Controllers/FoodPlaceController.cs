using BotForMushrooms.Models;
using BotForMushrooms.Repository;
using Microsoft.AspNetCore.Mvc;

namespace BotForMushrooms.Controllers
{
    [Route("api/[controller]")]
    public class FoodPlaceController : Controller
    {
        IFoodPlaceRepository FoodPlaceRepository;

        public FoodPlaceController(IFoodPlaceRepository todoRepository)
        {
            FoodPlaceRepository = todoRepository;
        }

        [HttpGet(Name = "GetAll")]
        public IEnumerable<FoodPlace> Get()
        {
            return FoodPlaceRepository.Get();
        }

        [HttpGet("{id}", Name = "Get")]
        public IActionResult Get(long Id)
        {
            FoodPlace? todoItem = FoodPlaceRepository.Get(Id);

            if (todoItem == null)
            {
                return NotFound();
            }

            return new ObjectResult(todoItem);
        }

        [HttpPost]
        public IActionResult Create([FromBody] FoodPlace todoItem)
        {
            if (todoItem == null)
            {
                return BadRequest();
            }
            FoodPlaceRepository.Create(todoItem);
            return CreatedAtRoute("Get", new { id = todoItem.Id }, todoItem);
        }
    }
}
