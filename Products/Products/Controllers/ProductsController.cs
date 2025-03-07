using Microsoft.AspNetCore.Mvc;

namespace Products.Controllers;

/// <summary>
/// Products controller.
/// </summary>
/// <param name="logger">Debug logger.</param>
[ApiController]
[Route("[controller]")]
public class ProductsController(ILogger<ProductsController> logger) : ControllerBase
{
    private static readonly List<Product> Products =
    [
        new(1, "Coca Cola", "A nice bubbly drink", 1.5),
        new(2, "Snickers", "A tasty chocolate candy", 2.5),
        new(3, "Loaf of Bread", "Warm and crispy", 1)
    ];

    /// <summary>
    /// Create a product.
    /// </summary>
    /// <param name="product">Product to be created.</param>
    /// <returns>Created product.</returns>
    [HttpPost]
    public IActionResult Post(Product product)
    {
        logger.LogInformation($"Creating the product {product.Id}");
        var found = Products.FirstOrDefault(x => x.Id == product.Id);
        if (found != null)
        {
            logger.LogError($"Product {product.Id} already exists");
            return BadRequest("Product already exists");
        }
        
        Products.Add(product);
        return Ok(product);
    }
    
    /// <summary>
    /// Read the products list.
    /// </summary>
    /// <returns>List of available products.</returns>
    [HttpGet]
    public IActionResult Get()
    {
        logger.LogInformation("Getting the list of products");
        return Ok(Products);
    }

    /// <summary>
    /// Read the product by Id.
    /// </summary>
    /// <param name="id">Product's identifier.</param>
    /// <returns>Found product.</returns>
    [HttpGet("{id}")]
    public IActionResult Get(int id)
    {
        logger.LogInformation($"Getting the product {id}");
        var found = Products.FirstOrDefault(x => x.Id == id);
        if (found == null)
        {
            logger.LogError($"Product {id} not found");
            NotFound("Product not found");
        }
        
        return Ok(found);
    }
    
    /// <summary>
    /// Update the product.
    /// </summary>
    /// <param name="product">Product to be updated.</param>
    /// <returns>Updated product.</returns>
    [HttpPut]
    public IActionResult Put(Product product)
    {
        logger.LogInformation($"Updating the product {product.Id}");
        var found = Products.FirstOrDefault(x => x.Id == product.Id);
        if (found == null)
        {
            logger.LogError($"Product {product.Id} not found");
            return NotFound("Product not found");
        }
        
        found.Name = product.Name;
        found.Description = product.Description;
        found.Price = product.Price;
        
        return Ok(found);
    }
    
    /// <summary>
    /// Delete the product.
    /// </summary>
    /// <param name="id">Product identifier.</param>
    /// <returns>Deleted product.</returns>
    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        logger.LogInformation($"Deleting the product {id}");
        var found = Products.FirstOrDefault(x => x.Id == id);
        if (found == null)
        {
            logger.LogError($"Product {id} not found");
            return NotFound("Product not found");
        }
        
        Products.Remove(found);
        return Ok(found);
    }
}