# SeatForge

A Windows desktop prototype for listing connected devices and recording a local assignment to Seat 1, Seat 2 or Unassigned.

## Problem and scope

A multi-seat setup needs an inventory of displays, input devices, cameras and audio devices before any seat-specific system integration can be explored. SeatForge provides a WPF interface backed by WMI and stores chosen assignments in `%LOCALAPPDATA%\SeatForge\assignments.json`. It filters the inventory by device attributes and seat.

The current application **does not** install drivers, change the Windows registry, create user sessions or isolate devices between seats. It is an inventory and assignment prototype.

## Architecture

- `Services/DeviceInventoryService.cs`: WMI device enumeration.
- `Models/DeviceRecord.cs`: device representation.
- `Services/SeatAssignmentStore.cs`: local persistence.
- `MainWindow.xaml` and `MainWindow.xaml.cs`: WPF user interface.
- `Build.ps1` and `Run.ps1`: build and launch helpers.

## Run and build

Requires Windows, PowerShell and .NET 8 SDK. The project uses `System.Management` 8.0.0.

```powershell
./Run.ps1
./Build.ps1
```

Alternatively, use `dotnet run` from the repository root with a system-installed .NET 8 SDK. The portable SDK mentioned in older project notes is local and is not included in GitHub.

## Status and verification

Early clean-room prototype. The repository contains no automated tests or CI. Build and hardware behavior could not be verified in this non-Windows environment. Planned improvements include stronger device identifiers and import/export; driver and session work would require separate technical and legal review.

## License and author

MIT; see [LICENSE](LICENSE). Created by **Santino Burkhardt** as a personal Windows systems prototype, independent of his employer. It does not include Hydra source code or brand assets.
