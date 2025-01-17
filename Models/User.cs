namespace CourseworkAD.Models
{
    public class User
    {
        public string Username { get; set; }
        public string Password { get; set; } // This will store the hashed password
        public string PreferredCurrency { get; set; } = "NPR";

    }
}

