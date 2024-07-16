using BotForMushrooms.Models;
using Microsoft.EntityFrameworkCore;

namespace BotForMushrooms.Repository
{
    public class FoodPlaceDbContext : DbContext
    {
        public FoodPlaceDbContext(DbContextOptions<FoodPlaceDbContext> options) : base(options) 
        { }

        public DbSet<FoodPlace> FoodPlaces { get; set; }
    }
}
