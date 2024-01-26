using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestApiJwt.Models;

namespace TestApiJwt.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CartItemsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CartItemsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/CartItems
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CartItem>>> GetCartItems()
        {
            return await _context.CartItems.ToListAsync();
        }

        // GET: api/CartItems/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CartItem>> GetCartItem(int id)
        {
            var cartItem = await _context.CartItems.FindAsync(id);

            if (cartItem == null)
            {
                return NotFound();
            }

            return cartItem;
        }
        [HttpGet("ByCartId/{cartId}")]
        public async Task<ActionResult<IEnumerable<CartItem>>> GetCartItemsByCartId(int cartId)
        {
            // Get the user ID from the token
            var userId = User.FindFirst("uid")?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                // User ID not found in the token
                return BadRequest("User ID not found in the token.");
            }

            // Retrieve cart items based on the cartId and userId
            var cartItems = await _context.CartItems
                .Include(ci => ci.Product) // Include the related Product
                .Where(ci => ci.CartId == cartId && ci.UserId == userId)
                .ToListAsync();

            return cartItems;
        }

        // PUT: api/CartItems/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCartItem(int id, CartItem cartItem)
        {
            if (id != cartItem.CartItemId)
            {
                return BadRequest();
            }

            _context.Entry(cartItem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CartItemExists(id))
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

        // POST: api/CartItems
        [HttpPost]
        public async Task<ActionResult<CartItem>> PostCartItem(CartItem cartItem)
        {
            // Get the user ID from the token
            var userId = User.FindFirst("uid")?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                // User ID not found in the token
                return BadRequest("User ID not found in the token.");
            }

            // Assign the user ID to the cart item
            cartItem.UserId = userId;

            _context.CartItems.Add(cartItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCartItem", new { id = cartItem.CartItemId }, cartItem);
        }
    


// DELETE: api/CartItems/5
[HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCartItem(int id)
        {
            var cartItem = await _context.CartItems.FindAsync(id);
            if (cartItem == null)
            {
                return NotFound();
            }

            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CartItemExists(int id)
        {
            return _context.CartItems.Any(e => e.CartItemId == id);
        }
    }
}
