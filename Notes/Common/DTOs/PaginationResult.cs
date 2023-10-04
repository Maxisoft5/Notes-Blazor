namespace Common.DTOs
{
    public class PaginationResult<T>
    {
        public IEnumerable<T> Result { get; set; }
        public int TotalCount { get; set; }
    }
}
