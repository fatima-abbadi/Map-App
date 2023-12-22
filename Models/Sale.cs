using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestApiJwt.Models
{
    public class Sale
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int ShopId;
        public Shop? shop { get; set; }

        public double salePercentage { get; set; }
        public DateTime StartDate { get; set; }
        public int DurationInMinutes { get; set; }
    }
}
