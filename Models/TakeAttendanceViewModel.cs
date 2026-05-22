namespace SchoolApp.Models
{
    public class TakeAttendanceViewModel
    {
        public int CourseId { get; set; }
        public string CourseName { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public List<AttendanceRecord> Records { get; set; } = new List<AttendanceRecord>();
    }

    public class AttendanceRecord
    {
        public int TraineeId { get; set; }
        public string TraineeName { get; set; } = string.Empty;
        public bool IsPresent { get; set; }
        public string? Remarks { get; set; }
    }
}
