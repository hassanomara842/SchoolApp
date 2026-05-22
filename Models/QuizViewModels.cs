using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SchoolApp.Models
{
    public class CreateQuizViewModel
    {
        [Required]
        public string Title { get; set; }

        [Required]
        [Display(Name = "Course")]
        public int CourseId { get; set; }

        [Required]
        [Display(Name = "Duration (Minutes)")]
        public int DurationMinutes { get; set; } = 30;

        public List<QuestionViewModel> Questions { get; set; } = new List<QuestionViewModel>();
    }

    public class AiQuizRequest
    {
        public string Topic { get; set; }
        public int Count { get; set; }
    }

    public class QuestionViewModel
    {
        [Required]
        public string Text { get; set; }
        public int Points { get; set; } = 1;
        public List<OptionViewModel> Options { get; set; } = new List<OptionViewModel>();
    }

    public class OptionViewModel
    {
        [Required]
        public string Text { get; set; }
        public bool IsCorrect { get; set; }
    }

    public class TakeQuizViewModel
    {
        public int QuizId { get; set; }
        public string QuizTitle { get; set; }
        public int DurationMinutes { get; set; }
        public List<TakeQuestionViewModel> Questions { get; set; } = new List<TakeQuestionViewModel>();
    }

    public class TakeQuestionViewModel
    {
        public int QuestionId { get; set; }
        public string Text { get; set; }
        public int Points { get; set; }
        public List<TakeOptionViewModel> Options { get; set; } = new List<TakeOptionViewModel>();
        public int SelectedOptionId { get; set; }
    }

    public class TakeOptionViewModel
    {
        public int OptionId { get; set; }
        public string Text { get; set; }
    }
}
