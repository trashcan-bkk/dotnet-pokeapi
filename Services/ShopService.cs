using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using dotnet_pokeapi.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace dotnet_pokeapi.Services
{
    public class ShopService
    {
        private readonly AppDbContext _db;

        public ShopService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<User> GetUserAsync(string username)
        {
            var user = await _db.Users
                .FirstOrDefaultAsync(u => u.Username == username);

            if (user == null) return null;

            return new User
            {
                Id = user.Id,
                Username = user.Username,
                PokeCoins = user.PokeCoins
            };
        }

        public async Task<List<Item>> GetItemListAsync()
        {
            return await _db.Items.ToListAsync();
        }


        public async Task<Item?> GetItemAsync(string itemName)
        {
            var item = await _db.Items
                .FirstOrDefaultAsync(u => u.ItemName == itemName);

            if (item == null) return null;

            return new Item
            {
                Id = item.Id,
                ItemName = item.ItemName,
                Effect = item.Effect,
                Cost = item.Cost
            };
        }

        public async Task<string> BuyItemAsync(string username, string itemName, int quantity)
        {
            var user = await _db.Users
                .Include(u => u.Items)
                .ThenInclude(ui => ui.Item)
                .FirstOrDefaultAsync(u => u.Username == username);

            if (user == null)
                return "User not found.";

            var item = await _db.Items.FirstOrDefaultAsync(i => i.ItemName == itemName);
            if (item == null)
                return "Item not found.";

            var totalCost = item.Cost * quantity;
            if (user.PokeCoins < totalCost)
                return "Insufficient PokéCoins.";

            var userItem = user.Items.FirstOrDefault(ui => ui.ItemId == item.Id);
            if (userItem == null)
            {
                userItem = new UserItem
                {
                    UserId = user.Id,
                    ItemId = item.Id,
                    Quantity = quantity
                };
                _db.UserItems.Add(userItem);
            }
            else
            {
                userItem.Quantity += quantity;
            }

            user.PokeCoins -= totalCost;

            await _db.SaveChangesAsync();
            return $"{username} bought {quantity} x {itemName}.";
        }

        public async Task<string> SellItemAsync(string username, string itemName, int quantity)
        {
            var user = await _db.Users
                .Include(u => u.Items)
                .ThenInclude(ui => ui.Item)
                .FirstOrDefaultAsync(u => u.Username == username);

            if (user == null)
                return "User not found.";

            var item = await _db.Items.FirstOrDefaultAsync(i => i.ItemName == itemName);
            if (item == null)
                return "Item not found.";

            var userItem = user.Items.FirstOrDefault(ui => ui.ItemId == item.Id);
            if (userItem == null || userItem.Quantity < quantity)
                return "You don't have enough of this item to sell.";

            userItem.Quantity -= quantity;
            if (userItem.Quantity == 0)
            {
                _db.UserItems.Remove(userItem);
            }

            var totalSellPrice = (item.Cost / 2) * quantity; // You can adjust the sell price logic
            user.PokeCoins += totalSellPrice;

            await _db.SaveChangesAsync();
            return $"{username} sold {quantity} x {itemName}";
        }

        public async Task<List<InventoryDto>> GetInventoryAsync(string username)
        {
            var user = await _db.Users
                .Include(u => u.Items)
                    .ThenInclude(ui => ui.Item)
                .FirstOrDefaultAsync(u => u.Username == username);

            if (user == null || user.Items == null)
                return new List<InventoryDto>();

            return user.Items.Select(ui => new InventoryDto
            {
                UserId = user.Id,
                UserName = user.Username,
                ItemId = ui.ItemId,
                ItemName = ui.Item?.ItemName ?? "Unknown",
                Quantity = ui.Quantity
            }).ToList();
        }
    }
}