using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolApp.Models
{
    public class Quiz
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Quiz Title")]
        public string Title { get; set; }

        [Required]
        [Display(Name = "Course")]
        public int CourseId { get; set; }

        [ForeignKey("CourseId")]
        public Course? Course { get; set; }

        [Display(Name = "Duration (Minutes)")]
        public int DurationMinutes { get; set; } = 30;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Question> Questions { get; set; } = new List<Question>();
        public ICollection<QuizAttempt> Attempts { get; set; } = new List<QuizAttempt>();
    }
}
