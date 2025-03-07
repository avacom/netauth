namespace Products;

/// <summary>
/// Product model.
/// </summary>
public class Product
{
    /// <summary>
    /// Create a new product.
    /// </summary>
    /// <param name="id">Unique identifier.</param>
    /// <param name="name">Product name.</param>
    /// <param name="description">Product description.</param>
    /// <param name="price">Product price.</param>
    public Product(int id, string name, string description, double price)
    {
        Id = id;
        Name = name;
        Description = description;
        Price = price;
    }

    /// <summary>
    /// Gets the product identifier.
    /// </summary>
    public int Id { get; }
    
    /// <summary>
    /// Gets or sets the product name.
    /// </summary>
    public string Name { get; set; }
    
    /// <summary>
    /// Gets or sets the product description.
    /// </summary>
    public string Description { get; set; }
    
    /// <summary>
    /// Gets or sets the product price.
    /// </summary>
    public double Price { get; set; }
}