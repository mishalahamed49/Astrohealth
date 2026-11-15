# ── Build stage ──────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution + project files first (layer cache for restore)
COPY AstroHealth.sln ./
COPY AstroHealth.Shared/AstroHealth.Shared.csproj         AstroHealth.Shared/
COPY AstroHealth.Client/AstroHealth.Client.csproj         AstroHealth.Client/
COPY AstroHealth.Server/AstroHealth.Server.csproj         AstroHealth.Server/

RUN dotnet restore AstroHealth.Server/AstroHealth.Server.csproj

# Copy everything and publish
COPY . .
RUN dotnet publish AstroHealth.Server/AstroHealth.Server.csproj \
    -c Release -o /app/publish --no-restore

# ── Runtime stage ─────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

# Render.com uses PORT env variable
ENV ASPNETCORE_URLS=http://+:${PORT:-8080}
ENV ASPNETCORE_ENVIRONMENT=Production

EXPOSE 8080

ENTRYPOINT ["dotnet", "AstroHealth.Server.dll"]
