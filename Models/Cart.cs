using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestApiJwt.Models
{
    public class Cart
    {
        public Cart()//delete 
        {
            OrderDate = DateTime.UtcNow; // Set the default value to the current UTC time
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CartId { get; set; }
        public String UserId { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime? OrderDate { get; set; }//in order header 

        public ApplicationUser? User { get; set; }
     
    }
}
