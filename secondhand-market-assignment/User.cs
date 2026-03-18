namespace secondhand_market_assignment;

public class User
{
    public string Username { get; set; }
    private string Password { get; set; }
    
    public List<Listing> Listings { get; } = new();

    public List<Transaction> Purchases { get; } = new();

    public List<Transaction> Sales { get; } = new();

    public List<Review> ReviewsReceived { get; } = new();
    
    public User (string username, string password)
    {
        Username = username;
        Password = password;
    }
    
    public bool CheckPassword(string password)
    {
        return password == Password;
    }
}