using System.Collections.ObjectModel;
using System.Windows;
using Microsoft.Win32;
using WarehouseManager.App.Commands;
using WarehouseManager.Domain.Models;

namespace WarehouseManager.App.ViewModels;

public class WarehousesViewModel : ViewModelBase
{
    private readonly Organization _organization;
    private Warehouse? _selectedWarehouse;
    private Product? _selectedProduct;
    private string _searchText = string.Empty;
    private string _statusText = string.Empty;

    public Organization Organization => _organization;

    public ObservableCollection<Warehouse> Warehouses { get; } = new();
    public ObservableCollection<Product> Products { get; } = new();

    public Warehouse? SelectedWarehouse
    {
        get => _selectedWarehouse;
        set
        {
            if (Set(ref _selectedWarehouse, value))
            {
                LoadProducts();
                UpdateContext();
            }
        }
    }

    public Product? SelectedProduct
    {
        get => _selectedProduct;
        set => Set(ref _selectedProduct, value);
    }

    public string SearchText
    {
        get => _searchText;
        set { if (Set(ref _searchText, value)) LoadProducts(); }
    }

    public string ContextInfo { get => _statusText; private set => Set(ref _statusText, value); }

    public RelayCommand AddWarehouseCommand { get; }
    public RelayCommand EditWarehouseCommand { get; }
    public RelayCommand DeleteWarehouseCommand { get; }
    public RelayCommand AddProductCommand { get; }
    public RelayCommand EditProductCommand { get; }
    public RelayCommand DeleteProductCommand { get; }
    public RelayCommand ImportProductsCommand { get; }
    public RelayCommand OpenInvoicesCommand { get; }
    public RelayCommand CreateInvoiceCommand { get; }

    public WarehousesViewModel(Organization org)
    {
        _organization = org;

        AddWarehouseCommand = new RelayCommand(_ => AddWarehouse());
        EditWarehouseCommand = new RelayCommand(_ => EditWarehouse(), _ => SelectedWarehouse != null);
        DeleteWarehouseCommand = new RelayCommand(_ => DeleteWarehouse(), _ => SelectedWarehouse != null);
        AddProductCommand = new RelayCommand(_ => AddProduct(), _ => SelectedWarehouse != null);
        EditProductCommand = new RelayCommand(_ => EditProduct(), _ => SelectedWarehouse != null && SelectedProduct != null);
        DeleteProductCommand = new RelayCommand(_ => DeleteProduct(), _ => SelectedWarehouse != null && SelectedProduct != null);
        ImportProductsCommand = new RelayCommand(_ => ImportProducts(), _ => SelectedWarehouse != null);
        OpenInvoicesCommand = new RelayCommand(_ => OpenInvoices(), _ => SelectedWarehouse != null);
        CreateInvoiceCommand = new RelayCommand(_ => CreateInvoice(), _ => SelectedWarehouse != null);

        LoadWarehouses();
        UpdateContext();
    }

    private void LoadWarehouses()
    {
        Warehouses.Clear();
        foreach (var w in App.Warehouses.GetByOrganization(_organization.Id))
            Warehouses.Add(w);
        SelectedWarehouse = Warehouses.FirstOrDefault();
    }

    private void LoadProducts()
    {
        Products.Clear();
        if (SelectedWarehouse is null) return;

        var query = App.Products.GetByWarehouse(SelectedWarehouse.Id);
        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            var q = SearchText.Trim();
            query = query.Where(p =>
                p.Name.Contains(q, StringComparison.OrdinalIgnoreCase) ||
                p.Article.Contains(q, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        foreach (var p in query) Products.Add(p);
    }

    private void UpdateContext()
    {
        ContextInfo = SelectedWarehouse is null
            ? $"Организация: {_organization.Name}  |  склад не выбран"
            : $"Организация: {_organization.Name}  |  склад: {SelectedWarehouse.Name}";
    }

    private void AddWarehouse()
    {
        var vm = new WarehouseDialogViewModel(null);
        var dlg = new Views.WarehouseDialogView { DataContext = vm, Owner = Application.Current.MainWindow };
        if (dlg.ShowDialog() == true)
        {
            try
            {
                App.Warehouses.Add(new Warehouse
                {
                    OrganizationId = _organization.Id,
                    Name = vm.WarehouseName.Trim(),
                    Address = vm.Address.Trim()
                });
                LoadWarehouses();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }

    private void EditWarehouse()
    {
        if (SelectedWarehouse is null) return;
        var vm = new WarehouseDialogViewModel(SelectedWarehouse);
        var dlg = new Views.WarehouseDialogView { DataContext = vm, Owner = Application.Current.MainWindow };
        if (dlg.ShowDialog() == true)
        {
            try
            {
                App.Warehouses.Update(new Warehouse
                {
                    Id = SelectedWarehouse.Id,
                    Name = vm.WarehouseName.Trim(),
                    Address = vm.Address.Trim()
                });
                LoadWarehouses();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }

    private void DeleteWarehouse()
    {
        if (SelectedWarehouse is null) return;
        var res = MessageBox.Show(
            $"Удалить склад «{SelectedWarehouse.Name}» и все его товары?",
            "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
        if (res != MessageBoxResult.Yes) return;

        App.Warehouses.Delete(SelectedWarehouse.Id);
        LoadWarehouses();
        LoadProducts();
    }

    private void AddProduct()
    {
        if (SelectedWarehouse is null) return;
        var vm = new ProductDialogViewModel(null);
        var dlg = new Views.ProductDialogView { DataContext = vm, Owner = Application.Current.MainWindow };
        if (dlg.ShowDialog() == true)
        {
            var product = vm.TryBuild(SelectedWarehouse.Id, out var error);
            if (product is null)
            {
                MessageBox.Show(error, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            try
            {
                App.Products.Add(product);
                LoadProducts();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }

    private void EditProduct()
    {
        if (SelectedProduct is null || SelectedWarehouse is null) return;
        var vm = new ProductDialogViewModel(SelectedProduct);
        var dlg = new Views.ProductDialogView { DataContext = vm, Owner = Application.Current.MainWindow };
        if (dlg.ShowDialog() == true)
        {
            var built = vm.TryBuild(SelectedWarehouse.Id, out var error);
            if (built is null)
            {
                MessageBox.Show(error, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            built.Id = SelectedProduct.Id;
            try
            {
                App.Products.Update(built);
                LoadProducts();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }

    private void DeleteProduct()
    {
        if (SelectedProduct is null) return;
        var res = MessageBox.Show(
            $"Удалить товар «{SelectedProduct.Name}»?",
            "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
        if (res != MessageBoxResult.Yes) return;

        App.Products.Delete(SelectedProduct.Id);
        LoadProducts();
    }

    private void ImportProducts()
    {
        if (SelectedWarehouse is null) return;

        var ofd = new OpenFileDialog
        {
            Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*",
            Title = "Импорт товаров из CSV"
        };
        if (ofd.ShowDialog() != true) return;

        try
        {
            var count = App.Products.ImportFromCsv(ofd.FileName, SelectedWarehouse.Id);
            LoadProducts();
            MessageBox.Show($"Импортировано товаров: {count}",
                "Импорт завершён", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Ошибка импорта", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    private void OpenInvoices()
    {
        if (SelectedWarehouse is null) return;
        var vm = new InvoicesViewModel(SelectedWarehouse);
        var wnd = new Views.InvoicesWindow { DataContext = vm, Owner = Application.Current.MainWindow };
        wnd.ShowDialog();
        LoadProducts();
    }

    private void CreateInvoice()
    {
        if (SelectedWarehouse is null) return;
        var vm = new InvoiceDialogViewModel(SelectedWarehouse, null);
        var dlg = new Views.InvoiceDialogView { DataContext = vm, Owner = Application.Current.MainWindow };
        if (dlg.ShowDialog() == true)
        {
            LoadProducts();
        }
    }
}