using System.Management;
using SeatForge.Models;

namespace SeatForge.Services;

public sealed class DeviceInventoryService
{
    private static readonly HashSet<string> RelevantClasses = new(StringComparer.OrdinalIgnoreCase)
    {
        "AudioEndpoint",
        "Camera",
        "Display",
        "HIDClass",
        "Image",
        "Keyboard",
        "MEDIA",
        "Monitor",
        "Mouse"
    };

    public Task<IReadOnlyList<DeviceRecord>> GetDevicesAsync(CancellationToken cancellationToken = default) =>
        Task.Run<IReadOnlyList<DeviceRecord>>(() => GetDevices(cancellationToken), cancellationToken);

    private static IReadOnlyList<DeviceRecord> GetDevices(CancellationToken cancellationToken)
    {
        var devices = new List<DeviceRecord>();
        using var searcher = new ManagementObjectSearcher(
            "SELECT PNPDeviceID, Name, PNPClass, Status, Manufacturer FROM Win32_PnPEntity WHERE Name IS NOT NULL");

        foreach (ManagementObject item in searcher.Get())
        {
            cancellationToken.ThrowIfCancellationRequested();

            var category = item["PNPClass"]?.ToString() ?? "Other";
            if (!RelevantClasses.Contains(category))
            {
                continue;
            }

            var id = item["PNPDeviceID"]?.ToString();
            var name = item["Name"]?.ToString();
            if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(name))
            {
                continue;
            }

            devices.Add(new DeviceRecord
            {
                Id = id,
                Name = name,
                Category = NormalizeCategory(category),
                Status = item["Status"]?.ToString() ?? "Unknown",
                Manufacturer = item["Manufacturer"]?.ToString() ?? "Unknown"
            });
        }

        return devices
            .OrderBy(device => device.Category, StringComparer.CurrentCultureIgnoreCase)
            .ThenBy(device => device.Name, StringComparer.CurrentCultureIgnoreCase)
            .ToArray();
    }

    private static string NormalizeCategory(string category) => category.ToUpperInvariant() switch
    {
        "AUDIOENDPOINT" or "MEDIA" => "Audio",
        "HIDCLASS" => "HID / Controller",
        "IMAGE" or "CAMERA" => "Camera",
        _ => category
    };
}
