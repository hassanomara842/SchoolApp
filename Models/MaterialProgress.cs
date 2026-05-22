using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolApp.Models
{
    public class MaterialProgress
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int TraineeId { get; set; }

        [ForeignKey("TraineeId")]
        public Trainee? Trainee { get; set; }

        [Required]
        public int MaterialId { get; set; }

        [ForeignKey("MaterialId")]
        public CourseMaterial? Material { get; set; }

        public bool IsCompleted { get; set; } = false;

        public DateTime? CompletedAt { get; set; }
    }
}
