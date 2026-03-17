namespace secondhand_market_assignment;

public class User
{
    public string Username { get; set; }
    private string Password { get; set; }
    
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