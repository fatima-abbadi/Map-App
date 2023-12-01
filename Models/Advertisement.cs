using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestApiJwt.Models
{
    public class Advertisement
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AdId { get; set; }
        public int ShopId { get; set; }
        public string AdDescription {get; set; }
        public Shop?  Shop { get; set; }  
    }
}
