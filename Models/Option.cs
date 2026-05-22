using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolApp.Models
{
    public class Option
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Text { get; set; }

        public bool IsCorrect { get; set; } = false;

        public int QuestionId { get; set; }

        [ForeignKey("QuestionId")]
        public Question? Question { get; set; }
    }
}
