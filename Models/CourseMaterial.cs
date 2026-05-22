namespace SchoolApp.Models
{
    public class CourseMaterial
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public bool IsVideo { get; set; } = false;
        public string? VideoUrl { get; set; }
        public DateTime UploadDate { get; set; } = DateTime.UtcNow;
        public string UploadedByUserId { get; set; } = string.Empty;

        public Course? Course { get; set; }
    }
}
