using System.Net;
using System.Net.Http.Json;
using AstroHealth.Shared.Dtos;

namespace AstroHealth.Client.Services;

public sealed class HealthApiService(HttpClient httpClient)
{
    public async Task<IReadOnlyList<AstronautDto>> GetAstronautsAsync(
        CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.GetAsync("api/astronauts", cancellationToken);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<List<AstronautDto>>(cancellationToken)
            ?? throw new InvalidDataException("The astronauts API returned an empty response.");
    }

    public async Task<AstronautDto> CreateAstronautAsync(
        AstronautDto astronaut,
        CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.PostAsJsonAsync("api/astronauts", astronaut, cancellationToken);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<AstronautDto>(cancellationToken)
            ?? throw new InvalidDataException("The astronauts API returned an empty response.");
    }

    public async Task<IReadOnlyList<HealthReadingDto>> GetReadingsAsync(
        string astronautId,
        int limit = 30,
        CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.GetAsync(
            $"api/readings/{Uri.EscapeDataString(astronautId)}?limit={limit}",
            cancellationToken);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<List<HealthReadingDto>>(cancellationToken)
            ?? throw new InvalidDataException("The readings API returned an empty response.");
    }

    public async Task<HealthReadingDto?> GetLatestReadingAsync(
        string astronautId,
        CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.GetAsync(
            $"api/readings/{Uri.EscapeDataString(astronautId)}/latest",
            cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<HealthReadingDto>(cancellationToken)
            ?? throw new InvalidDataException("The latest reading API returned an empty response.");
    }

    public async Task AddReadingAsync(
        HealthReadingDto reading,
        CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.PostAsJsonAsync("api/readings", reading, cancellationToken);
        response.EnsureSuccessStatusCode();
    }
}
