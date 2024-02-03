using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TestApiJwt.Migrations;
using TestApiJwt.Models;

[ApiController]
[Route("api/[controller]")]
public class SaleController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IServiceProvider _serviceProvider;

    public SaleController(ApplicationDbContext dbContext, IServiceProvider serviceProvider)
    {
        _dbContext = dbContext;
        _serviceProvider = serviceProvider;
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
    [HttpGet("byshop/{shopId}")]
    public async Task<ActionResult<IEnumerable<Sale>>> GetSalesByShopId(int shopId)
    {
        var sales = await _dbContext.Sales
            .Where(s => s.ShopId == shopId)
            .ToListAsync();

        if (sales == null || !sales.Any())
        {
            return NotFound("No sales found for the specified shop ID.");
        }

        return Ok(sales);
    }
    [HttpPost]
    public async Task<ActionResult<Sale>> CreateSale(Sale sale)
    {
        // Set the StartDate to the current date and time
        sale.StartDate = DateTime.UtcNow;

        _dbContext.Sales.Add(sale);
        await _dbContext.SaveChangesAsync();

        // Update the shop status to 's' when the sale starts
        await UpdateShopStatus(sale.ShopId, Shop.ShopStatus.Yes, sale.DurationInMinutes);

        // Schedule cleanup for the newly created sale
        await ScheduleCleanup(sale);

        return CreatedAtAction(nameof(GetSale), new { id = sale.Id }, sale);
    }

    private async Task UpdateShopStatus(int shopId, Shop.ShopStatus status, int durationInMinutes)
    {
        var shop = await _dbContext.Shops.FindAsync(shopId);

        if (shop != null)
        {
            // Save the original status before updating
            var originalStatus = shop.Status;

            // Update the status
            shop.Status = status;
            await _dbContext.SaveChangesAsync();

            // Schedule a task to revert the status after the sale finishes
            await ScheduleRevertShopStatus(shopId, originalStatus, durationInMinutes);
        }
    }

    private async Task ScheduleRevertShopStatus(int shopId, Shop.ShopStatus originalStatus, int durationInMinutes)
    {
        // Calculate the time when the sale will finish
        DateTime saleEndTime = DateTime.UtcNow.AddMinutes(durationInMinutes);

        // Ensure a positive delay (in case the sale duration is already finished)
        TimeSpan delay = saleEndTime - DateTime.UtcNow;
        delay = delay > TimeSpan.Zero ? delay : TimeSpan.Zero;

        // Schedule a task to revert the shop status using Task.Delay
        await Task.Delay(delay);

        // Update the shop status back to the original status
        await UpdateShopStatus(shopId, originalStatus, 0); // Pass 0 if there's no meaningful duration for the revert
    }

    private async Task ScheduleCleanup(Sale sale)
    {
        DateTime cleanupTime = sale.StartDate.AddMinutes(sale.DurationInMinutes);

        // Ensure a positive delay (in case the sale duration is already finished)
        TimeSpan delay = cleanupTime - DateTime.UtcNow;
        delay = delay > TimeSpan.Zero ? delay : TimeSpan.Zero;

        await Task.Delay(delay);

        // Create a new scope and resolve a new instance of ApplicationDbContext
        using (var scope = _serviceProvider.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            // Remove the sale from the database using the new DbContext instance
            var storedSale = await dbContext.Sales.FindAsync(sale.Id);
            if (storedSale != null)
            {
                dbContext.Sales.Remove(storedSale);
                await dbContext.SaveChangesAsync();
            }

            // Update the shop status back to normal
            await UpdateShopStatus(storedSale.ShopId, Shop.ShopStatus.No, 0);
        }
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
