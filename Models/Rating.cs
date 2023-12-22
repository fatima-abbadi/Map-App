
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;


namespace TestApiJwt.Models
{
    public class Rating
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public String UserId; // User ID
        public ApplicationUser? User { get; set; }

        public int ShopId;
        public Shop? shop { get; set; }
        private int _rate;

        public int Rate
        {
            get => _rate;
            set => _rate = (value < 1) ? 1 : (value > 5) ? 5 : value; // Constrain the rating to the range of 1 to 5
        }//the value of rate just from 1 to 5 
    }
}
