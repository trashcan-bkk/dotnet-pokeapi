using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Excel;
using dotnet_pokeapi.Models;
using Microsoft.EntityFrameworkCore;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace dotnet_pokeapi.Services
{
    public class PokemonCollectionService
    {
        private readonly AppDbContext _db;

        private readonly HttpClient _httpClient;

        public PokemonCollectionService(AppDbContext db, HttpClient httpClient)
        {
            _db = db;
            _httpClient = httpClient;
        }

        // add to collection
        public async Task<PokemonCollection> AddToCollectionAsync(PokemonCollection collection)
        {

            // Check for existing Pokémon in user's collection
            var existingPokemonChecking = await _db.PokemonCollection
                .FirstOrDefaultAsync(p => p.Username == collection.Username && p.PokemonName.ToLower() == collection.PokemonName.ToLower());

            if (existingPokemonChecking != null)
            {
                throw new InvalidOperationException("You already have this Pokémon in your collection.");
            }

            // If marking as favorite, check favorite limit
            if (collection.IsFavorite)
            {
                var favoriteCount = await _db.PokemonCollection
                    .CountAsync(p => p.Username == collection.Username && p.IsFavorite);

                if (favoriteCount >= 6)
                {
                    throw new InvalidOperationException("You can only have up to 6 favorite Pokémon.");
                }
            }
            collection.CaughtAt = DateTime.UtcNow;
            _db.PokemonCollection.Add(collection);
            await _db.SaveChangesAsync();
            return collection;
        }

        // get pokemon collection by username
        public async Task<List<PokemonCollection>> GetCollectionByUsernameAsync(string username)
        {
            return await _db.PokemonCollection
                .Where(p => p.Username == username)
                .ToListAsync();
        }


        // get pokemon fav collection by username
        public async Task<IEnumerable<PokemonCollection>> GetFavCollectionByUsernameAsync(string username)
        {
            var favCollection = await _db.PokemonCollection
                .Where(p => p.Username == username && p.IsFavorite)
                .ToListAsync();

            return favCollection;
        }

        // remove from fav 
        public async Task<PokemonCollection?> UnmarkFavAsync(string username, string pokemonName)
        {
            var collection = await _db.PokemonCollection
                .FirstOrDefaultAsync(p => p.Username == username && p.PokemonName.ToLower() == pokemonName.ToLower());

            if (collection == null || !collection.IsFavorite)
            {
                return null; // Not found or not marked as favorite
            }

            collection.IsFavorite = false;
            collection.CaughtAt = DateTime.SpecifyKind(collection.CaughtAt, DateTimeKind.Utc);

            _db.PokemonCollection.Update(collection);
            await _db.SaveChangesAsync();

            return collection;
        }

        // remove from collection 
        public async Task<bool> RemoveFromCollectionAsync(string username, string pokemonName)
        {
            var collection = await _db.PokemonCollection
                .FirstOrDefaultAsync(p => p.Username == username && p.PokemonName.ToLower() == pokemonName.ToLower());

            if (collection == null)
                return false;

            _db.PokemonCollection.Remove(collection);
            await _db.SaveChangesAsync();
            return true;
        }

        // generate collection PDF by username 
        public async Task<byte[]> GenerateCollectionPDF(string username)
        {
            var collection = await _db.PokemonCollection
                .Where(p => p.Username == username)
                .ToListAsync();

            var document = new PdfDocument();
            var font = new XFont("Arial", 12);

            foreach (var pokemon in collection)
            {
                var page = document.AddPage();
                var gfx = XGraphics.FromPdfPage(page);

                gfx.DrawString($"Name: {pokemon.PokemonName}", font, XBrushes.Black, 20, 40);
                gfx.DrawString($"Favorite: {pokemon.IsFavorite}", font, XBrushes.Black, 20, 60);
                gfx.DrawString($"Shiny: {pokemon.IsShiny}", font, XBrushes.Black, 20, 80);
                gfx.DrawString($"Caught At: {pokemon.CaughtAt:yyyy-MM-dd}", font, XBrushes.Black, 20, 100);

                // Download and draw image
                try
                {
                    var imageUrl = $"https://img.pokemondb.net/artwork/large/{pokemon.PokemonName.ToLower()}.jpg";
                    var imageBytes = await _httpClient.GetByteArrayAsync(imageUrl);

                    using var ms = new MemoryStream(imageBytes);
                    using var image = SixLabors.ImageSharp.Image.Load(ms, out IImageFormat format);


                    image.Mutate(x => x.Resize(128, 128)); // Resize to fit PDF

                    using var imgStream = new MemoryStream();
                    await image.SaveAsync(imgStream, new JpegEncoder());
                    imgStream.Position = 0;

                    var xImage = XImage.FromStream(() => imgStream);
                    gfx.DrawImage(xImage, 350, 40);
                }
                catch
                {
                    gfx.DrawString("Image not available", font, XBrushes.Gray, 350, 40);
                }
            }

            using var stream = new MemoryStream();
            document.Save(stream, false);
            return stream.ToArray();
        }

        // generate fav collection PDF by username 
        public async Task<byte[]> GenerateFavCollectionPDF(string username)
        {
            var collection = await _db.PokemonCollection
                .Where(p => p.Username == username && p.IsFavorite)
                .ToListAsync();

            var document = new PdfDocument();
            var font = new XFont("Arial", 12);

            foreach (var pokemon in collection)
            {
                var page = document.AddPage();
                var gfx = XGraphics.FromPdfPage(page);

                gfx.DrawString($"Name: {pokemon.PokemonName}", font, XBrushes.Black, 20, 40);
                gfx.DrawString($"Favorite: {pokemon.IsFavorite}", font, XBrushes.Black, 20, 60);
                gfx.DrawString($"Shiny: {pokemon.IsShiny}", font, XBrushes.Black, 20, 80);
                gfx.DrawString($"Caught At: {pokemon.CaughtAt:yyyy-MM-dd}", font, XBrushes.Black, 20, 100);

                // Download and draw image
                try
                {
                    var imageUrl = $"https://img.pokemondb.net/artwork/large/{pokemon.PokemonName.ToLower()}.jpg";
                    var imageBytes = await _httpClient.GetByteArrayAsync(imageUrl);

                    using var ms = new MemoryStream(imageBytes);
                    using var image = SixLabors.ImageSharp.Image.Load(ms, out IImageFormat format);


                    image.Mutate(x => x.Resize(128, 128)); // Resize to fit PDF

                    using var imgStream = new MemoryStream();
                    await image.SaveAsync(imgStream, new JpegEncoder());
                    imgStream.Position = 0;

                    var xImage = XImage.FromStream(() => imgStream);
                    gfx.DrawImage(xImage, 350, 40);
                }
                catch
                {
                    gfx.DrawString("Image not available", font, XBrushes.Gray, 350, 40);
                }
            }

            using var stream = new MemoryStream();
            document.Save(stream, false);
            return stream.ToArray();
        }


        // generate fav collection Excel by username  
        public byte[] ExportFavCollectionExcel(string username)
        {
            var favorites = _db.PokemonCollection
                .Where(p => p.Username == username && p.IsFavorite)
                .ToList();

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Favorite Pokémon");

            // Headers
            worksheet.Cell(1, 1).Value = "Pokemon Name";
            worksheet.Cell(1, 2).Value = "Is Shiny";
            worksheet.Cell(1, 3).Value = "Caught At";
            worksheet.Cell(1, 4).Value = "Username";

            for (int i = 0; i < favorites.Count; i++)
            {
                var pokemon = favorites[i];
                worksheet.Cell(i + 2, 1).Value = pokemon.PokemonName;
                worksheet.Cell(i + 2, 2).Value = pokemon.IsShiny ? "Yes" : "No";
                worksheet.Cell(i + 2, 3).Value = pokemon.CaughtAt.ToString("yyyy-MM-dd");
                worksheet.Cell(i + 2, 4).Value = username;
            }

            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }
    }
}