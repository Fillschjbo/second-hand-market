namespace secondhand_market_assignment;

public class Listing
{
    private static int _nextId = 1;
    
    public int Id { get; }
    
    public string Title { get; set; }
    
    public string Description { get; set; }
    
    public Category Category { get; set; }
    
    public Condition Condition { get; set; }
    
    public decimal Price { get; set; }
    
    public User Seller { get; set; }
    
    public ListingStatus Status { get; set; }
    
    public DateTime CreatedAt { get; } = DateTime.Now;

    public Listing(string title, string description, Category category, Condition condition, decimal price, User seller)
    { 
        Id = _nextId++;
        Title = title;
        Description = description;
        Category = category;
        Condition = condition;
        Price = price;
        Seller = seller;
    }
}
