using System.Collections.Generic;

namespace SchoolApp.Models
{
    public class Trainee
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Image { get; set; }
        public string? Address { get; set; }
        public decimal Grade { get; set; }
        public int Dept_id { get; set; }

        public Department? Department { get; set; }
        public ICollection<CrsResult>? CrsResults { get; set; } = new List<CrsResult>();
        public ICollection<Attendance>? Attendances { get; set; } = new List<Attendance>();
    }
}
