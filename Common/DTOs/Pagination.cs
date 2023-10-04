namespace Common.DTOs
{
    public class Pagination<T> : FiltersRequest
    {
        public int Offset { get; set; }
        public int Limit { get; set; }
    }
}
