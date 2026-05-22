using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolApp.Models
{
    public class Enrollment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int CourseId { get; set; }
        
        [ForeignKey("CourseId")]
        public Course? Course { get; set; }

        [Required]
        public int TraineeId { get; set; }

        [ForeignKey("TraineeId")]
        public Trainee? Trainee { get; set; }

        public bool IsPaid { get; set; } = false;

        public double Progress { get; set; } = 0;

        public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;

        // Paymob Integration
        public string? PaymobOrderId { get; set; }
        public string? TransactionId { get; set; }
    }
}
