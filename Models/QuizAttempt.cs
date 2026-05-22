using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolApp.Models
{
    public class QuizAttempt
    {
        [Key]
        public int Id { get; set; }

        public int QuizId { get; set; }
        
        [ForeignKey("QuizId")]
        public Quiz? Quiz { get; set; }

        [Required]
        public string TraineeUserId { get; set; }

        [ForeignKey("TraineeUserId")]
        public ApplicationUser? TraineeUser { get; set; }

        public int Score { get; set; }
        public int TotalScore { get; set; }

        public DateTime StartedAt { get; set; } = DateTime.UtcNow;
        public DateTime? CompletedAt { get; set; }
    }
}
