using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestApiJwt.Models
{
    public class CartItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CartItemId { get; set; }

        public int CartId { get; set; } // Foreign Key to Carts Table
        public Cart? Cart { get; set; }

        public int ProductId { get; set; } // Foreign Key to Products Table
        public Product? Product { get; set; }

        public int Quantity { get; set; }

        public string UserId { get; set; }
        public ApplicationUser? User { get; set; }

        //[NotMapped]
        //public decimal TotalItemPrice
        //{
        //    get
        //    {
        //        if (Product != null)
        //        {
        //            return Quantity * Product.ProductPrice;
        //        }
        //        return 0; // or handle the case where Product is null
        //    }

    }
}
