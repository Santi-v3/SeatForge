using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SeatForge.Models;

public sealed class DeviceRecord : INotifyPropertyChanged
{
    private string _assignedSeat = "Unassigned";

    public required string Id { get; init; }
    public required string Name { get; init; }
    public required string Category { get; init; }
    public string Status { get; init; } = "Unknown";
    public string Manufacturer { get; init; } = "Unknown";

    public string AssignedSeat
    {
        get => _assignedSeat;
        set
        {
            if (_assignedSeat == value)
            {
                return;
            }

            _assignedSeat = value;
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
