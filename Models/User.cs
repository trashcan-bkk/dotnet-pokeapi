using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace dotnet_pokeapi.Models
{
    [Table("users")]
    public class User
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("username")]
        public string Username { get; set; } = string.Empty;

        [Column("pokecoins")]
        public int PokeCoins { get; set; }

        [JsonIgnore]
        public ICollection<UserItem> Items { get; set; } = new List<UserItem>();
    }
}