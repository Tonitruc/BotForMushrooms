using BotForMushrooms.Models;

namespace BotForMushrooms.Repository
{
    public interface IFoodPlaceRepository
    {
        IEnumerable<FoodPlace> GetAll();
        FoodPlace? Get(long id);
        FoodPlace? GetByName(string name);
        FoodPlace? Create(FoodPlace foodPlace);
        FoodPlace? Update(long id, FoodPlace updateFoodPlace);
        bool Delete(long id);
        bool DeleteByName(string name);
    }
}
