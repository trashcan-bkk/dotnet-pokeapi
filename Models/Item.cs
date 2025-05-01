using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace dotnet_pokeapi.Models
{
    [Table("items")]
    public class Item
    {
         [Column("id")]
        public int Id { get; set; }

         [Column("name")]
        public string ItemName { get; set; } = string.Empty;

         [Column("effect")]
        public string? Effect { get; set; }

         [Column("cost")]
        public int Cost { get; set; }

        [JsonIgnore]
        public ICollection<UserItem> UserItems { get; set; } = new List<UserItem>();
    }
}