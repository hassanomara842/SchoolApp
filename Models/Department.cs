using System.Collections.Generic;

namespace SchoolApp.Models
{
    public class Department
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Manager { get; set; }

        // Navigation Properties
        public ICollection<Instructor>? Instructors { get; set; } = new List<Instructor>();
        public ICollection<Course>? Courses { get; set; } = new List<Course>();
        public ICollection<Trainee>? Trainees { get; set; } = new List<Trainee>();
    }
}
