using BotForMushrooms.Models;

namespace BotForMushrooms.Repository.Implements
{
    public class FoodPlaceRepository : IFoodPlaceRepository
    {
        private FoodPlaceDbContext Context;

        public FoodPlaceRepository(FoodPlaceDbContext context)
        {
            Context = context;
        }

        public IEnumerable<FoodPlace> GetAll()
        {
            return Context.FoodPlaces;
        }

        public FoodPlace? Get(long Id)
        {
            return Context.FoodPlaces.Find(Id);
        }

        public FoodPlace? GetByName(string name)
        {
            return Context.FoodPlaces.FirstOrDefault(x => x.Name == name);
        }

        public FoodPlace? Create(FoodPlace foodPlace)
        {
            if(GetByName(foodPlace.Name) != null)
            {
                return null;
            }

            Context.FoodPlaces.Add(foodPlace);
            Context.SaveChanges();
            return foodPlace;
        }

        public FoodPlace? Update(long id, FoodPlace updateFoodPlace)
        {
            FoodPlace? existFoodPlace = Get(id);

            if(existFoodPlace == null)
            {
                return null;
            }

            existFoodPlace.Id = id;
            updateFoodPlace.Id = id;
            existFoodPlace.Name = updateFoodPlace.Name;

            Context.FoodPlaces.Update(existFoodPlace);
            Context.SaveChanges();

            return existFoodPlace;
        }

        public bool Delete(long Id)
        {
            FoodPlace? existFoodPlace = Get(Id);

            if (existFoodPlace != null)
            {
                Context.FoodPlaces.Remove(existFoodPlace);
                Context.SaveChanges();
                return true;
            }

            return false;
        }

        public bool DeleteByName(string name)
        {
            FoodPlace? existFoodPlace = GetByName(name);

            return Delete(existFoodPlace?.Id ?? -1);
        }
    }
}
