namespace SchoolApp.Models
{
    public class LeaderboardViewModel
    {
        public int Rank { get; set; }
        public int TraineeId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ProfilePicture { get; set; } = string.Empty;
        public string DepartmentName { get; set; } = string.Empty;
        public int TotalXP { get; set; }
    }
}
