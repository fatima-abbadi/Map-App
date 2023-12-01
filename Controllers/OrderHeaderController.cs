using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestApiJwt.Migrations;
using TestApiJwt.Models;

namespace TestApiJwt.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderHeaderController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public OrderHeaderController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/OrderHeader
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderHeader>>> GetOrderHeaders()
        {
            return await _context.OrderHeaders.ToListAsync();
        }

        // GET: api/OrderHeader/5
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderHeader>> GetOrderHeader(int id)
        {
            var orderHeader = await _context.OrderHeaders.FindAsync(id);

            if (orderHeader == null)
            {
                return NotFound();
            }

            return orderHeader;
        }

        // POST: api/OrderHeader
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<OrderHeader>> PostOrderHeader(OrderHeader orderHeader)
        {
            var userId = User.FindFirst("uid")?.Value;

            // Assign the user's ID to the shop
            orderHeader.UserId = userId;

            _context.OrderHeaders.Add(orderHeader);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetOrderHeader", new { id = orderHeader.Id }, orderHeader);
        }

        // PUT: api/OrderHeader/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrderHeader(int id, OrderHeader orderHeader)
        {
            if (id != orderHeader.Id)
            {
                return BadRequest();
            }

            _context.Entry(orderHeader).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderHeaderExists(id))
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

        // DELETE: api/OrderHeader/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrderHeader(int id)
        {
            var orderHeader = await _context.OrderHeaders.FindAsync(id);
            if (orderHeader == null)
            {
                return NotFound();
            }

            _context.OrderHeaders.Remove(orderHeader);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool OrderHeaderExists(int id)
        {
            return _context.OrderHeaders.Any(e => e.Id == id);
        }
    }
}
