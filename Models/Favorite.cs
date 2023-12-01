using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestApiJwt.Models
{
    public class Favorite
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int FavoriteId { get; set; }

        // Foreign key to link a favorite to a user
        public string UserId { get; set; }
        public ApplicationUser? User { get; set; }

        // Foreign key to link a favorite to a shop
        public int ShopId { get; set; }
        public Shop? Shop { get; set; }
    }
}
