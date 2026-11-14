using System.IO;
using System.Text.Json;
using SeatForge.Models;

namespace SeatForge.Services;

public sealed class SeatAssignmentStore
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private readonly string _filePath;

    public SeatAssignmentStore(string filePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
        _filePath = filePath;
    }

    public async Task<IReadOnlyDictionary<string, string>> LoadAsync(CancellationToken cancellationToken = default)
    {
        if (!File.Exists(_filePath))
        {
            return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }

        await using var stream = File.OpenRead(_filePath);
        var assignments = await JsonSerializer.DeserializeAsync<Dictionary<string, string>>(
            stream,
            JsonOptions,
            cancellationToken);

        return assignments is null
            ? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            : new Dictionary<string, string>(assignments, StringComparer.OrdinalIgnoreCase);
    }

    public async Task SaveAsync(IEnumerable<DeviceRecord> devices, CancellationToken cancellationToken = default)
    {
        var directory = Path.GetDirectoryName(_filePath);
        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        var assignments = devices
            .Where(device => !string.Equals(device.AssignedSeat, "Unassigned", StringComparison.Ordinal))
            .ToDictionary(device => device.Id, device => device.AssignedSeat, StringComparer.OrdinalIgnoreCase);

        var temporaryPath = _filePath + ".tmp";
        await using (var stream = File.Create(temporaryPath))
        {
            await JsonSerializer.SerializeAsync(stream, assignments, JsonOptions, cancellationToken);
            await stream.FlushAsync(cancellationToken);
        }

        File.Move(temporaryPath, _filePath, overwrite: true);
    }
}
