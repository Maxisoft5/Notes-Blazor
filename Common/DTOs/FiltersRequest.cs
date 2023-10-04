namespace Common.DTOs
{
    public class FiltersRequest
    {
        public string? TitleFilter { get; set; }
        public string? BodyKeyWordFilter { get; set; }
        public DateTime? CreatedAtFilter { get; set; }
    }
}
