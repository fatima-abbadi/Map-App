using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestApiJwt.Models; // Replace with your actual namespace
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

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

    // GET: api/shops
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Shop>>> GetShops()
    {
        //.Include(x=> x.User) بس يلزم ترجعي الداتا اللي جوات اليوزر مثلا لو بدك تعرضي اسم اليوزر او الداتا اللي جواتو بتعملي هاي الحركة يعني ما بلزم دائما تعمليها
        var shops = await _context.Shops.ToListAsync();
        return Ok(shops);
    }

    // GET: api/shops/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Shop>> GetShop(int id)
    {
        var shop = await _context.Shops.FindAsync(id);

        if (shop == null)
        {
            return NotFound();
        }

        return Ok(shop);
    }
    [HttpGet("userShops")]
    [Authorize]
    public async Task<ActionResult<IEnumerable<Shop>>> GetUserShops()
    {
        // Get the currently authenticated user
        var userId = User.FindFirst("uid")?.Value;

        // Retrieve shops associated with the current user
        var shops = await _context.Shops
            .Where(s => s.UserId == userId)
            .ToListAsync();

        return Ok(shops);
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

        _context.Shops.Add(shop);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetShop", new { id = shop.ShopId }, shop);
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

    // DELETE: api/shops/5
    [HttpDelete("{id}")]
    //[Authorize] // Requires authorization (you can adjust the policy as needed)
    public async Task<ActionResult<Shop>> DeleteShop(int id)
    {
        var shop = await _context.Shops.FindAsync(id);
        if (shop == null)
        {
            return NotFound();
        }

        _context.Shops.Remove(shop);
        await _context.SaveChangesAsync();

        return Ok(shop);
    }

    private bool ShopExists(int id)
    {
        return _context.Shops.Any(e => e.ShopId == id);
    }
}

