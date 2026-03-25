using System.Text.Json;
using System.Text.Json.Serialization;
using LibraryDev.Domain.Services;

namespace LibraryDev.Infrastructure.Services;

public class OpenLibraryService : IOpenLibraryService
{
    private readonly HttpClient _httpClient;
    private const string BaseUrl = "https://openlibrary.org/api/books";

    public OpenLibraryService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<LivroExternoDto?> BuscarPorISBNAsync(string isbn)
    {
        try
        {
            var isbnLimpo = isbn.Replace("-", "").Replace(" ", "");
            var url = $"{BaseUrl}?bibkeys=ISBN:{isbnLimpo}&format=json&jscmd=data";

            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode) return null;

            var json = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<Dictionary<string, OpenLibraryBook>>(json);

            var key = $"ISBN:{isbnLimpo}";
            if (data == null || !data.TryGetValue(key, out var book)) return null;

            return new LivroExternoDto
            {
                Titulo = book.Title ?? string.Empty,
                Autor = book.Authors?.FirstOrDefault()?.Name ?? "Desconhecido",
                Editora = book.Publishers?.FirstOrDefault()?.Name,
                ISBN = isbnLimpo,
                AnoDePublicacao = ParseAno(book.PublishDate),
                Descricao = book.Excerpts?.FirstOrDefault()?.Text,
                CapaUrl = book.Cover?.Large ?? book.Cover?.Medium
            };
        }
        catch
        {
            return null;
        }
    }

    private static int? ParseAno(string? publishDate)
    {
        if (string.IsNullOrWhiteSpace(publishDate)) return null;
        var parts = publishDate.Split(' ', ',');
        foreach (var part in parts)
            if (int.TryParse(part.Trim(), out var ano) && ano > 1000 && ano <= DateTime.UtcNow.Year)
                return ano;
        return null;
    }

    private class OpenLibraryBook
    {
        [JsonPropertyName("title")]
        public string? Title { get; set; }

        [JsonPropertyName("authors")]
        public List<NamedEntity>? Authors { get; set; }

        [JsonPropertyName("publishers")]
        public List<NamedEntity>? Publishers { get; set; }

        [JsonPropertyName("publish_date")]
        public string? PublishDate { get; set; }

        [JsonPropertyName("excerpts")]
        public List<Excerpt>? Excerpts { get; set; }

        [JsonPropertyName("cover")]
        public Cover? Cover { get; set; }
    }

    private class NamedEntity
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }
    }

    private class Excerpt
    {
        [JsonPropertyName("text")]
        public string? Text { get; set; }
    }

    private class Cover
    {
        [JsonPropertyName("small")]
        public string? Small { get; set; }

        [JsonPropertyName("medium")]
        public string? Medium { get; set; }

        [JsonPropertyName("large")]
        public string? Large { get; set; }
    }
}
