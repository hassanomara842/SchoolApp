namespace SchoolApp.Models
{
    public class DashboardViewModel
    {
        public int TotalStudents { get; set; }
        public int TotalInstructors { get; set; }
        public int TotalCourses { get; set; }
        public int TotalDepartments { get; set; }
        public List<DepartmentStats>? DepartmentStats { get; set; }
        public List<CourseStats>? CourseStats { get; set; }
    }

    public class DepartmentStats
    {
        public string DepartmentName { get; set; } = string.Empty;
        public int StudentCount { get; set; }
    }

    public class CourseStats
    {
        public string CourseName { get; set; } = string.Empty;
        public int EnrollmentCount { get; set; }
    }
}
