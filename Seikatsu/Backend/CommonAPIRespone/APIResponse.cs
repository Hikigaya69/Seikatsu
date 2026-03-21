namespace Seikatsu.Backend.CommonAPIRespone
{
    public class APIResponse<T>
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }
        public string? ErrorCode { get; set; }
        public Dictionary<string, string>? Errors { get; set; }
    }
}
