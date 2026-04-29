using UrlShortener.Models;

namespace UrlShortener.Services;

public interface IUrlService
{
    Task<ShortenResponse> CreateShortUrlAsync(ShortenRequest request, string baseUrl);
    Task<string?> GetOriginalUrlAsync(string code);
    Task IncrementClickAsync(string code);
    Task<IEnumerable<ShortenResponse>> GetAllAsync(string baseUrl);
    Task<bool> DeleteAsync(string code);
}
