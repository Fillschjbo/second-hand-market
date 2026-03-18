namespace secondhand_market_assignment;

public class Transaction
{
    private static int _nextId = 1;
    
    public int id { get; }
    
    public User Buyer { get; }
    
    public User Seller { get; }
    
    public Listing Listing { get; }
    
    public decimal Price { get; }
    
    public DateTime Date { get; } = DateTime.Now;


    public Transaction(User buyer, User seller, Listing listing)
    {
        id = ++_nextId;
        Buyer = buyer;
        Seller = seller;
        Listing = listing;
        Price = listing.Price;
    }
}