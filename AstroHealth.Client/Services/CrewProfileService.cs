using System.Text.Json;
using AstroHealth.Shared.Dtos;

namespace AstroHealth.Client.Services;

public sealed class CrewProfileService(HealthApiService api, IConfiguration configuration)
{
    private Task? _initializationTask;

    public IReadOnlyList<AstronautDto> Astronauts { get; private set; } = Array.Empty<AstronautDto>();
    public AstronautDto? SelectedAstronaut { get; private set; }

    public event Action? SelectedAstronautChanged;

    public Task InitializeAsync()
    {
        _initializationTask ??= LoadAstronautsAsync();
        return _initializationTask;
    }

    public async Task<AstronautDto> CreateAsync(AstronautDto astronaut)
    {
        await InitializeAsync();

        var created = await api.CreateAstronautAsync(astronaut);
        Astronauts = Astronauts
            .Append(created)
            .OrderBy(profile => profile.Name, StringComparer.OrdinalIgnoreCase)
            .ToList();
        SelectedAstronaut = created;
        SelectedAstronautChanged?.Invoke();
        return created;
    }

    public void Select(string? astronautId)
    {
        var selected = Astronauts.FirstOrDefault(profile => profile.AstronautId == astronautId);
        if (selected is null || selected == SelectedAstronaut)
            return;

        SelectedAstronaut = selected;
        SelectedAstronautChanged?.Invoke();
    }

    private async Task LoadAstronautsAsync()
    {
        try
        {
            Astronauts = await api.GetAstronautsAsync();
            var preferredId = configuration["DemoAstronautId"];
            SelectedAstronaut = Astronauts.FirstOrDefault(profile => profile.AstronautId == preferredId)
                ?? Astronauts.FirstOrDefault();
            SelectedAstronautChanged?.Invoke();
        }
        catch (Exception exception) when (
            exception is HttpRequestException or InvalidDataException or JsonException)
        {
            _initializationTask = null;
            throw;
        }
    }
}
