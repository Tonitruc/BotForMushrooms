using BotForMushrooms.Models;

namespace BotForMushrooms.Repository
{
    public class FoodPlaceRepository : IFoodPlaceRepository
    {
        private FoodPlaceDbContext Context;

        public FoodPlaceRepository(FoodPlaceDbContext context)
        {
            Context = context;
        }

        public IEnumerable<FoodPlace> Get()
        {
            return Context.FoodPlaces;
        }

        public FoodPlace? Get(long Id)
        {
            return Context.FoodPlaces.Find(Id);
        }


        public void Create(FoodPlace item)
        {
            Context.FoodPlaces.Add(item);
            Context.SaveChanges();
        }

        public void Update(FoodPlace updateFoodPlace)
        {
            FoodPlace? currentItem = Get(updateFoodPlace.Id);
            currentItem.Name = updateFoodPlace.Name;

            Context.FoodPlaces.Update(currentItem);
            Context.SaveChanges();
        }

        public FoodPlace Delete(long Id)
        {
            FoodPlace? todoItem = Get(Id);

            if (todoItem != null)
            {
                Context.FoodPlaces.Remove(todoItem);
                Context.SaveChanges();
            }

            return todoItem;
        }
    }
}
