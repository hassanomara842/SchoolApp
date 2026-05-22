using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SchoolApp.Models;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using System;

namespace SchoolApp.Controllers
{
    [Authorize]
    public class QuizController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public QuizController(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Quiz (For instructors/admins)
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var isTrainee = await _userManager.IsInRoleAsync(user, "Trainee");

            if (isTrainee)
            {
                // Trainee sees quizzes they can take or have taken
                var quizzes = await _context.Quizzes.Include(q => q.Course).ToListAsync();
                
                var attempts = await _context.QuizAttempts
                    .Where(qa => qa.TraineeUserId == user.Id)
                    .ToListAsync();

                ViewBag.IsTrainee = true;
                ViewBag.Attempts = attempts;
                return View(quizzes);
            }
            else
            {
                // Admin/Instructor sees all quizzes
                var quizzes = await _context.Quizzes.Include(q => q.Course).ToListAsync();
                ViewBag.IsTrainee = false;
                return View(quizzes);
            }
        }

        [Authorize(Roles = "Admin,Instructor")]
        public IActionResult Create()
        {
            ViewBag.Courses = new SelectList(_context.Courses, "Id", "Name");
            return View(new CreateQuizViewModel());
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Instructor")]
        public async Task<IActionResult> Create([FromBody] CreateQuizViewModel model)
        {
            if (ModelState.IsValid)
            {
                var quiz = new Quiz
                {
                    Title = model.Title,
                    CourseId = model.CourseId,
                    DurationMinutes = model.DurationMinutes,
                    Questions = model.Questions.Select(q => new Question
                    {
                        Text = q.Text,
                        Points = q.Points,
                        Options = q.Options.Select(o => new Option
                        {
                            Text = o.Text,
                            IsCorrect = o.IsCorrect
                        }).ToList()
                    }).ToList()
                };

                _context.Quizzes.Add(quiz);
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            return Json(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Instructor")]
        public async Task<IActionResult> GenerateAiQuestions([FromBody] AiQuizRequest req, [FromServices] SchoolApp.Services.IAiQuizService aiService)
        {
            try 
            {
                var questions = await aiService.GenerateQuestionsAsync(req.Topic, req.Count);
                return Json(new { success = true, questions = questions });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [Authorize(Roles = "Trainee")]
        public async Task<IActionResult> Take(int id)
        {
            var quiz = await _context.Quizzes
                .Include(q => q.Questions)
                .ThenInclude(q => q.Options)
                .FirstOrDefaultAsync(q => q.Id == id);

            if (quiz == null) return NotFound();

            var viewModel = new TakeQuizViewModel
            {
                QuizId = quiz.Id,
                QuizTitle = quiz.Title,
                DurationMinutes = quiz.DurationMinutes,
                Questions = quiz.Questions.Select(q => new TakeQuestionViewModel
                {
                    QuestionId = q.Id,
                    Text = q.Text,
                    Points = q.Points,
                    Options = q.Options.Select(o => new TakeOptionViewModel
                    {
                        OptionId = o.Id,
                        Text = o.Text
                    }).ToList()
                }).ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        [Authorize(Roles = "Trainee")]
        public async Task<IActionResult> Submit([FromBody] TakeQuizViewModel submission)
        {
            var user = await _userManager.GetUserAsync(User);
            var quiz = await _context.Quizzes
                .Include(q => q.Questions)
                .ThenInclude(q => q.Options)
                .FirstOrDefaultAsync(q => q.Id == submission.QuizId);

            if (quiz == null) return NotFound();

            int totalScore = 0;
            int maxScore = 0;

            foreach (var question in quiz.Questions)
            {
                maxScore += question.Points;
                var submittedQ = submission.Questions.FirstOrDefault(q => q.QuestionId == question.Id);
                if (submittedQ != null)
                {
                    var correctOption = question.Options.FirstOrDefault(o => o.IsCorrect);
                    if (correctOption != null && correctOption.Id == submittedQ.SelectedOptionId)
                    {
                        totalScore += question.Points;
                    }
                }
            }

            var attempt = new QuizAttempt
            {
                QuizId = quiz.Id,
                TraineeUserId = user.Id,
                Score = totalScore,
                TotalScore = maxScore,
                CompletedAt = DateTime.UtcNow
            };

            _context.QuizAttempts.Add(attempt);
            await _context.SaveChangesAsync();

            return Json(new { success = true, score = totalScore, total = maxScore });
        }

        [Authorize(Roles = "Admin,Instructor")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var quiz = await _context.Quizzes
                .Include(q => q.Course)
                .Include(q => q.Questions)
                .ThenInclude(q => q.Options)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (quiz == null) return NotFound();

            return View(quiz);
        }

        // GET: Quiz/Delete/5
        [Authorize(Roles = "Admin,Instructor")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var quiz = await _context.Quizzes
                .Include(q => q.Course)
                .FirstOrDefaultAsync(m => m.Id == id);
            
            if (quiz == null) return NotFound();

            return View(quiz);
        }

        // POST: Quiz/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Instructor")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var quiz = await _context.Quizzes.FindAsync(id);
            if (quiz != null)
            {
                _context.Quizzes.Remove(quiz);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
