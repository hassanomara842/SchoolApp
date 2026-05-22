namespace SchoolApp.Models
{
    public class Attendance
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public int TraineeId { get; set; }
        public DateTime Date { get; set; }
        public bool IsPresent { get; set; }
        public string? Remarks { get; set; }

        public Course? Course { get; set; }
        public Trainee? Trainee { get; set; }
    }
}
