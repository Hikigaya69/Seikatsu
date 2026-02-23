namespace Seikatsu.Backend.Entity

{   // Entity is used to define data models which will be used to create database tables    
    public class User
    {
        public Guid ID { get; set; }
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; }

        public string Roles { get; set; } = string.Empty;  
        
        public string ? RefreshToken { get; set; }

        public DateTime? RefreshTokenExpiryTime { get; set; }


    }
}
