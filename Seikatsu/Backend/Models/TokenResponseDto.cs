namespace Seikatsu.Backend.Models
{
    public class TokenResponseDto
    {  // DTO to send tokens to client (internal application procedures)

        public  required string AccessToken { get; set; }
        public required string RefreshToken { get; set; }   

        

    }
}
