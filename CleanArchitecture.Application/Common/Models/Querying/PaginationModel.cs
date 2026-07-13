namespace CleanArchitecture.Application.Common.Models.Querying
{
    public sealed class PaginationModel
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
