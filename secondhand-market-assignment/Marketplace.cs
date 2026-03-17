namespace secondhand_market_assignment;

public class Marketplace
{
    //user management
    private readonly List<User> _users = new ();

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
        return listing;
    }
}