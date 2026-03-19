namespace secondhand_market_assignment;

public class Review
{
    public int Rating { get; }
    
    public string? Comment { get; }
    
    public User Author { get; }
    
    public Transaction Transaction { get; }

    public Review(int rating, string? comment, User author, Transaction transaction)
    {
        Rating = rating;
        Comment = comment;
        Author = author;
        Transaction = transaction;
    }
}