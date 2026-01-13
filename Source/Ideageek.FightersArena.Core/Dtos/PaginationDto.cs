namespace Ideageek.FightersArena.Core.Dtos
{
    public class PaginationDto
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string SortColumn { get; set; } = string.Empty;
        public string SortOrder { get; set; } = "asc";
        public List<FilterDto> Filters { get; set; } = new();
    }

    public class FilterDto
    {
        public string Column { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public string FilterType { get; set; } = string.Empty;
    }
    public class ResponseDto<T>
    {
        public int Total { get; set; }
        public IEnumerable<T> Data { get; set; } = Array.Empty<T>();
    }
}
