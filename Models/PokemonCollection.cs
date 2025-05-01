using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace dotnet_pokeapi.Models
{
    [Table("pokemon_collections")]
    public class PokemonCollection
    {
        public int Id { get; set; }
        public string Username { get; set; } 
        public string PokemonName { get; set; }
        public bool IsFavorite { get; set; }
        public bool IsShiny { get; set; }
        public DateTime CaughtAt { get; set; }
    }
}