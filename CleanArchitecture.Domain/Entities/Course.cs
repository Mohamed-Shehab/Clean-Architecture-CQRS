using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Domain.Entities
{
    public class Course
    {
        public Course()
        {
            StudentCourses = new List<StudentCourse>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(150)]
        public string Title { get; set; }

        [Range(1, 1000)]
        public int MaxStudents { get; set; }

        // Navigation Property
        public virtual ICollection<StudentCourse> StudentCourses { get; set; }
    }
}
