# Multi-stage build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy project files
COPY UrlShortener/UrlShortener.csproj ./UrlShortener/
RUN cd UrlShortener && dotnet restore

# Copy source
COPY . .
RUN cd UrlShortener && dotnet publish -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .

# Expose port
EXPOSE 10000

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=5s --retries=3 \
  CMD curl -f http://localhost:10000/swagger || exit 1

# Run app
ENTRYPOINT ["dotnet", "UrlShortener.dll"]
