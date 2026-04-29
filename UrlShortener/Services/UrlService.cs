using Microsoft.EntityFrameworkCore;
using UrlShortener.Data;
using UrlShortener.Models;

namespace UrlShortener.Services;

public class UrlService : IUrlService
{
    private readonly AppDbContext _db;
    private const string Chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

    public UrlService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<ShortenResponse> CreateShortUrlAsync(ShortenRequest request, string baseUrl)
    {
        // Validate URL
        if (!Uri.TryCreate(request.OriginalUrl, UriKind.Absolute, out var uri) ||
            (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
        {
            throw new ArgumentException("Invalid URL. Must start with http:// or https://");
        }

        // Check if this URL was already shortened
        var existing = await _db.ShortUrls
            .FirstOrDefaultAsync(u => u.OriginalUrl == request.OriginalUrl);

        if (existing != null)
            return ToResponse(existing, baseUrl);

        // Generate a unique 6-char code
        string code;
        do { code = GenerateCode(6); }
        while (await _db.ShortUrls.AnyAsync(u => u.Code == code));

        var shortUrl = new ShortUrl
        {
            OriginalUrl = request.OriginalUrl,
            Code = code,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = request.ExpiresInDays.HasValue
                ? DateTime.UtcNow.AddDays(request.ExpiresInDays.Value)
                : null
        };

        _db.ShortUrls.Add(shortUrl);
        await _db.SaveChangesAsync();

        return ToResponse(shortUrl, baseUrl);
    }

    public async Task<string?> GetOriginalUrlAsync(string code)
    {
        var entry = await _db.ShortUrls.FirstOrDefaultAsync(u => u.Code == code);
        if (entry == null) return null;

        // Check expiry
        if (entry.ExpiresAt.HasValue && entry.ExpiresAt < DateTime.UtcNow)
            return null;

        return entry.OriginalUrl;
    }

    public async Task IncrementClickAsync(string code)
    {
        var entry = await _db.ShortUrls.FirstOrDefaultAsync(u => u.Code == code);
        if (entry != null)
        {
            entry.Clicks++;
            await _db.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<ShortenResponse>> GetAllAsync(string baseUrl)
    {
        var urls = await _db.ShortUrls
            .OrderByDescending(u => u.CreatedAt)
            .ToListAsync();

        return urls.Select(u => ToResponse(u, baseUrl));
    }

    public async Task<bool> DeleteAsync(string code)
    {
        var entry = await _db.ShortUrls.FirstOrDefaultAsync(u => u.Code == code);
        if (entry == null) return false;

        _db.ShortUrls.Remove(entry);
        await _db.SaveChangesAsync();
        return true;
    }

    // Helpers
    private static string GenerateCode(int length)
    {
        var rng = new Random();
        return new string(Enumerable.Range(0, length)
            .Select(_ => Chars[rng.Next(Chars.Length)])
            .ToArray());
    }

    private static ShortenResponse ToResponse(ShortUrl u, string baseUrl) =>
        new(
            ShortUrl: $"{baseUrl}/{u.Code}",
            Code: u.Code,
            OriginalUrl: u.OriginalUrl,
            Clicks: u.Clicks,
            CreatedAt: u.CreatedAt,
            ExpiresAt: u.ExpiresAt
        );
}
