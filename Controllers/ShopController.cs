using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestApiJwt.Models; // Replace with your actual namespace
using Microsoft.AspNetCore.Authorization;

[Route("api/shops")]
[ApiController]
public class ShopController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ShopController(ApplicationDbContext context)
    {
        _context = context;
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

    // POST: api/shops
    [HttpPost]
    //[Authorize] // Requires authorization (you can adjust the policy as needed)
    public async Task<ActionResult<Shop>> PostShop(Shop shop)
    {
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

        return NoContent();
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

