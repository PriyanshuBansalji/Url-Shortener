namespace UrlShortener.Models;

public record ShortenRequest(string OriginalUrl, int? ExpiresInDays = null);

public record ShortenResponse(
    string ShortUrl,
    string Code,
    string OriginalUrl,
    int Clicks,
    DateTime CreatedAt,
    DateTime? ExpiresAt
);
