using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using dotnet_pokeapi.Models;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_pokeapi.Services
{
    public class LocationService
    {
        private readonly HttpClient _httpClient;

        public LocationService()
        {
            _httpClient =  new HttpClient();
        }

        public async Task<(string PokemonName, List<LocationInfo> Locations)> GetPokemonLocationsAsync(
            string name, string version, string method)
        {
            // call to PokeAPI using pokemon name
            var response = await _httpClient.GetAsync($"https://pokeapi.co/api/v2/pokemon/{name.ToLower()}/encounters");

            if (!response.IsSuccessStatusCode)
                throw new Exception("Pokémon not found or has no encounter data.");

            var json = await response.Content.ReadAsStringAsync();
            var jsonDoc = JsonDocument.Parse(json).RootElement;

            // set result as a list to get data as LocationInfo
            var result = new List<LocationInfo>();

            foreach (var area in jsonDoc.EnumerateArray())
            {
                string areaName = area.GetProperty("location_area").GetProperty("name").GetString()!;
                string regionName = areaName.Split('-')[0]; // simple region 

                var versionDetails = area.GetProperty("version_details");

                var matchingDetails = versionDetails.EnumerateArray()
                    .Where(v =>
                        string.IsNullOrEmpty(version) ||
                        v.GetProperty("version").GetProperty("name").GetString() == version.ToLower()
                    );

                foreach (var detail in matchingDetails)
                {
                    var encounterDetails = detail.GetProperty("encounter_details");

                    foreach (var enc in encounterDetails.EnumerateArray())
                    {
                        string methodList = enc.GetProperty("method").GetProperty("name").GetString()!;
                        if (!string.IsNullOrEmpty(method) && methodList != method.ToLower()) continue;

                        int minLevel = enc.GetProperty("min_level").GetInt32();
                        int maxLevel = enc.GetProperty("max_level").GetInt32();
                        int chance = enc.GetProperty("chance").GetInt32();

                        result.Add(new LocationInfo
                        {
                            Area = areaName,
                            Region = regionName,
                            Methods = new List<string> { methodList },
                            Chance = $"{chance}%",
                            LevelRange = $"Lv {minLevel}" + (minLevel != maxLevel ? $"–{maxLevel}" : "")
                        });
                    }
                }
            }

            // return pokemon name in lower, and result as a list of LocationInfo
            return (name.ToLower(), result);
        }
    }
}