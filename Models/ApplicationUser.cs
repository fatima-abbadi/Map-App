using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace TestApiJwt.Models
{
    public class ApplicationUser : IdentityUser
    {
        [MaxLength(50)]
        public string FirstName { get; set; }

        [MaxLength(50)]
        public string LastName { get; set; }

        public List<RefreshToken>? RefreshTokens { get; set; }
        public ICollection<Shop> Shops { get; set; }
        public ICollection<Favorite>? Favorites { get; set; }
        public ICollection<OrderHeader>? OrderHeaders { get; set; }
        public ICollection<Rating>? Rates { get; set; }
        public ICollection<CartItem>? CartItems { get; set; }
    }
}
