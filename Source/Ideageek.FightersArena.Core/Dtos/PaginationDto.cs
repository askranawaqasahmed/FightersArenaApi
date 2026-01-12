namespace Ideageek.FightersArena.Core.Dtos
{
    public class PaginationDto
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string SortColumn { get; set; }
        public string SortOrder { get; set; } = "asc";
        public List<FilterDto> Filters { get; set; } = new();
    }

    public class FilterDto
    {
        public string Column { get; set; }
        public string Value { get; set; }
        public string FilterType { get; set; }
    }
    public class ResponseDto<T>
    {
        public int Total { get; set; }
        public IEnumerable<T> Data { get; set; }
    }
}
