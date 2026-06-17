using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Features.Courses.DTOs
{
    public class CourseDetailsDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CourseCapacity { get; set; }
    }
}
