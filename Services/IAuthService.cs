using TestApiJwt.Models;

namespace TestApiJwt.Services
{
    // defines the contract for services that handle user registration, authentication, authorization, and token management.
    public interface IAuthService
    {

        //take register model as a aprameter and return auth model after successful registration.
        Task<AuthModel> RegisterAsync(RegisterModel model);

        //take TokenRequestModel as a parameter and return authmodelafter successful authentication.
        //login
        Task<AuthModel> GetTokenAsync(TokenRequestModel model);

        //add a rule to the user, take addRoleModel as parameter and return a string with the role
        Task<string> AddRoleAsync(AddRoleModel model);

        //request a new authantication token by take\ing  string (existing token) as a parameter and return auth model (new athantication token)
        Task<AuthModel> RefreshTokenAsync(string token);

        //revoke an existing token, take a string(token) as a parametera and return a boolean value weather it was successful or not
        Task<bool> RevokeTokenAsync(string token);
    }
}
