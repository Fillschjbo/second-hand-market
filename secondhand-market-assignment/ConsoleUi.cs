using System.ComponentModel.Design;
using System.Runtime.InteropServices;

namespace secondhand_market_assignment;

public class ConsoleUi
{
    private readonly Marketplace _marketplace;
    private User? _currentUser;
    
    public ConsoleUi(Marketplace marketplace)
    {
        _marketplace = marketplace;
    }

    //entry point

    public void Run()
    {
        while (true)
        {
            if (_currentUser == null)
                ShowGuestMenu();

            else
                ShowMainMenu();
        }
           
    }
    
    
    //input helpers
    private int ReadIntInRange(string prompt, int min, int max)
    {
        while (true)
        {
            Console.WriteLine(prompt);
            if (int.TryParse(Console.ReadLine(), out int value) && value >= min && value <= max)
                return value;
            Console.WriteLine($"Please enter a number between {min} and {max}.");
        }
    }

    private string ReadRequiredString(string prompt)
    {
        while (true)
        {
            Console.WriteLine(prompt);
            var input = Console.ReadLine()?.Trim();
            if (!string.IsNullOrEmpty(input))
                return input;
            Console.WriteLine($"This field can not be empty.");
        }
    }

    private T ReadEnum<T>(string prompt) where T : struct, Enum
    {
        var values =  Enum.GetValues<T>();
        Console.WriteLine(prompt);
        for (int i = 0; i < values.Length; i++)
            Console.WriteLine($"{i + 1} . {values[i]}");
        
        int choice = ReadIntInRange("Select: ", 1, values.Length);
        return values[choice - 1];
    }

    private decimal ReadPositiveDecimal(string prompt)
    {
        while (true)
        {
            Console.WriteLine(prompt);
            if (decimal.TryParse(Console.ReadLine(), out decimal value) && value > 0)
                return value;
            Console.WriteLine($"Please enter a positive number.");
        }
    }
    
    //guest menu
    private void PrintHeader(string title)
    {
        Console.WriteLine();
        Console.WriteLine($"=== {title} ===");
    }

    private void ShowGuestMenu()
    {
        PrintHeader("Marketplace Menu");
        Console.WriteLine("1. Register new user");
        Console.WriteLine("2. Login");
        Console.WriteLine("3. Exit");
        
        int choice = ReadIntInRange("Select an option: ", 1,3);
        switch (choice)
        {
            case 1: HandeleRegister(); break;
            case 2: HandleLogin(); break;
            case 3:
                Console.WriteLine("Goodbye!");
                Environment.Exit(0);
                break;
        }
    }

    private void HandeleRegister()
    {
        PrintHeader("Register new user");
        string username = ReadRequiredString("Username: ");
        string password = ReadRequiredString("Password: ");

        try
        {
            var user = _marketplace.Register(username, password);
            Console.WriteLine($"User {username} successfully registered.");
            _currentUser = user;
        }
        catch (InvalidOperationException e)
        {
            Console.WriteLine($"User {username} failed to register. {e.Message}");
        }
    }

    private void HandleLogin()
    {
        {
            PrintHeader("Log In!");
            string username = ReadRequiredString("Username: ");
            string password = ReadRequiredString("Password: ");

            try
            {
                _currentUser = _marketplace.Login(username, password);
                Console.WriteLine($"Welcome back {_currentUser.Username}!");
            }
            catch (InvalidOperationException e)
            {
                Console.WriteLine($"User {username} failed to Log in. {e.Message}");
            }
        }
    }

    
    // Main Menu
    private void ShowMainMenu()
    {
        PrintHeader($"Main Menu. Logged in as '{_currentUser!.Username}'!");
        Console.WriteLine("1. Create Listing");
        Console.WriteLine("2. Browse Listings");
        Console.WriteLine("3. Search Listings");
        Console.WriteLine("4. My Profile");
        Console.WriteLine("5. Logout");
        
        int  choice = ReadIntInRange("Select an option: ", 1, 5);
        switch (choice)
        {
            case 1: HandleCreateListing(); break;
            case 2 : HandleBrowseListings(); break;
            case 3: HandleSearchListingss(); break;
            case 5: HandleLogout(); break;
        }
    }

    private void HandleLogout()
    {
        Console.WriteLine($"Goodbye {_currentUser!.Username}!");
        _currentUser = null;
    }
    
    //Manage Listings

    private void HandleCreateListing()
    {
        PrintHeader("Create Listing");
        
        string title = ReadRequiredString("Title: ");
        string description = ReadRequiredString("Description: ");
        Category category = ReadEnum<Category>("Category: ");
        Condition condition = ReadEnum<Condition>("Condition: ") ;
        decimal price = ReadPositiveDecimal("Price: ");
        
        var listing = _marketplace.CreateListing(_currentUser!, title, description, category, condition, price);
        Console.WriteLine($"Listing \"{listing.Title}\" created successfully!");
    }
    
    private void HandleRemoveListing(Listing listing)
    {
        try
        {
            _marketplace.RemoveListing(_currentUser!, listing);
            Console.WriteLine($"{listing.Title} successfully removed!");
        }
        catch (InvalidOperationException e)
        {
            Console.WriteLine($"{listing.Title} failed to remove listing. {e.Message}");
        }
        
    }
    
    //Browse Listings
    
    private void HandleBrowseListings()
    {
        PrintHeader("Browse Listings");
        Console.WriteLine("1. Browse All Listings");
        Console.WriteLine("2. Filter By Category");
        
        int choice  = ReadIntInRange("Select an option: ", 1, 2);

        List<Listing> listings;
        if (choice == 1)
        {
            listings = _marketplace.GetAvailableListings();
        }
        else
        {
            Category category = ReadEnum<Category>("Select Category: ");
            listings = _marketplace.GetListingByCategory(category);
        }
        ShowListingsTable(listings);
    }

    private void HandleSearchListingss()
    {
        PrintHeader("Search Listings");
        string SearchTerm = ReadRequiredString("Search Term: ");
        
        var listings = _marketplace.SearchListings(SearchTerm);
        ShowListingsTable(listings);
    }
    
    //show listings
    private void ShowListingsTable(List<Listing> listings)
    {
        if (listings.Count == 0)
        {
            Console.WriteLine("No listings found!");
            return;  
        }

        Console.WriteLine();
        Console.WriteLine($"{"#", 4} {"Title", -25} {"Category", -22} {"Condition", -10} {"Price", 8}");
        Console.WriteLine(new string('-', 75));

        for (int i = 0; listings.Count > i; i++)
        {
            var l =  listings[i];
            Console.WriteLine($"{i + 1, -4} {l.Title, -25} {l.Category, -22} {l.Condition, -10} {l.Price, 5:N0} kr");
        }

        Console.WriteLine();
        int choice = ReadIntInRange("Select a listing to view (press 0 to go back): ", 0, listings.Count);
        if (choice == 0)
        {
            Console.WriteLine("No listings found!");
            return;
        }
        
        ShowListingDetails(listings[choice -1]);
    }

    private void ShowListingDetails(Listing listing)
    {
        PrintHeader(listing.Title);
        Console.WriteLine($"Seller: {listing.Seller.Username}");
        Console.WriteLine($"Category: {listing.Category}");
        Console.WriteLine($"Condition: {listing.Condition}");
        Console.WriteLine($"Price: {listing.Price}");
        Console.WriteLine($"Description: {listing.Description}");
        Console.WriteLine();
        
        bool isOwnListing = listing.Seller.Username == _currentUser!.Username;
        if (isOwnListing)
        {
            Console.WriteLine($"Own listing: {listing.Title}");
            Console.WriteLine("1. Remove listing");
            Console.WriteLine("2. Go back");
            int choice = ReadIntInRange("Select an option: ", 1, 2);
            if (choice == 1)
                HandleRemoveListing(listing);
            return;
        }

        Console.WriteLine("1. Buy item");
        Console.WriteLine("2. Go Back");
        int buyChoice = ReadIntInRange("Select an option: ", 1, 2);
        if (buyChoice == 1) HandlePurchase(listing);
        return;
    }

    
    //purchases
    private void HandlePurchase(Listing listing)
    {
        try
        {
            var transaction = _marketplace.Purchase(_currentUser!,  listing);
            Console.WriteLine("Purchase successful!");
        }
        catch (InvalidOperationException e)
        {
            Console.WriteLine($"{listing.Title} failed to purchase listing. {e.Message}");}
    }
    
}