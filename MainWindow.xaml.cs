using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using SeatForge.Models;
using SeatForge.Services;

namespace SeatForge;

public partial class MainWindow : Window
{
    private const string AllSeatsLabel = "Alle";

    private readonly DeviceInventoryService _inventoryService = new();
    private readonly SeatAssignmentStore _assignmentStore;
    private readonly ICollectionView _deviceView;

    private string _searchText = string.Empty;
    private string _seatFilter = AllSeatsLabel;

    public MainWindow()
    {
        InitializeComponent();
        DataContext = this;

        // Die Ansicht blendet nur Zeilen aus; Zuordnungen bleiben am Gerätemodell erhalten.
        _deviceView = CollectionViewSource.GetDefaultView(Devices);
        _deviceView.Filter = MatchesFilter;

        var dataDirectory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "SeatForge");
        _assignmentStore = new SeatAssignmentStore(Path.Combine(dataDirectory, "assignments.json"));

        SeatFilterBox.SelectedItem = AllSeatsLabel;

        Loaded += async (_, _) =>
        {
            if (SeatFilterBox.SelectedItem is null)
            {
                SeatFilterBox.SelectedIndex = 0;
            }

            await RefreshAsync();
        };
    }

    public ObservableCollection<DeviceRecord> Devices { get; } = [];
    public IReadOnlyList<string> SeatOptions { get; } = ["Unassigned", "Seat 1", "Seat 2"];
    public IReadOnlyList<string> SeatFilters { get; } = [AllSeatsLabel, "Unassigned", "Seat 1", "Seat 2"];
    public ICollectionView DeviceView => _deviceView;

    private async void RefreshButton_Click(object sender, RoutedEventArgs e) => await RefreshAsync();

    private async Task RefreshAsync()
    {
        try
        {
            SetBusy(true, "Geräte werden gelesen …");
            var assignments = await _assignmentStore.LoadAsync();
            var devices = await _inventoryService.GetDevicesAsync();

            Devices.Clear();
            foreach (var device in devices)
            {
                if (assignments.TryGetValue(device.Id, out var assignedSeat))
                {
                    device.AssignedSeat = assignedSeat;
                }

                Devices.Add(device);
            }

            RefreshFilter();
            StatusText.Text = $"{Devices.Count} Geräte gelesen";
        }
        catch (Exception exception)
        {
            StatusText.Text = "Geräte konnten nicht gelesen werden";
            MessageBox.Show(exception.Message, "SeatForge", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            SetBusy(false);
        }
    }

    private async void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            SetBusy(true, "Zuordnungen werden gespeichert …");
            // Auch derzeit ausgeblendete Geräte werden gespeichert.
            await _assignmentStore.SaveAsync(Devices);
            StatusText.Text = "Zuordnungen wurden lokal gespeichert";
        }
        catch (Exception exception)
        {
            StatusText.Text = "Speichern fehlgeschlagen";
            MessageBox.Show(exception.Message, "SeatForge", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            SetBusy(false);
        }
    }

    private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        _searchText = SearchBox.Text;
        RefreshFilter();
    }

    private void SeatFilter_Changed(object sender, SelectionChangedEventArgs e)
    {
        if (SeatFilterBox.SelectedItem is string seat)
        {
            _seatFilter = seat;
            RefreshFilter();
        }
    }

    private void SeatSelection_Changed(object sender, SelectionChangedEventArgs e) => UpdateSummary();

    private void RefreshFilter()
    {
        if (_deviceView is IEditableCollectionView editable)
        {
            if (editable.IsEditingItem)
            {
                editable.CommitEdit();
            }

            if (editable.IsAddingNew)
            {
                editable.CommitNew();
            }
        }

        _deviceView.Refresh();
        UpdateSummary();
    }

    private bool MatchesFilter(object item)
    {
        if (item is not DeviceRecord device)
        {
            return false;
        }

        var term = _searchText.Trim();
        if (term.Length > 0 &&
            !Contains(device.Name, term) &&
            !Contains(device.Category, term) &&
            !Contains(device.Manufacturer, term))
        {
            return false;
        }

        return _seatFilter == AllSeatsLabel ||
               string.Equals(device.AssignedSeat, _seatFilter, StringComparison.OrdinalIgnoreCase);
    }

    private static bool Contains(string? value, string term) =>
        value?.Contains(term, StringComparison.OrdinalIgnoreCase) == true;

    private void UpdateSummary()
    {
        var visible = _deviceView.Cast<DeviceRecord>().Count();
        DeviceCountText.Text = $"{visible} / {Devices.Count}";
        SeatOneCountText.Text = Devices.Count(device => device.AssignedSeat == "Seat 1").ToString();
        SeatTwoCountText.Text = Devices.Count(device => device.AssignedSeat == "Seat 2").ToString();
    }

    private void SetBusy(bool isBusy, string? status = null)
    {
        DevicesGrid.IsEnabled = !isBusy;
        if (status is not null)
        {
            StatusText.Text = status;
        }
    }
}
