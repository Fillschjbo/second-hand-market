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
            case 5: HandleLogout(); break;
        }
    }

    private void HandleLogout()
    {
        Console.WriteLine($"Goodbye {_currentUser!.Username}!");
        _currentUser = null;
    }
    
    //Create Listing 

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
}