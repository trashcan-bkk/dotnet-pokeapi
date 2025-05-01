using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using dotnet_pokeapi.Models;

namespace dotnet_pokeapi.Services
{
    public class PokemonService
    {
        private readonly HttpClient _httpClient;

        public PokemonService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<string> GetPokemonAsync(string name)
        {
            // call to PokeAPI
            var response = await _httpClient.GetAsync($"https://pokeapi.co/api/v2/pokemon/{name.ToLower()}");

            if (!response.IsSuccessStatusCode)
            {
                return $"Error: {response.StatusCode}";
            }

            // return raw json
            var result = await response.Content.ReadAsStringAsync();
            return result;
        }

        public async Task<List<PokemonInfo>> GetPokemonListWithLimit(int limit)
        {
            // declare result as a list of pokemon 
            var result = new List<PokemonInfo>();

            var response = await _httpClient.GetAsync($"https://pokeapi.co/api/v2/pokemon?limit={limit}");

            if (!response.IsSuccessStatusCode)
            {
                return result;
            }

            var json = await response.Content.ReadAsStringAsync();
            using var jsonDoc = JsonDocument.Parse(json);

            var pokemonArray = jsonDoc.RootElement.GetProperty("results").EnumerateArray();

            foreach (var pokemon in pokemonArray)
            {
                // get pokemon name
                string pokemonName = pokemon.GetProperty("name").GetString();

                // get full details and extract sprite
                var pokemonDetail = await _httpClient.GetAsync($"https://pokeapi.co/api/v2/pokemon/{pokemonName}");
                if (!pokemonDetail.IsSuccessStatusCode) continue;

                var detailJson = await pokemonDetail.Content.ReadAsStringAsync();
                var detailJsonDoc = JsonDocument.Parse(detailJson);

                string image = detailJsonDoc.RootElement.GetProperty("sprites").GetProperty("front_default").GetString();

                // add each pokemon into PokemonInfo
                result.Add(new PokemonInfo
                {
                    PokemonName = pokemonName,
                    PokemonImage = image
                });
            }
            return result;
        }

        public async Task<List<string>> GetPokemonByType(string type)
        {
            // call to get pokemon type
            var response = await _httpClient.GetAsync($"https://pokeapi.co/api/v2/type/{type.ToLower()}");

            if (!response.IsSuccessStatusCode)
            {
                return new List<string>();
            }

            var json = await response.Content.ReadAsStringAsync();
            var jsonDoc = JsonDocument.Parse(json);

            // pick up the pokemon's name and return as a list of string
            return jsonDoc.RootElement
                .GetProperty("pokemon")
                .EnumerateArray()
                .Select(p => p.GetProperty("pokemon").GetProperty("name").GetString())
                .ToList();
        }

        public async Task<List<PokemonInfo>> GetPokemonByTypeWithImage(string type)
        {
            // create a list of PokemonInfo
            var result = new List<PokemonInfo>();

            // call to get pokemon type
            var response = await _httpClient.GetAsync($"https://pokeapi.co/api/v2/type/{type.ToLower()}");
            if (!response.IsSuccessStatusCode) return result;

            var json = await response.Content.ReadAsStringAsync();
            var jsonDoc = JsonDocument.Parse(json);

            var pokemonList = jsonDoc.RootElement
                .GetProperty("pokemon")
                .EnumerateArray()
                .Select(p => p.GetProperty("pokemon").GetProperty("name").GetString())
                .Take(5) // optional limit
                .ToList();

            foreach (var name in pokemonList)
            {
                var pokeRes = await _httpClient.GetAsync($"https://pokeapi.co/api/v2/pokemon/{name}");
                if (!pokeRes.IsSuccessStatusCode) continue;

                var pokeJson = await pokeRes.Content.ReadAsStringAsync();
                using var pokeDoc = JsonDocument.Parse(pokeJson);

                var image = pokeDoc.RootElement
                    .GetProperty("sprites")
                    .GetProperty("front_default")
                    .GetString();

                result.Add(new PokemonInfo
                {
                    PokemonName = name,
                    PokemonImage = image
                });
            }
            return result;
        }

        public async Task<List<string>> GetPokemonByAbility(string ability)
        {
            var response = await _httpClient.GetAsync($"https://pokeapi.co/api/v2/ability/{ability.ToLower()}");
            if (!response.IsSuccessStatusCode) return new List<string>();

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);

            return doc.RootElement
                .GetProperty("pokemon")
                .EnumerateArray()
                .Select(p => p.GetProperty("pokemon").GetProperty("name").GetString())
                .ToList();
        }


        public async Task<PokemonInfo> GetRandomPokemonAsync()
        {
            // use Random() to get random number between 1-1010 and set 'id' to store that random number
            var random = new Random();
            int id = random.Next(1, 1010); // PokeAPI goes up to Gen 9 (check latest count)

            // parse random id in the PokeAPI's url
            var response = await _httpClient.GetAsync($"https://pokeapi.co/api/v2/pokemon/{id}");
            if (!response.IsSuccessStatusCode) return null;

            var json = await response.Content.ReadAsStringAsync();
            using var jsonDoc = JsonDocument.Parse(json);

            string pokemonName = jsonDoc.RootElement.GetProperty("name").GetString();
            string pokemonImage = jsonDoc.RootElement
                .GetProperty("sprites")
                .GetProperty("front_default")
                .GetString();

            return new PokemonInfo
            {
                PokemonName = pokemonName,
                PokemonImage = pokemonImage
            };
        }
    }
}