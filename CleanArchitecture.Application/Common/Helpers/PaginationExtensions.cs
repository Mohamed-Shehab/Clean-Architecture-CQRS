using CleanArchitecture.Application.Common.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Common.Helpers
{
    public static class PaginationExtensions
    {
        public static void Normalize(this PaginationParams pagination)
        {
            pagination.PageNumber = pagination.PageNumber < 1 ? 1 : pagination.PageNumber;

            pagination.PageSize = pagination.PageSize < 1 ? 10 :
                                  pagination.PageSize > 100 ? 100 : pagination.PageSize;
        }
    }
}
