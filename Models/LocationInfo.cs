using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnet_pokeapi.Models
{
    public class LocationInfo
    {
        public string Area { get; set; }
        public string Region { get; set; }
        public List<string> Methods { get; set; }
        public string Chance { get; set; }
        public string LevelRange { get; set; }
    }
}