using CleanArchitecture.Application.Common.Models.Querying;

namespace CleanArchitecture.Application.Common.Helpers
{
    public static class PaginationExtensions
    {
        public static void Normalize(this PaginationModel pagination, 
                                     int defaultPageSize = 10, 
                                     int maxPageSize = 100)
        {
            pagination.PageNumber = pagination.PageNumber < 1 ? 1 : pagination.PageNumber;

            pagination.PageSize = pagination.PageSize < 1 ? defaultPageSize :
                                  pagination.PageSize > maxPageSize ? maxPageSize : pagination.PageSize;
        }
    }
}
