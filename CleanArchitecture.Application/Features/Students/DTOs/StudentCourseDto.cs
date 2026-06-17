using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Features.Students.DTOs
{
    public class StudentCourseDto
    {
        public int CourseId { get; set; }
        public string Title { get; set; }
        public DateTime EnrolledAt { get; set; }
    }
}
