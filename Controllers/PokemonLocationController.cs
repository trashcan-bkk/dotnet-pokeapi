using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dotnet_pokeapi.Services;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_pokeapi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PokemonLocationController : ControllerBase
    {
        private readonly LocationService _locationService = new();


        [HttpGet("/trashcan/pokemon/locations/{name}")]
        public async Task<IActionResult> GetLocations(
            string name, [FromQuery] string version = null, [FromQuery] string method = null)
        {
            try
            {
                // set variables to get pokemon name and locations from location service
                var (pokemonName, locations) = await _locationService.GetPokemonLocationsAsync(name, version, method);
                // it will return pokemonName and locations 

                // set pokemon = pokemonName returned from LocationService
                return Ok(new { pokemon = pokemonName, locations });
            }
            catch (Exception ex)
            {
                return NotFound(new { error = ex.Message });
            }
        }
    }
}