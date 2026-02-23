namespace Seikatsu.Backend.Models
{ //models are used to define data transfer objects (DTOs) which are used
  //to transfer data between client and server and internall application procedures
    public class UserDTO
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } =string.Empty;
    }
}

