namespace secondhand_market_assignment;

public class Marketplace
{
    //user management
    private readonly List<User> _users = new ();
    private readonly List<Listing> _listings = new();
    private readonly List<Transaction> _transactions = new();

    public User Register(string username, string password)
    {
        if (_users.Any(u => u.Username == username))
            throw new InvalidOperationException($"Username '{username}' is aleready taken. Please enter a new one");
        
        var user = new User(username, password);
        _users.Add(user);
        return user;
    }

    public User Login(string username, string password)
    {
        var user = _users.FirstOrDefault(u => u.Username == username);

        if (user == null || !user.CheckPassword(password))
            throw new InvalidOperationException("invalid username or password");
        
        return user;
    }
    
    //listing management

    public Listing CreateListing(User seller, string title, string description, Category category, Condition condition, decimal price)
    {
        var listing = new Listing(title, description, category, condition, price, seller);
        seller.Listings.Add(listing);
        _listings.Add(listing);
        return listing;
    }

    public void RemoveListing(User requestingUser, Listing listing)
    {
        if (listing.Seller.Username != requestingUser.Username)
            throw new InvalidOperationException("you can only remove your own listings");
        
        _listings.Remove(listing);
        requestingUser.Listings.Remove(listing);
    }
    
    //browse & sort
    public List<Listing> GetAvailableListings()
    {
        return _listings
            .Where(l => l.Status == ListingStatus.Available)
            .ToList();
    }
    
    public List<Listing> GetListingByCategory(Category category)
    {
        return _listings
            .Where(l => l.Status == ListingStatus.Available && l.Category == category)
            .ToList();
    }

    public List<Listing> SearchListings(string searchTerm)
    {
        var lower = searchTerm.ToLower();
        return _listings
            .Where(l => l.Status == ListingStatus.Available &&
                        (l.Title.ToLower().Contains(lower) || 
                         l.Description.ToLower().Contains(lower)))
            .ToList();
    }
    
    // Purchase

    public Transaction Purchase(User buyer, Listing listing)
    {
        if (listing.Seller.Username == buyer.Username)
            throw new InvalidOperationException("you cannot buy your own listing");

        if (listing.Status == ListingStatus.Sold)
            throw new InvalidOperationException("this listing has already been sold");

        listing.Status = ListingStatus.Sold;

        var transaction = new Transaction(buyer, listing.Seller, listing);
        _transactions.Add(transaction);
        buyer.Purchases.Add(transaction);
        listing.Seller.Sales.Add(transaction);
        
        return transaction;
    }

    public Review LeaveReview(User buyer, Transaction transaction, int rating, string? comment)
    {
        if (transaction.Buyer.Username != buyer.Username)
            throw new InvalidOperationException("cannot leave review on own listing");


        if (transaction.Review != null)
            throw new InvalidOperationException("You have already reviewed this listing");

        if (rating < 1 || rating > 6)
            throw new InvalidOperationException("Rating must be between 1 and 6");

        var review = new Review(rating, comment, buyer, transaction);
        transaction.Review = review;
        transaction.Seller.ReviewsReceived.Add(review);
        
        return review;
    }
}