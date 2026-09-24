using System.Collections.ObjectModel;
using WarehouseManager.Domain.Models;

namespace WarehouseManager.Domain.Services;

public class DataStore
{
    public ObservableCollection<Organization> Organizations { get; } = new();
    public ObservableCollection<Warehouse> Warehouses { get; } = new();
    public ObservableCollection<Product> Products { get; } = new();
    public ObservableCollection<Invoice> Invoices { get; } = new();

    private int _orgId;
    private int _whId;
    private int _prodId;
    private int _invId;

    public int NextOrganizationId() => ++_orgId;
    public int NextWarehouseId() => ++_whId;
    public int NextProductId() => ++_prodId;
    public int NextInvoiceId() => ++_invId;

    public void Seed()
    {
        var o1 = new Organization { Id = NextOrganizationId(), Name = "ООО «Ромашка»" };
        var o2 = new Organization { Id = NextOrganizationId(), Name = "ИП Иванов" };
        Organizations.Add(o1);
        Organizations.Add(o2);

        var w1 = new Warehouse { Id = NextWarehouseId(), OrganizationId = o1.Id, Name = "Основной склад", Address = "г. Пермь, ул. Ленина, 1" };
        var w2 = new Warehouse { Id = NextWarehouseId(), OrganizationId = o1.Id, Name = "Резервный склад", Address = "г. Пермь, ул. Мира, 15" };
        var w3 = new Warehouse { Id = NextWarehouseId(), OrganizationId = o2.Id, Name = "Торговый зал", Address = "г. Пермь, ул. Пушкина, 7" };
        Warehouses.Add(w1);
        Warehouses.Add(w2);
        Warehouses.Add(w3);

        Products.Add(new Product
        {
            Id = NextProductId(),
            WarehouseId = w1.Id,
            Article = "EL-001",
            Name = "Смартфон Galaxy A15",
            Category = "Электроника",
            Manufacturer = "Samsung",
            Supplier = "ООО ТехноТрейд",
            Price = 19990m,
            Stock = 25,
            DiscountPercent = 5
        });
        Products.Add(new Product
        {
            Id = NextProductId(),
            WarehouseId = w1.Id,
            Article = "EL-002",
            Name = "Наушники WH-CH520",
            Category = "Электроника",
            Manufacturer = "Sony",
            Supplier = "ООО ТехноТрейд",
            Price = 4490m,
            Stock = 60,
            DiscountPercent = 0
        });
        Products.Add(new Product
        {
            Id = NextProductId(),
            WarehouseId = w2.Id,
            Article = "HM-101",
            Name = "Кресло офисное Comfort",
            Category = "Мебель",
            Manufacturer = "IKEA",
            Supplier = "ООО МебельСнаб",
            Price = 8900m,
            Stock = 12,
            DiscountPercent = 10
        });
        Products.Add(new Product
        {
            Id = NextProductId(),
            WarehouseId = w3.Id,
            Article = "FD-007",
            Name = "Кофе в зёрнах Arabica",
            Category = "Продукты",
            Manufacturer = "Lavazza",
            Supplier = "ООО ФудЛогистик",
            Price = 1290m,
            Stock = 40,
            DiscountPercent = 3
        });
    }

    public void Clear()
    {
        Organizations.Clear();
        Warehouses.Clear();
        Products.Clear();
        Invoices.Clear();
        _orgId = _whId = _prodId = _invId = 0;
    }
}