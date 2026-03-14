namespace Seikatsu.Backend.Models
{
    public class RequestTokenRefreshDto
    {  // DTO to receive token refresh requests from client  (internal application procedures)
        public Guid UserId { get; set; }

        public  string? RefreshToken { get; set; }
    }

}