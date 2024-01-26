using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using TestApiJwt.Models;

namespace TestApiJwt.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CartController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Cart/1
        [HttpGet("{id}")]
        public async Task<ActionResult<Cart>> GetCart(int id)
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems) // Include CartItems
                .FirstOrDefaultAsync(c => c.CartId == id);

            if (cart == null)
            {
                return NotFound();
            }

            return cart;
        }

        // POST: api/Cart
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Cart>> PostCart(Cart cart)
        {
            // Set the order date to the current UTC time
            cart.OrderDate = DateTime.UtcNow;

            var userId = User.FindFirst("uid")?.Value;

            // Assign the user's ID to the shop
          //  cart.UserId = userId;

            // Add the cart to the context
            _context.Carts.Add(cart);

            // Save changes to the database
            await _context.SaveChangesAsync();

            // Return the created cart
            return CreatedAtAction("GetCart", new { id = cart.CartId }, cart);
        }

        // PUT: api/Cart/1
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCart(int id, Cart cart)
        {
            if (id != cart.CartId)
            {
                return BadRequest();
            }

            _context.Entry(cart).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CartExists(id))
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

        // DELETE: api/Cart/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCart(int id)
        {
            var cart = await _context.Carts.FindAsync(id);
            if (cart == null)
            {
                return NotFound();
            }

            _context.Carts.Remove(cart);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CartExists(int id)
        {
            return _context.Carts.Any(e => e.CartId == id);
        }
    }
}
