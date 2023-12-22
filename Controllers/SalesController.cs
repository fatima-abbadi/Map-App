using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TestApiJwt.Models;

[ApiController]
[Route("api/[controller]")]
public class SaleController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;

    public SaleController(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Sale>>> GetSales()
    {
        return await _dbContext.Sales.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Sale>> GetSale(int id)
    {
        var sale = await _dbContext.Sales.FindAsync(id);

        if (sale == null)
        {
            return NotFound();
        }

        return sale;
    }

    [HttpPost]
    public async Task<ActionResult<Sale>> CreateSale(Sale sale)
    {
        _dbContext.Sales.Add(sale);
        await _dbContext.SaveChangesAsync();

        // Schedule cleanup for the newly created sale
        await ScheduleCleanup(sale);

        return CreatedAtAction(nameof(GetSale), new { id = sale.Id }, sale);
    }

    private async Task ScheduleCleanup(Sale sale)
    {
        DateTime cleanupTime = sale.StartDate.AddMinutes(sale.DurationInMinutes);

        // Ensure a positive delay (in case the sale duration is already finished)
        TimeSpan delay = cleanupTime - DateTime.UtcNow;
        delay = delay > TimeSpan.Zero ? delay : TimeSpan.Zero;

        // Schedule cleanup using a timer
        Timer cleanupTimer = new Timer(async _ =>
        {
            // Remove the sale from the database
            _dbContext.Sales.Remove(sale);
            await _dbContext.SaveChangesAsync();
        }, null, delay, Timeout.InfiniteTimeSpan);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateSale(int id, Sale sale)
    {
        if (id != sale.Id)
        {
            return BadRequest();
        }

        _dbContext.Entry(sale).State = EntityState.Modified;

        try
        {
            await _dbContext.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!SaleExists(id))
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

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSale(int id)
    {
        var sale = await _dbContext.Sales.FindAsync(id);
        if (sale == null)
        {
            return NotFound();
        }

        _dbContext.Sales.Remove(sale);
        await _dbContext.SaveChangesAsync();

        return NoContent();
    }

    private bool SaleExists(int id)
    {
        return _dbContext.Sales.Any(e => e.Id == id);
    }
}
