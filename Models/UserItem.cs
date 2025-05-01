using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace dotnet_pokeapi.Models
{
    [Table("useritems")]
    public class UserItem
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("userid")]
        public int UserId { get; set; }

        [Column("itemid")]
        public int ItemId { get; set; }

        [Column("quantity")]
        public int Quantity { get; set; }


        [JsonIgnore] // 👈 prevent infinite loop
        public User User { get; set; }

        [JsonIgnore] // 👈 prevent infinite loop
        public Item Item { get; set; }

    }

    public class InventoryDto
    {
        
        public int UserId { get; set; }

        public string UserName { get; set; }
        
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public int Quantity { get; set; }
    }
}