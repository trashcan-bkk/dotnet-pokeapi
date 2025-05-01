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
    public class PokemonCollectionController : ControllerBase
    {
        private readonly PokemonCollectionService _pokemonCollectionService;

        public PokemonCollectionController(PokemonCollectionService pokemonCollectionService)
        {
            _pokemonCollectionService = pokemonCollectionService;
        }

        [HttpPost("/trashcan/pokemon/collection/add")]
        public async Task<IActionResult> AddToCollection([FromBody] PokemonCollection entry)
        {
            var result = await _pokemonCollectionService.AddToCollectionAsync(entry);
            return Ok(result);
        }

        [HttpGet("/trashcan/pokemon/collection/{username}")]
        public async Task<IActionResult> GetUserCollection(string username)
        {
            var collection = await _pokemonCollectionService.GetCollectionByUsernameAsync(username);
            return Ok(collection);
        }

        [HttpGet("/trashcan/pokemon/collection/fav/{username}")]
        public async Task<ActionResult<IEnumerable<PokemonCollection>>> GetFavoritesByUsername(string username)
        {
            var favorites = await _pokemonCollectionService.GetFavCollectionByUsernameAsync(username);

            if (favorites == null || !favorites.Any())
            {
                return NotFound($"No favorite Pokémon found for {username}");
            }

            return Ok(favorites);
        }

        [HttpPut("/trashcan/pokemon/collection/fav/remove/{username}/{pokemonName}")]
        public async Task<IActionResult> UnmarkFavorite(string username, string pokemonName)
        {
            var result = await _pokemonCollectionService.UnmarkFavAsync(username, pokemonName);

            if (result == null)
            {
                return NotFound(new { message = $"{pokemonName} is not in {username}'s favorite list." });
            }

            return Ok(result);
        }

        [HttpDelete("/trashcan/pokemon/collection/remove/{username}/{pokemonName}")]
        public async Task<IActionResult> RemovePokemon(string username, string pokemonName)
        {
            var success = await _pokemonCollectionService.RemoveFromCollectionAsync(username, pokemonName);

            if (!success)
                return NotFound(new { message = $"{pokemonName} is not found in {username}'s collection." });

            return Ok(new { message = $"{pokemonName} removed from {username}'s collection." });
        }

        [HttpGet("/trashcan/pokemon/collection/pdf/{username}")]
        public async Task<IActionResult> GenerateCollectionPDF(string username)
        {
            var pdfBytes = await _pokemonCollectionService.GenerateCollectionPDF(username);
            return File(pdfBytes, "application/pdf", $"{username}_pokedex.pdf");
        }

        [HttpGet("/trashcan/pokemon/collection/fav/pdf/{username}")]
        public async Task<IActionResult> GenerateFavCollectionPDF(string username)
        {
            var pdfBytes = await _pokemonCollectionService.GenerateFavCollectionPDF(username);
            return File(pdfBytes, "application/pdf", $"{username}_pokedex.pdf");
        }

        [HttpGet("/trashcan/pokemon/collection/fav/excel/{username}")]
        public IActionResult ExportExcel(string username)
        {
            var excelBytes = _pokemonCollectionService.ExportFavCollectionExcel(username);
            return Ok(File(excelBytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"{username}_favorite_pokemon.xlsx"));
        }
    }
}