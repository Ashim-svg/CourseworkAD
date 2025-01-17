using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Web;
using CourseworkAD.Models;

public class UserService
{
    private static readonly string DesktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
    private static readonly string FolderPath = Path.Combine(DesktopPath, "LocalDB");
    private static readonly string FilePath = Path.Combine(FolderPath, "appdata.json");

    public User? CurrentUser { get; private set; } // Tracks the logged-in user

    // Load AppData (Users, Transactions, Debts) from JSON file
    public AppData LoadData()
    {
        if (!File.Exists(FilePath))
        {
            return new AppData();
        }

        try
        {
            var json = File.ReadAllText(FilePath);
            return JsonSerializer.Deserialize<AppData>(json) ?? new AppData();
        }
        catch (JsonException)
        {
            return new AppData();
        }
        catch (Exception)
        {
            return new AppData();
        }
    }

    // Save AppData (Users, Transactions, Debts) to JSON file
    public void SaveData(AppData data)
    {
        EnsureFolderExists();
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };
        var json = JsonSerializer.Serialize(data, options);
        File.WriteAllText(FilePath, json);
    }

    // Manage Users within AppData
    public List<User> LoadUsers()
    {
        var appData = LoadData();
        return appData.Users;
    }

    public void SaveUsers(List<User> users)
    {
        var appData = LoadData();
        appData.Users = users;
        SaveData(appData);
    }

    // Hash a password securely
    public string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(password);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }

    // Validate a password against a stored hash
    public bool ValidatePassword(string inputPassword, string storedPassword)
    {
        var decodedStoredPassword = HttpUtility.HtmlDecode(storedPassword);  // Decode if necessary
        var hashedInputPassword = HashPassword(inputPassword);
        return hashedInputPassword == decodedStoredPassword;
    }

    // Set the current user (login)
    public void LoginPage(User user)
    {
        CurrentUser = user;
    }

    // Clear the current user (logout)
    public void Logout()
    {
        CurrentUser = null;
    }

    // Utility: Ensure the data folder exists
    private void EnsureFolderExists()
    {
        if (!Directory.Exists(FolderPath))
        {
            Directory.CreateDirectory(FolderPath);
        }
    }

    // **New Method: Check if a user is logged in**
    public bool IsUserLoggedIn()
    {
        return CurrentUser != null;
    }
}
