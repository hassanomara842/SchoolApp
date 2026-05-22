using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using SchoolApp.Models;
using System;

namespace SchoolApp.Services
{
    public class GeminiQuizService : IAiQuizService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public GeminiQuizService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiKey = configuration["GeminiSettings:ApiKey"];
        }

        public async Task<List<QuestionViewModel>> GenerateQuestionsAsync(string topic, int count)
        {
            if (string.IsNullOrEmpty(_apiKey)) throw new InvalidOperationException("Gemini API key is not configured.");

            string url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-flash-latest:generateContent?key={_apiKey}";

            string prompt = $@"
You are a strict JSON API. Generate {count} multiple choice questions about '{topic}'.
Return ONLY a JSON array, nothing else (no markdown blocks like ```json).
Each question must follow this exact C# class structure:
[
  {{
    ""Text"": ""Question text?"",
    ""Points"": 1,
    ""Options"": [
      {{ ""Text"": ""Option 1"", ""IsCorrect"": true }},
      {{ ""Text"": ""Option 2"", ""IsCorrect"": false }},
      {{ ""Text"": ""Option 3"", ""IsCorrect"": false }},
      {{ ""Text"": ""Option 4"", ""IsCorrect"": false }}
    ]
  }}
]
Ensure exactly ONE correct option per question.
";

            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new { text = prompt }
                        }
                    }
                }
            };

            var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(url, content);
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(responseString);
            
            var root = doc.RootElement;
            var textResult = root
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text").GetString();

            // Clean up possible markdown if the AI ignored the instruction
            textResult = textResult.Trim();
            if (textResult.StartsWith("```json")) textResult = textResult.Substring(7);
            if (textResult.StartsWith("```")) textResult = textResult.Substring(3);
            if (textResult.EndsWith("```")) textResult = textResult.Substring(0, textResult.Length - 3);

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var questions = JsonSerializer.Deserialize<List<QuestionViewModel>>(textResult.Trim(), options);

            return questions ?? new List<QuestionViewModel>();
        }
    }
}
