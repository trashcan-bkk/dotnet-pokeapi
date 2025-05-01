using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnet_pokeapi.Models
{
    public class BuyRequest
    {
        public string ItemName { get; set; }
        public int Quantity { get; set; }
    }
}