using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolApp.Models
{
    public class Course
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Degree { get; set; }
        public int MinDegree { get; set; }
        public int Hrs { get; set; }
        public decimal Price { get; set; } = 49.99m;
        public int Dept_id { get; set; }

        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? DiscountPrice { get; set; }
        public string? BadgeText { get; set; }
        public string? BadgeColor { get; set; }

        public Department? Department { get; set; }
        public ICollection<Instructor>? Instructors { get; set; } = new List<Instructor>();
        public ICollection<CrsResult>? CrsResults { get; set; } = new List<CrsResult>();
        public ICollection<CourseMaterial>? CourseMaterials { get; set; } = new List<CourseMaterial>();
        public ICollection<Attendance>? Attendances { get; set; } = new List<Attendance>();
    }
}
