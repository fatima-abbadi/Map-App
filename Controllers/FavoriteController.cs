using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using TestApiJwt.Models;

[Route("api/[controller]")]
[ApiController]
[Authorize] // Add this attribute to ensure the request is authenticated
public class FavoriteController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public FavoriteController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET api/favorite/user
    [HttpGet("user")]
    public IActionResult GetUserFavorites()
    {
        // Extract the user id from the token
        var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "uid")?.Value;

        if (string.IsNullOrEmpty(userId))
        {
            return BadRequest("User id not found in token.");
        }

        var favorites = _context.Favorites
            .Where(f => f.UserId == userId)
            .Include(f => f.Shop) // Include the Shop details if needed
            .ToList();

        return Ok(favorites);
    }

    // POST api/favorite
    [HttpPost]
    public IActionResult AddFavorite([FromBody] Favorite favorite)
    {
        // Extract the user id from the token
        var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "uid")?.Value;

        if (string.IsNullOrEmpty(userId))
        {
            return BadRequest("User id not found in token.");
        }

        if (_context.Favorites.Any(f => f.UserId == userId && f.ShopId == favorite.ShopId))
        {
            return BadRequest("Favorite already exists.");
        }

        favorite.UserId = userId; // Set the user id from the token
        _context.Favorites.Add(favorite);
        _context.SaveChanges();

        return Ok("Favorite added successfully.");
    }
    // DELETE api/favorite/{favoriteId}
    [HttpDelete("{favoriteId}")]
    public IActionResult DeleteFavorite(int favoriteId)
    {
        // Extract the user id from the token
        var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "uid")?.Value;

        if (string.IsNullOrEmpty(userId))
        {
            return BadRequest("User id not found in token.");
        }

        var favorite = _context.Favorites.Find(userId, favoriteId);

        if (favorite == null)
        {
            return NotFound("Favorite not found.");
        }

        // Ensure that the user making the request owns the favorite
        var requestingUserId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "uid")?.Value;
        if (favorite.UserId != requestingUserId)
        {
            return Unauthorized("You don't have permission to delete this favorite.");
        }

        _context.Favorites.Remove(favorite);
        _context.SaveChanges();

        return Ok("Favorite deleted successfully.");
    }
}