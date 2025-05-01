using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using dotnet_pokeapi.Models;

namespace dotnet_pokeapi.Services
{
    public class QuizService
    {
        private readonly HttpClient _httpClient;
        public QuizService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<Quiz> GetPokemonTypeQuizAsync()
        {
            var random = new Random();
            int pokemonId = random.Next(1, 1010); // Random Pokémon ID

            var response = await _httpClient.GetAsync($"https://pokeapi.co/api/v2/pokemon/{pokemonId}");
            if (!response.IsSuccessStatusCode) 
                return null;

            var jsonData = await response.Content.ReadAsStringAsync();

            using var jsonDoc = JsonDocument.Parse(jsonData);
            var name = jsonDoc.RootElement.GetProperty("name").GetString();
            var types = jsonDoc.RootElement
                .GetProperty("types")
                .EnumerateArray()
                .Select(t => t.GetProperty("type").GetProperty("name").GetString())
                .ToList();

            if (types.Count == 0) 
                return null;

            var correctType = types[0]; // just use one type for now
            var allTypes = new List<string> {
                "fire", "water", "grass", "electric", "psychic", "ice", "dragon", "dark", "fairy", "ghost", "ground"
            };

            var options = allTypes
                .Where(t => t != correctType)
                .OrderBy(_ => random.Next())
                .Take(5)
                .ToList();

            options.Add(correctType);
            options = options.OrderBy(_ => random.Next()).ToList();

            // hash (base64-style ID, e.g. name + correctType for mock validation)
            var questionId = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{name}:{correctType}"));

            return new Quiz
            {
                Question = $"What type is {char.ToUpper(name[0]) + name.Substring(1)}?",
                Options = options,
                CorrectAnswer = correctType, 
                QuestionId = questionId
            };
        }
    }
}