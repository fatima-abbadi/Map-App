using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestApiJwt.Models
{
    public class Category
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CategoryId { get; set; }

        public string CategoryName { get; set; }
        public string CategoryDescription { get; set; }

        // Navigation property for products in this category
        public int ShopId { get; set; }
        public Shop? Shop { get; set; }

        public ICollection<Product>? Products { get; set; }
    }
}
