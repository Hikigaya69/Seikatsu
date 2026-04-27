namespace Seikatsu.Backend.Models
{
    public class PagedResult<T>
    {
        public List<T> Items { get; set; } = [];
        public DateTime? NextCursorDate { get; set; }
        //public Guid? NextCursorId { get; set; } wont work for some reason in the service layer
        public bool HasNextPage => NextCursorDate != null;
    }
}