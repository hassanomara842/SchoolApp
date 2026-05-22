using System.Collections.Generic;
using System.Threading.Tasks;
using SchoolApp.Models;

namespace SchoolApp.Services
{
    public interface IAiQuizService
    {
        Task<List<QuestionViewModel>> GenerateQuestionsAsync(string topic, int count);
    }
}
