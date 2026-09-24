using System.Collections.ObjectModel;
using System.Windows;
using WarehouseManager.App.Commands;
using WarehouseManager.Domain.Models;

namespace WarehouseManager.App.ViewModels;

public class MainViewModel : ViewModelBase
{
    private Organization? _selected;

    public ObservableCollection<Organization> Organizations { get; } = new();

    public Organization? SelectedOrganization
    {
        get => _selected;
        set => Set(ref _selected, value);
    }

    public RelayCommand AddCommand { get; }
    public RelayCommand EditCommand { get; }
    public RelayCommand DeleteCommand { get; }
    public RelayCommand OpenCommand { get; }

    public MainViewModel()
    {
        Reload();
        AddCommand = new RelayCommand(_ => AddOrganization());
        EditCommand = new RelayCommand(_ => EditOrganization(), _ => SelectedOrganization != null);
        DeleteCommand = new RelayCommand(_ => DeleteOrganization(), _ => SelectedOrganization != null);
        OpenCommand = new RelayCommand(_ => OpenWarehouses(), _ => SelectedOrganization != null);
    }

    private void Reload()
    {
        Organizations.Clear();
        foreach (var o in App.Organizations.GetAll())
            Organizations.Add(o);
    }

    private void AddOrganization()
    {
        var vm = new OrganizationDialogViewModel(null);
        var dlg = new Views.OrganizationDialogView
        {
            DataContext = vm,
            Owner = Application.Current.MainWindow
        };
        if (dlg.ShowDialog() == true)
        {
            App.Organizations.Add(new Organization { Name = vm.OrganizationName });
            Reload();
        }
    }

    private void EditOrganization()
    {
        if (SelectedOrganization is null) return;
        var vm = new OrganizationDialogViewModel(SelectedOrganization);
        var dlg = new Views.OrganizationDialogView
        {
            DataContext = vm,
            Owner = Application.Current.MainWindow
        };
        if (dlg.ShowDialog() == true)
        {
            App.Organizations.Update(new Organization
            {
                Id = SelectedOrganization.Id,
                Name = vm.OrganizationName
            });
            Reload();
        }
    }

    private void DeleteOrganization()
    {
        if (SelectedOrganization is null) return;
        var vm = new ConfirmDeleteViewModel(SelectedOrganization.Name);
        var dlg = new Views.ConfirmDeleteDialogView
        {
            DataContext = vm,
            Owner = Application.Current.MainWindow
        };
        if (dlg.ShowDialog() == true)
        {
            App.Organizations.Delete(SelectedOrganization.Id);
            Reload();
        }
    }

    private void OpenWarehouses()
    {
        if (SelectedOrganization is null) return;
        var vm = new WarehousesViewModel(SelectedOrganization);
        var wnd = new Views.WarehousesWindow
        {
            DataContext = vm,
            Owner = Application.Current.MainWindow
        };
        wnd.ShowDialog();
    }
}