using BotForMushrooms.Repository;
using BotForMushrooms.Repository.Implements;
using Microsoft.EntityFrameworkCore;

namespace BotForMushrooms.Extensions
{
    public static class DbContextRegistrar
    {
        private const string ConnectionStringName = "ConnectionStrings:DefaultConnection";

        public static IServiceCollection AddDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<FoodPlaceDbContext>(options => options.UseSqlServer(configuration[ConnectionStringName]));
            services.AddTransient<IFoodPlaceRepository, FoodPlaceRepository>();

            services.AddDbContext<BanQuestionDbContext>(options => options.UseSqlServer(configuration[ConnectionStringName]));
            services.AddTransient<IBanQuestionRepository, BanQuestionRepository>();

            return services;
        }
    }
}
