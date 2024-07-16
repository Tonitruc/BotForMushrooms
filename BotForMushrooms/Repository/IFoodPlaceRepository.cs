using BotForMushrooms.Models;

namespace BotForMushrooms.Repository
{
    public interface IFoodPlaceRepository
    {
        IEnumerable<FoodPlace> Get();
        FoodPlace? Get(long id);
        void Create(FoodPlace item);
        void Update(FoodPlace item);
        FoodPlace? Delete(long id);
    }
}
