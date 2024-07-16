using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BotForMushrooms.Models
{
    public class FoodPlace
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public string Name { get; set; } = "Unknown";

        public double Rating { get; } = 0;

        public FoodPlace() { }

        public FoodPlace(string name)
        {
            Name = name;
        }
    }
}
