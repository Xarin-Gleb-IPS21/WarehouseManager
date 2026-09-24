using System.Globalization;
using System.IO;
using WarehouseManager.Domain.Models;

namespace WarehouseManager.Domain.Services;

public class ProductService : IDataService<Product>
{
    private readonly DataStore _store;
    public ProductService(DataStore store) => _store = store;

    public IReadOnlyList<Product> GetAll() => _store.Products.ToList();

    public Product? GetById(int id) =>
        _store.Products.FirstOrDefault(p => p.Id == id);

    public IReadOnlyList<Product> GetByWarehouse(int warehouseId) =>
        _store.Products.Where(p => p.WarehouseId == warehouseId).ToList();

    public void Add(Product item)
    {
        Validate(item);
        if (!_store.Warehouses.Any(w => w.Id == item.WarehouseId))
            throw new InvalidOperationException("Склад не найден.");

        item.Id = _store.NextProductId();
        _store.Products.Add(item);
    }

    public void Update(Product item)
    {
        Validate(item);
        var existing = GetById(item.Id)
            ?? throw new InvalidOperationException("Товар не найден.");
        existing.Article = item.Article;
        existing.Name = item.Name;
        existing.Category = item.Category;
        existing.Manufacturer = item.Manufacturer;
        existing.Supplier = item.Supplier;
        existing.Price = item.Price;
        existing.Stock = item.Stock;
        existing.DiscountPercent = item.DiscountPercent;
        existing.Description = item.Description;
        existing.ImagePath = item.ImagePath;
    }

    public void Delete(int id)
    {
        var p = GetById(id);
        if (p is null) return;
        _store.Products.Remove(p);
    }

    public void ChangeStock(int productId, int delta)
    {
        var p = GetById(productId)
            ?? throw new InvalidOperationException("Товар не найден.");
        var next = p.Stock + delta;
        if (next < 0)
            throw new InvalidOperationException("Недостаточно товара на складе.");
        p.Stock = next;
    }

    /// <summary>
    /// Импорт товаров из CSV. Ожидаемый формат строк (разделитель ;):
    /// Article;Name;Category;Manufacturer;Supplier;Price;Stock;Discount
    /// </summary>
    public int ImportFromCsv(string path, int warehouseId)
    {
        if (!File.Exists(path))
            throw new FileNotFoundException("Файл не найден.", path);

        var lines = File.ReadAllLines(path);
        int imported = 0;
        for (int i = 0; i < lines.Length; i++)
        {
            var line = lines[i].Trim();
            if (string.IsNullOrEmpty(line)) continue;
            if (i == 0 && line.StartsWith("Article", StringComparison.OrdinalIgnoreCase)) continue;

            var parts = line.Split(';');
            if (parts.Length < 6) continue;

            var product = new Product
            {
                WarehouseId = warehouseId,
                Article = parts[0].Trim(),
                Name = parts[1].Trim(),
                Category = parts[2].Trim(),
                Manufacturer = parts[3].Trim(),
                Supplier = parts[4].Trim(),
                Price = decimal.Parse(parts[5].Trim(), CultureInfo.InvariantCulture),
                Stock = parts.Length > 6 ? int.Parse(parts[6].Trim()) : 0,
                DiscountPercent = parts.Length > 7
                    ? double.Parse(parts[7].Trim(), CultureInfo.InvariantCulture)
                    : 0
            };
            Add(product);
            imported++;
        }
        return imported;
    }

    private static void Validate(Product item)
    {
        if (item is null) throw new ArgumentNullException(nameof(item));
        if (string.IsNullOrWhiteSpace(item.Article))
            throw new ArgumentException("Артикул не может быть пустым.", nameof(item));
        if (string.IsNullOrWhiteSpace(item.Name))
            throw new ArgumentException("Наименование не может быть пустым.", nameof(item));
        if (item.Price < 0)
            throw new ArgumentException("Цена не может быть отрицательной.", nameof(item));
        if (item.Stock < 0)
            throw new ArgumentException("Остаток не может быть отрицательным.", nameof(item));
        if (item.DiscountPercent < 0 || item.DiscountPercent > 100)
            throw new ArgumentException("Скидка должна быть в диапазоне 0–100%.", nameof(item));
    }
}