using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dotnet_pokeapi.Models;
using dotnet_pokeapi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_pokeapi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PokemonMartController : ControllerBase
    {
        private readonly ShopService _shopService;

        public PokemonMartController(ShopService shopService)
        {
            _shopService = shopService;
        }

        [HttpGet("/trashcan/pokemon/pokemart/user/{username}")]
        public async Task<IActionResult> GetUser(string username)
        {
            var result = await _shopService.GetUserAsync(username);

            if (result == null)
                return NotFound(new { message = "User not found" });

            return Ok(result);
        }

        [HttpGet("/trashcan/pokemon/pokemart/item/{itemName}")]
        public async Task<IActionResult> GetItem(string itemName)
        {
            var result = await _shopService.GetItemAsync(itemName);

            if (result == null)
                return NotFound(new { message = "Item not found" });

            return Ok(result);
        }

        [HttpGet("/trashcan/pokemon/pokemart/item")]
        public async Task<IActionResult> GetItemList()
        {
            var items = await _shopService.GetItemListAsync();
            return Ok(items);
        }


        [HttpPost("/trashcan/pokemon/pokemart/buy")]
        public async Task<IActionResult> BuyItem(string username, string itemName, int quantity)
        {
            var result = await _shopService.BuyItemAsync(username, itemName, quantity);
            return Ok(new { message = result });
        }

        [HttpPost("/trashcan/pokemon/pokemart/sell")]
        public async Task<IActionResult> SellItem(string username, string itemName, int quantity)
        {
            var result = await _shopService.SellItemAsync(username, itemName, quantity);
            return Ok(new { message = result });
        }

        [HttpGet("/trashcan/pokemon/pokemart/inventory/{username}")]
        public async Task<IActionResult> GetInventory(string username)
        {
            var items = await _shopService.GetInventoryAsync(username);
            return Ok(items);
        }
    }
}