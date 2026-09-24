using System.Collections.ObjectModel;
using WarehouseManager.Domain.Models;

namespace WarehouseManager.App.ViewModels;

public class InvoiceItemDialogViewModel : ViewModelBase
{
    private Product? _selectedProduct;
    private string _quantity = "1";

    public ObservableCollection<Product> Products { get; } = new();

    public Product? SelectedProduct
    {
        get => _selectedProduct;
        set => Set(ref _selectedProduct, value);
    }

    public string Quantity
    {
        get => _quantity;
        set => Set(ref _quantity, value);
    }

    public InvoiceItem? Result { get; private set; }

    public InvoiceItemDialogViewModel(Warehouse warehouse)
    {
        foreach (var p in App.Products.GetByWarehouse(warehouse.Id))
            Products.Add(p);
    }

    public bool TryBuild(out string? error)
    {
        error = null;
        if (SelectedProduct is null)
        {
            error = "Выберите товар.";
            return false;
        }
        if (!int.TryParse(Quantity, out var qty) || qty <= 0)
        {
            error = "Количество должно быть положительным целым числом.";
            return false;
        }

        Result = new InvoiceItem
        {
            ProductId = SelectedProduct.Id,
            Article = SelectedProduct.Article,
            ProductName = SelectedProduct.Name,
            Quantity = qty,
            Price = SelectedProduct.Price
        };
        return true;
    }
}