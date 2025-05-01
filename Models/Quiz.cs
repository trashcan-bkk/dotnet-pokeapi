using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnet_pokeapi.Models
{
    public class Quiz
    {
        public string Question { get; set; }
        public List<string> Options { get; set; }
        public string CorrectAnswer { get; set; } // Optional: remove from response later
        public string QuestionId { get; set; } // used for answer tracking
    }
}