using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestApiJwt.Migrations;
using TestApiJwt.Models;

namespace TestApiJwt.Controllers
{
    [Route("api/products")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/products
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            var products = await _context.Products.Include(x => x.Category).ToListAsync();
            return products;
        }

        // GET: api/products/images/5
        [HttpGet("images/{id}")]
        public async Task<ActionResult<string>> GetProductImage(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null || string.IsNullOrEmpty(product.ProductImage))
            {
                return NotFound("Product image not found.");
            }

            // Assuming the images are stored in the "images" folder
            var imagePath = Path.Combine("images", product.ProductImage);

            // Construct the full URL path
            var imageUrl = $"{Request.Scheme}://{Request.Host}/{imagePath}";

            return imageUrl;
        }

        // GET: api/products/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            return product;
        }

        // GET: api/products/byCategory/5
        [HttpGet("byCategory/{categoryId}")]
        public async Task<ActionResult<IEnumerable<Product>>> GetProductsByCategory(int categoryId)
        {
            var products = await _context.Products
           .Where(p => p.CategoryId == categoryId)
           .ToListAsync();

            if (products == null || !products.Any())
            {
                return NotFound("No products found for the specified category.");
            }

            return products;
        }


        [HttpPost]

        public async Task<ActionResult<Product>> PostProduct([FromForm] Product product)
        {
            // Check ModelState before any modifications to the data model
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                if (product.PhotoFile != null && product.PhotoFile.Length > 0)
                {
                    var imagesFolder = Path.Combine(Directory.GetCurrentDirectory(), "images");

                    // Create the "images" folder if it doesn't exist
                    if (!Directory.Exists(imagesFolder))
                    {
                        Directory.CreateDirectory(imagesFolder);
                    }

                    // Generate a unique filename for the uploaded file
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(product.PhotoFile.FileName);
                    var filePath = Path.Combine(imagesFolder, fileName);

                    // Save the file to the server
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await product.PhotoFile.CopyToAsync(stream);
                    }

                    // Update the product's image property with the filename
                    product.ProductImage = fileName;
                }

                // Add the product to the database
                _context.Products.Add(product);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetProduct", new { id = product.ProductId }, product);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                return StatusCode(500, "Internal Server Error");
            }
        }



        // PUT: api/products/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(int id, Product product)
        {
            if (id != product.ProductId)
            {
                return BadRequest();
            }

            _context.Entry(product).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
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

        // DELETE: api/products/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Product>> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return product;
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }
    }

}
