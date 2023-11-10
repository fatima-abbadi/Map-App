using System.ComponentModel.DataAnnotations;

namespace TestApiJwt.Models
{
    public class TokenRequestModel
    {
        [EmailAddress]
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
