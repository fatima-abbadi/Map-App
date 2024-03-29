﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestApiJwt.Models; // Replace with your actual namespace
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.ML;

[Route("api/shops")]
[ApiController]
public class ShopController : ControllerBase
{
    private readonly ApplicationDbContext _context; // Replace YourDbContext with your actual DbContext.
    private readonly UserManager<ApplicationUser> _userManager;// Inject your DbContext into the controller.

    public ShopController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    //get by geogrpahical bounds
    [HttpGet("byBounds")]
    public async Task<ActionResult<IEnumerable<Shop>>> GetShopsByBoundsAndApproval(double northEastLat, double northEastLng, double southWestLat, double southWestLng, bool isApproved)
    {
        // Filter shops within the provided map bounds (coordinates) and with the specified approval status
        var shopsFiltered = await _context.Shops
            .Where(s => s.ShopLocationLatitude <= northEastLat && s.ShopLocationLongitude <= northEastLng && s.ShopLocationLatitude >= southWestLat && s.ShopLocationLongitude >= southWestLng && s.IsApproved == isApproved)
            .ToListAsync();

        return Ok(shopsFiltered);
    }



    // GET: api/shops
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Shop>>> GetShops()
    {
        //.Include(x=> x.User) بس يلزم ترجعي الداتا اللي جوات اليوزر مثلا لو بدك تعرضي اسم اليوزر او الداتا اللي جواتو بتعملي هاي الحركة يعني ما بلزم دائما تعمليها
        var shops = await _context.Shops
         .Where(s => s.IsApproved)
         .ToListAsync();

        return Ok(shops);
    }



    [HttpGet("{id}")]
    public async Task<ActionResult<Shop>> GetShop(int id)
    {
        var shop = await _context.Shops
                                .Include(s => s.Cart) // Include the Cart object
                                .FirstOrDefaultAsync(s => s.ShopId == id);

        if (shop == null)
        {
            return NotFound();
        }

        // Assuming CartId is a property of the Cart class
        var cartId = shop.Cart?.CartId;

        return Ok(new { Shop = shop, CartId = cartId });
    }
    //get :api/shops/userShops
    [HttpGet("userShops")]
    [Authorize]
    public async Task<ActionResult<IEnumerable<Shop>>> GetUserShops()
    {
        // Get the currently authenticated user
        var userId = User.FindFirst("uid")?.Value;

        // Retrieve shops associated with the current user with approved status
        var shops = await _context.Shops
            .Where(s => s.UserId == userId && s.IsApproved)
            .ToListAsync();

        return Ok(shops);
    }
    // GET: api/shops/admin/pending
    [HttpGet("admin/pending")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<IEnumerable<Shop>>> GetPendingShops()
    {
        var pendingShops = await _context.Shops
            .Where(s => !s.IsApproved)
            .ToListAsync();

        return Ok(pendingShops);
    }

  


// PUT: api/shops/admin/reject/5
[HttpPut("admin/reject/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> RejectShop(int id)
    {
        var shop = await _context.Shops.FindAsync(id);

        if (shop == null)
        {
            return NotFound();
        }

        // Optionally, you may want to perform additional actions before rejecting the shop

        _context.Shops.Remove(shop);
        await _context.SaveChangesAsync();

        return Ok(new { Message = "Shop rejected ." });
    }


    [HttpGet("userShops/{shopId}")]
    [Authorize]
    public async Task<ActionResult<Shop>> GetUserShopById(int shopId)
    {
        // Get the currently authenticated user
        var userId = User.FindFirst("uid")?.Value;

        // Retrieve the specific shop associated with the current user and the specified shop ID
        var specificShop = await _context.Shops
            .Where(s => s.UserId == userId && s.ShopId == shopId)
            .SingleOrDefaultAsync();

        if (specificShop == null)
        {
            return NotFound(); // Return 404 if the specific shop is not found
        }

        return Ok(specificShop);
    }



    // POST: api/shops
    [HttpPost]
    [Authorize]
    public async Task<ActionResult<Shop>> PostShop(Shop shop)
    {
        // Get the currently authenticated user
        var userId = User.FindFirst("uid")?.Value;

        // Assign the user's ID to the shop
        shop.UserId = userId;

        // Set the default approval status to false
        shop.IsApproved = false;

        // Create a new cart for the shop
        var cart = new Cart();
        shop.Cart = cart; // Associate the cart with the shop

        _context.Shops.Add(shop);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetShop", new { id = shop.ShopId }, new { Message = "Shop request submitted. Awaiting approval." });
    }


    [HttpPut("admin/approve/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ApproveShop(int id)
    {
        var shop = await _context.Shops.FindAsync(id);

        if (shop == null)
        {
            return NotFound();
        }

        shop.IsApproved = true;

        await _context.SaveChangesAsync();

        return Ok(new { Message = "Shop approved successfully." });
    }
    // PUT: api/shops/5
    [HttpPut("{id}")]
    // [Authorize] // Requires authorization (you can adjust the policy as needed)
    public async Task<IActionResult> PutShop(int id, Shop shop)
    {
        if (id != shop.ShopId)
        {
            return BadRequest();
        }

        _context.Entry(shop).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ShopExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return Ok();
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<ActionResult<Shop>> DeleteShop(int id)
    {
        var shop = await _context.Shops.FindAsync(id);

        if (shop == null)
        {
            return NotFound();
        }

        // Get the user ID from the token
        var userId = User.FindFirst("uid")?.Value;

        // Check if the user making the request is the owner of the shop
        if (shop.UserId != userId)
        {
            // Unauthorized, the user is not the owner of the shop
            return Forbid();
        }

        // Manually delete associated favorites
        var associatedFavorites = _context.Favorites.Where(f => f.ShopId == id);
        _context.Favorites.RemoveRange(associatedFavorites);

        // Now, delete the shop
        _context.Shops.Remove(shop);
        await _context.SaveChangesAsync();

        return Ok(shop);
    }

    // New action to get shops with Status 'n'
    [HttpGet("status-n")]
    public async Task<ActionResult<IEnumerable<Shop>>> GetShopsWithStatusN()
    {
        var shopsWithStatusN = await _context.Shops
            .Where(s => s.Status == Shop.ShopStatus.No)
            .ToListAsync();

        return Ok(shopsWithStatusN);
    }

    // New action to get shops with Status 's'
    [HttpGet("status-s")]
    public async Task<ActionResult<IEnumerable<Shop>>> GetShopsWithStatusS()
    {
        var shopsWithStatusS = await _context.Shops
            .Where(s => s.Status == Shop.ShopStatus.Yes)
            .ToListAsync();

        return Ok(shopsWithStatusS);
    }

    private bool ShopExists(int id)
    {
        return _context.Shops.Any(e => e.ShopId == id);
    }

}

