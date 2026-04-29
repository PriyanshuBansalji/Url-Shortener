using Microsoft.EntityFrameworkCore;
using UrlShortener.Data;
using UrlShortener.Services;

var builder = WebApplication.CreateBuilder(args);

// Services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// SQLite database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=urls.db"));

// Custom services
builder.Services.AddScoped<IUrlService, UrlService>();

// CORS for frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowNetlify", policy =>
        policy
            .WithOrigins(
                "https://urll.netlify.app",
                "http://localhost:3000",
                "http://localhost:5000")
            .AllowAnyMethod()
            .AllowAnyHeader());
});

var app = builder.Build();

// Auto-migrate DB on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowNetlify");
app.UseStaticFiles();

// Redirect short URLs — this must come BEFORE MapControllers
app.MapGet("/{code}", async (string code, IUrlService urlService, HttpContext ctx) =>
{
    var url = await urlService.GetOriginalUrlAsync(code);
    if (url == null)
        return Results.NotFound(new { error = "Short URL not found." });

    await urlService.IncrementClickAsync(code);
    return Results.Redirect(url);
});

app.MapControllers();

// Serve frontend index.html as default
app.MapFallbackToFile("index.html");

app.Run();
