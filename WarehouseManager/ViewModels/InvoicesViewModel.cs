using System.Collections.ObjectModel;
using System.Windows;
using WarehouseManager.App.Commands;
using WarehouseManager.Domain.Models;

namespace WarehouseManager.App.ViewModels;

public class InvoicesViewModel : ViewModelBase
{
    private Invoice? _selectedInvoice;
    private readonly Warehouse _warehouse;

    public ObservableCollection<Invoice> Invoices { get; } = new();

    public Invoice? SelectedInvoice
    {
        get => _selectedInvoice;
        set => Set(ref _selectedInvoice, value);
    }

    public string ContextInfo => $"Склад: {_warehouse.Name}";

    public RelayCommand CreateCommand { get; }
    public RelayCommand ViewCommand { get; }
    public RelayCommand EditCommand { get; }
    public RelayCommand PostCommand { get; }
    public RelayCommand UnpostCommand { get; }
    public RelayCommand DeleteCommand { get; }

    public InvoicesViewModel(Warehouse warehouse)
    {
        _warehouse = warehouse;
        Load();

        CreateCommand = new RelayCommand(_ => Create());
        ViewCommand = new RelayCommand(_ => ViewDetails(), _ => SelectedInvoice != null);
        EditCommand = new RelayCommand(_ => Edit(), _ => SelectedInvoice != null && SelectedInvoice.Status == InvoiceStatus.Draft);
        PostCommand = new RelayCommand(_ => Post(), _ => SelectedInvoice != null && SelectedInvoice.Status == InvoiceStatus.Draft);
        UnpostCommand = new RelayCommand(_ => Unpost(), _ => SelectedInvoice != null && SelectedInvoice.Status == InvoiceStatus.Posted);
        DeleteCommand = new RelayCommand(_ => Delete(), _ => SelectedInvoice != null && SelectedInvoice.Status == InvoiceStatus.Draft);
    }

    private void Load()
    {
        Invoices.Clear();
        foreach (var inv in App.Invoices.GetByWarehouse(_warehouse.Id))
            Invoices.Add(inv);
    }

    private void Create()
    {
        var vm = new InvoiceDialogViewModel(_warehouse, null);
        var dlg = new Views.InvoiceDialogView { DataContext = vm, Owner = Application.Current.MainWindow };
        if (dlg.ShowDialog() == true) Load();
    }

    private void Edit()
    {
        if (SelectedInvoice is null) return;
        var vm = new InvoiceDialogViewModel(_warehouse, SelectedInvoice);
        var dlg = new Views.InvoiceDialogView { DataContext = vm, Owner = Application.Current.MainWindow };
        if (dlg.ShowDialog() == true) Load();
    }

    private void ViewDetails()
    {
        if (SelectedInvoice is null) return;
        var dlg = new Views.InvoiceDetailsView
        {
            DataContext = SelectedInvoice,
            Owner = Application.Current.MainWindow
        };
        dlg.ShowDialog();
    }

    private void Post()
    {
        if (SelectedInvoice is null) return;
        try
        {
            App.Invoices.Post(SelectedInvoice.Id);
            Load();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Ошибка проведения", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    private void Unpost()
    {
        if (SelectedInvoice is null) return;
        try
        {
            App.Invoices.Unpost(SelectedInvoice.Id);
            Load();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Ошибка отмены", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    private void Delete()
    {
        if (SelectedInvoice is null) return;
        var res = MessageBox.Show(
            $"Удалить накладную {SelectedInvoice.Number}?",
            "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
        if (res != MessageBoxResult.Yes) return;

        try
        {
            App.Invoices.Delete(SelectedInvoice.Id);
            Load();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }
}