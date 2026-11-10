using Inventory.Api.Data;
using Inventory.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly AppDbContext  _context;
  
    public ProductsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
    {
        return await _context.Products.ToListAsync();
    }

    [HttpPost]
    public async Task<ActionResult<Product>> AddProduct(Product product)
    {
        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetProducts), new { id = product.Id}, product);
    }
    
    [HttpPost("{id}/purchase")]
    public async Task<IActionResult> PurchaseProduct(int id, int quantity)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null) return NotFound();

        // BUG: Race condition. No transaction or concurrency token used.
        product.StockQty -= quantity; 
        
        await _context.SaveChangesAsync();
        return Ok(product);
    }

}