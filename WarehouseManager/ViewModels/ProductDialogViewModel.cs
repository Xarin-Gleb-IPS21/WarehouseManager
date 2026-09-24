using System.Globalization;
using WarehouseManager.Domain.Models;

namespace WarehouseManager.App.ViewModels;

public class ProductDialogViewModel : ViewModelBase
{
    private string _article = string.Empty;
    private string _name = string.Empty;
    private string _category = string.Empty;
    private string _manufacturer = string.Empty;
    private string _supplier = string.Empty;
    private string _price = "0";
    private string _stock = "0";
    private string _discount = "0";
    private string _description = string.Empty;
    private string _imagePath = string.Empty;

    public string Article { get => _article; set => Set(ref _article, value); }
    public string Name { get => _name; set => Set(ref _name, value); }
    public string Category { get => _category; set => Set(ref _category, value); }
    public string Manufacturer { get => _manufacturer; set => Set(ref _manufacturer, value); }
    public string Supplier { get => _supplier; set => Set(ref _supplier, value); }
    public string Price { get => _price; set => Set(ref _price, value); }
    public string Stock { get => _stock; set => Set(ref _stock, value); }
    public string Discount { get => _discount; set => Set(ref _discount, value); }
    public string Description { get => _description; set => Set(ref _description, value); }
    public string ImagePath { get => _imagePath; set => Set(ref _imagePath, value); }

    public string Title => IsEdit ? "Редактирование товара" : "Новый товар";
    public bool IsEdit { get; }

    public ProductDialogViewModel(Product? product)
    {
        IsEdit = product != null;
        if (product is null) return;

        _article = product.Article;
        _name = product.Name;
        _category = product.Category;
        _manufacturer = product.Manufacturer;
        _supplier = product.Supplier;
        _price = product.Price.ToString(CultureInfo.InvariantCulture);
        _stock = product.Stock.ToString(CultureInfo.InvariantCulture);
        _discount = product.DiscountPercent.ToString(CultureInfo.InvariantCulture);
        _description = product.Description;
        _imagePath = product.ImagePath;
    }

    /// <summary>Пробует собрать Product; при ошибке возвращает null и текст ошибки.</summary>
    public Product? TryBuild(int warehouseId, out string? error)
    {
        error = null;

        if (string.IsNullOrWhiteSpace(Article)) { error = "Артикул не может быть пустым."; return null; }
        if (string.IsNullOrWhiteSpace(Name)) { error = "Наименование не может быть пустым."; return null; }

        if (!decimal.TryParse(Price, NumberStyles.Any, CultureInfo.InvariantCulture, out var price) || price < 0)
        { error = "Цена должна быть неотрицательным числом."; return null; }

        if (!int.TryParse(Stock, NumberStyles.Any, CultureInfo.InvariantCulture, out var stock) || stock < 0)
        { error = "Остаток должен быть неотрицательным целым числом."; return null; }

        if (!double.TryParse(Discount, NumberStyles.Any, CultureInfo.InvariantCulture, out var discount)
            || discount < 0 || discount > 100)
        { error = "Скидка должна быть числом в диапазоне 0–100."; return null; }

        return new Product
        {
            WarehouseId = warehouseId,
            Article = Article.Trim(),
            Name = Name.Trim(),
            Category = Category.Trim(),
            Manufacturer = Manufacturer.Trim(),
            Supplier = Supplier.Trim(),
            Price = price,
            Stock = stock,
            DiscountPercent = discount,
            Description = Description,
            ImagePath = ImagePath
        };
    }
}