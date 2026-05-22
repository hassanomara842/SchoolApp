using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolApp.Models
{
    public class Question
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Text { get; set; }

        public int Points { get; set; } = 1;

        public int QuizId { get; set; }

        [ForeignKey("QuizId")]
        public Quiz? Quiz { get; set; }

        public ICollection<Option> Options { get; set; } = new List<Option>();
    }
}
