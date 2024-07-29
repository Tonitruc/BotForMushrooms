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
        public IEnumerable<FoodPlace> GetAll()
        {
            return FoodPlaceRepository.GetAll();
        }

        [HttpGet("{id}", Name = "Get")]
        public IActionResult Get(long Id)
        {
            FoodPlace? foodPlace = FoodPlaceRepository.Get(Id);

            if (foodPlace == null)
            {
                return NotFound();
            }

            return new ObjectResult(foodPlace);
        }

        [HttpPost]
        public IActionResult Create([FromBody] FoodPlace? foodPlace)
        {
            if (foodPlace == null)
            {
                return BadRequest();
            }    

            if(FoodPlaceRepository.Create(foodPlace) == null)
            {
                return BadRequest($"Food place with {foodPlace.Name} is exist.");
            }

            return CreatedAtRoute("Get", new { id = foodPlace.Id }, foodPlace);
        }

        [HttpPut("{id}", Name = "Update")]
        public IActionResult Update(long Id, [FromBody] FoodPlace? foodPlace)
        {
            if(foodPlace == null)
            {
                return BadRequest();
            }

            if(FoodPlaceRepository.Update(Id, foodPlace) == null)
            {
                return BadRequest($"Food place with {foodPlace.Id} is not exist.");
            }

            return CreatedAtRoute("Get", new { id = Id }, foodPlace);
        }

        [HttpDelete("id/{id:long}", Name = "DeleteById")]
        public IActionResult DeleteById(long id)
        {
            if (!FoodPlaceRepository.Delete(id))
            {
                return NotFound();
            }

            return Ok();
        }

        [HttpDelete("name/{foodPlaceName}", Name = "DeleteByName")]
        public IActionResult DeleteByName(string foodPlaceName)
        {
            if (!FoodPlaceRepository.DeleteByName(foodPlaceName))
            {
                return NotFound();
            }

            return Ok();
        }
    }
}
