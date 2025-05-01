using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using dotnet_pokeapi.Services;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_pokeapi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PokemonController : ControllerBase
    {
        private readonly PokemonService _pokemonService = new();

        // get pokemon by specific pokemon's name 
        [HttpGet("/trashcan/pokemon/{name}")]
        public async Task<IActionResult> GetPokemon(string name)
        {
            var pokemon = await _pokemonService.GetPokemonAsync(name);
            return Content(pokemon, "application/json");
        }

        // get pokemon list with query string of limit 
        [HttpGet("/trashcan/pokemon/get-list")]
        public async Task<IActionResult> GetPokemonListWithLimit([FromQuery] int limit)
        {
            var listOfPokemon = await _pokemonService.GetPokemonListWithLimit(limit);
            return Ok(listOfPokemon);
        }

        [HttpGet("/trashcan/pokemon/get-by-type/{type}")]
        public async Task<IActionResult> GetPokemonByType(string type)
        {
            var listOfPokemonByType = await _pokemonService.GetPokemonByType(type);
            return Ok(listOfPokemonByType);
        }
        
        [HttpGet("/trashcan/pokemon/get-by-type-with-img/{type}")]
        public async Task<IActionResult> GetPokemonByTypeWithImage(string type)
        {
            var listOfPokemonByTypeWithImage = await _pokemonService.GetPokemonByTypeWithImage(type);
            return Ok(listOfPokemonByTypeWithImage);
        }

        [HttpGet("/trashcan/pokemon/get-by-ability/{ability}")]
        public async Task<IActionResult> GetPokemonByAbility(string ability)
        {
            var listOfPokemonByAbility = await _pokemonService.GetPokemonByAbility(ability);
            return Ok(listOfPokemonByAbility);
        } 

        [HttpGet("/trashcan/pokemon/get-random")]
        public async Task<IActionResult> GetRandomPokemon()
        {
            var randomPokemon = await _pokemonService.GetRandomPokemonAsync();
            return Ok(randomPokemon); 
        }
    }
}