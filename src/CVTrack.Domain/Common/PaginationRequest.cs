namespace CVTrack.Domain.Common
{
    public class PaginationRequest
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        // Maximum page size limiti
        public int MaxPageSize { get; set; } = 100;

        public int ValidatedPageSize => PageSize > MaxPageSize ? MaxPageSize : PageSize;
        public int ValidatedPageNumber => PageNumber < 1 ? 1 : PageNumber;
    }
}