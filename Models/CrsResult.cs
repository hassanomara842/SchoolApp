namespace SchoolApp.Models
{
    public class CrsResult
    {
        public int Id { get; set; }
        public int Degree { get; set; }
        public int Crs_id { get; set; }
        public int Trainee_id { get; set; }

        public Course? Course { get; set; }
        public Trainee? Trainee { get; set; }
    }
}
