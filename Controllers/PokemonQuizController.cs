using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dotnet_pokeapi.Models;
using dotnet_pokeapi.Services;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_pokeapi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PokemonQuizController : ControllerBase
    {
        private readonly QuizService _quizService = new();

        [HttpGet("/trashcan/pokemon/quiz")]
        public async Task<IActionResult> GetQuiz()
        {
            var quiz = await _quizService.GetPokemonTypeQuizAsync();
            return quiz != null ? Ok(quiz) : StatusCode(500, "Failed to fetch trivia.");
        }

        [HttpPost("/trashcan/pokemon/check-answer")]
        public IActionResult CheckAnswer([FromBody] AnswerSubmission input)
        {
            if (string.IsNullOrWhiteSpace(input.Answer) || string.IsNullOrWhiteSpace(input.QuestionId))
            {
                return BadRequest("Question ID and selected answer are required.");
            }

            try
            {
                // decode the QuestionId to get the correct answer
                var decoded = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(input.QuestionId));
                var parts = decoded.Split(':');
                if (parts.Length != 2)
                    return BadRequest("Invalid question ID format.");

                string correct = parts[1]; // e.g., "electric"
                bool isCorrect = string.Equals(input.Answer.Trim(), correct.Trim(), StringComparison.OrdinalIgnoreCase);

                return Ok(new
                {
                    result = isCorrect ? "Correct!" : "Wrong!",
                    isCorrect,
                    correctAnswer = correct
                });
            }
            catch
            {
                return BadRequest("Invalid question ID.");
            }
        }
    }

}