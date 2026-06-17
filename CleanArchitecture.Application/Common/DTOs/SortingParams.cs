using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Common.DTOs
{
    public class SortingParams
    {
        public string OrderBy { get; set; } = "id";
        public bool IsDescending { get; set; } = false;
    }
}
