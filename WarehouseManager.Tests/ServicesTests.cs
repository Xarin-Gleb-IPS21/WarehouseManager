using System;
using System.Linq;
using WarehouseManager.Domain.Models;
using WarehouseManager.Domain.Services;
using Xunit;

namespace WarehouseManager.Tests;

public class ServicesTests
{
    private static (DataStore store,
                    OrganizationService orgs,
                    WarehouseService whs,
                    ProductService prods,
                    InvoiceService invs) NewServices()
    {
        var store = new DataStore();
        var orgs = new OrganizationService(store);
        var whs = new WarehouseService(store);
        var prods = new ProductService(store);
        var invs = new InvoiceService(store, prods);
        return (store, orgs, whs, prods, invs);
    }

    // ---------- Organizations ----------

    [Fact]
    public void Organization_Add_ShouldIncreaseCount()
    {
        var s = NewServices();
        s.orgs.Add(new Organization { Name = "A" });
        s.orgs.Add(new Organization { Name = "B" });
        Assert.Equal(2, s.orgs.GetAll().Count);
    }

    [Fact]
    public void Organization_Add_EmptyName_Throws()
    {
        var s = NewServices();
        Assert.Throws<ArgumentException>(() => s.orgs.Add(new Organization { Name = " " }));
    }

    [Fact]
    public void Organization_GetById_ReturnsCorrect()
    {
        var s = NewServices();
        s.orgs.Add(new Organization { Name = "Ромашка" });
        var org = s.orgs.GetAll().Single();
        Assert.Equal("Ромашка", s.orgs.GetById(org.Id)!.Name);
    }

    [Fact]
    public void Organization_GetById_WrongId_ReturnsNull()
    {
        var s = NewServices();
        Assert.Null(s.orgs.GetById(12345));
    }

    [Fact]
    public void Organization_Update_ChangesName()
    {
        var s = NewServices();
        s.orgs.Add(new Organization { Name = "Старое" });
        var org = s.orgs.GetAll().Single();
        s.orgs.Update(new Organization { Id = org.Id, Name = "Новое" });
        Assert.Equal("Новое", s.orgs.GetById(org.Id)!.Name);
    }

    [Fact]
    public void Organization_Delete_RemovesOrgWithChildren()
    {
        var s = NewServices();
        s.orgs.Add(new Organization { Name = "O" });
        var org = s.orgs.GetAll().Single();
        s.whs.Add(new Warehouse { OrganizationId = org.Id, Name = "W" });
        var wh = s.whs.GetAll().Single();
        s.prods.Add(new Product { WarehouseId = wh.Id, Article = "A", Name = "N", Price = 1, Stock = 1 });

        s.orgs.Delete(org.Id);

        Assert.Empty(s.orgs.GetAll());
        Assert.Empty(s.whs.GetAll());
        Assert.Empty(s.prods.GetAll());
    }

    // ---------- Warehouses ----------

    [Fact]
    public void Warehouse_Add_RequiresExistingOrganization()
    {
        var s = NewServices();
        Assert.Throws<InvalidOperationException>(() =>
            s.whs.Add(new Warehouse { OrganizationId = 999, Name = "W" }));
    }

    [Fact]
    public void Warehouse_Add_EmptyName_Throws()
    {
        var s = NewServices();
        s.orgs.Add(new Organization { Name = "O" });
        var org = s.orgs.GetAll().Single();
        Assert.Throws<ArgumentException>(() =>
            s.whs.Add(new Warehouse { OrganizationId = org.Id, Name = "" }));
    }

    [Fact]
    public void Warehouse_GetByOrganization_Filters()
    {
        var s = NewServices();
        s.orgs.Add(new Organization { Name = "O1" });
        s.orgs.Add(new Organization { Name = "O2" });
        var o1 = s.orgs.GetAll()[0];
        var o2 = s.orgs.GetAll()[1];

        s.whs.Add(new Warehouse { OrganizationId = o1.Id, Name = "W1" });
        s.whs.Add(new Warehouse { OrganizationId = o1.Id, Name = "W2" });
        s.whs.Add(new Warehouse { OrganizationId = o2.Id, Name = "W3" });

        Assert.Equal(2, s.whs.GetByOrganization(o1.Id).Count);
        Assert.Single(s.whs.GetByOrganization(o2.Id));
    }

    [Fact]
    public void Warehouse_Update_ChangesFields()
    {
        var s = NewServices();
        s.orgs.Add(new Organization { Name = "O" });
        var org = s.orgs.GetAll().Single();
        s.whs.Add(new Warehouse { OrganizationId = org.Id, Name = "W", Address = "A" });
        var wh = s.whs.GetAll().Single();

        s.whs.Update(new Warehouse { Id = wh.Id, Name = "W2", Address = "B" });

        var updated = s.whs.GetById(wh.Id)!;
        Assert.Equal("W2", updated.Name);
        Assert.Equal("B", updated.Address);
    }

    [Fact]
    public void Warehouse_Delete_CascadesProducts()
    {
        var s = NewServices();
        s.orgs.Add(new Organization { Name = "O" });
        var org = s.orgs.GetAll().Single();
        s.whs.Add(new Warehouse { OrganizationId = org.Id, Name = "W" });
        var wh = s.whs.GetAll().Single();
        s.prods.Add(new Product { WarehouseId = wh.Id, Article = "A", Name = "N", Price = 1, Stock = 1 });

        s.whs.Delete(wh.Id);

        Assert.Empty(s.whs.GetAll());
        Assert.Empty(s.prods.GetAll());
    }

    // ---------- Products ----------

    [Fact]
    public void Product_Add_EmptyArticle_Throws()
    {
        var s = NewServices();
        s.orgs.Add(new Organization { Name = "O" });
        var org = s.orgs.GetAll().Single();
        s.whs.Add(new Warehouse { OrganizationId = org.Id, Name = "W" });
        var wh = s.whs.GetAll().Single();

        Assert.Throws<ArgumentException>(() =>
            s.prods.Add(new Product { WarehouseId = wh.Id, Article = "", Name = "N", Price = 1, Stock = 1 }));
    }

    [Fact]
    public void Product_Add_NegativePrice_Throws()
    {
        var s = NewServices();
        s.orgs.Add(new Organization { Name = "O" });
        var wh = s.whs.GetAll().SingleOrDefault();
        s.whs.Add(new Warehouse { OrganizationId = s.orgs.GetAll()[0].Id, Name = "W" });
        wh = s.whs.GetAll().Single();

        Assert.Throws<ArgumentException>(() =>
            s.prods.Add(new Product { WarehouseId = wh.Id, Article = "A", Name = "N", Price = -1, Stock = 0 }));
    }

    [Fact]
    public void Product_GetByWarehouse_Filters()
    {
        var s = NewServices();
        s.orgs.Add(new Organization { Name = "O" });
        var org = s.orgs.GetAll().Single();
        s.whs.Add(new Warehouse { OrganizationId = org.Id, Name = "W1" });
        s.whs.Add(new Warehouse { OrganizationId = org.Id, Name = "W2" });
        var w1 = s.whs.GetAll()[0];
        var w2 = s.whs.GetAll()[1];

        s.prods.Add(new Product { WarehouseId = w1.Id, Article = "A1", Name = "N1", Price = 1, Stock = 5 });
        s.prods.Add(new Product { WarehouseId = w2.Id, Article = "A2", Name = "N2", Price = 1, Stock = 5 });

        Assert.Single(s.prods.GetByWarehouse(w1.Id));
        Assert.Single(s.prods.GetByWarehouse(w2.Id));
    }

    [Fact]
    public void Product_ChangeStock_IncreasesAndDecreases()
    {
        var s = NewServices();
        s.orgs.Add(new Organization { Name = "O" });
        var org = s.orgs.GetAll().Single();
        s.whs.Add(new Warehouse { OrganizationId = org.Id, Name = "W" });
        var wh = s.whs.GetAll().Single();
        s.prods.Add(new Product { WarehouseId = wh.Id, Article = "A", Name = "N", Price = 1, Stock = 50 });
        var p = s.prods.GetAll().Single();

        s.prods.ChangeStock(p.Id, +10);
        Assert.Equal(60, s.prods.GetById(p.Id)!.Stock);

        s.prods.ChangeStock(p.Id, -20);
        Assert.Equal(40, s.prods.GetById(p.Id)!.Stock);
    }

    [Fact]
    public void Product_ChangeStock_NegativeResult_Throws()
    {
        var s = NewServices();
        s.orgs.Add(new Organization { Name = "O" });
        var org = s.orgs.GetAll().Single();
        s.whs.Add(new Warehouse { OrganizationId = org.Id, Name = "W" });
        var wh = s.whs.GetAll().Single();
        s.prods.Add(new Product { WarehouseId = wh.Id, Article = "A", Name = "N", Price = 1, Stock = 5 });
        var p = s.prods.GetAll().Single();

        Assert.Throws<InvalidOperationException>(() => s.prods.ChangeStock(p.Id, -10));
    }

    [Fact]
    public void Product_Update_ChangesFields()
    {
        var s = NewServices();
        s.orgs.Add(new Organization { Name = "O" });
        var wh = s.whs.GetAll().SingleOrDefault();
        s.whs.Add(new Warehouse { OrganizationId = s.orgs.GetAll()[0].Id, Name = "W" });
        wh = s.whs.GetAll().Single();
        s.prods.Add(new Product { WarehouseId = wh.Id, Article = "A", Name = "Old", Price = 10, Stock = 1 });
        var p = s.prods.GetAll().Single();

        s.prods.Update(new Product
        {
            Id = p.Id,
            WarehouseId = wh.Id,
            Article = p.Article,
            Name = "New",
            Price = 20,
            Stock = 1
        });

        Assert.Equal("New", s.prods.GetById(p.Id)!.Name);
        Assert.Equal(20, s.prods.GetById(p.Id)!.Price);
    }

    [Fact]
    public void Product_Delete_Removes()
    {
        var s = NewServices();
        s.orgs.Add(new Organization { Name = "O" });
        var org = s.orgs.GetAll().Single();
        s.whs.Add(new Warehouse { OrganizationId = org.Id, Name = "W" });
        var wh = s.whs.GetAll().Single();
        s.prods.Add(new Product { WarehouseId = wh.Id, Article = "A", Name = "N", Price = 1, Stock = 1 });
        var p = s.prods.GetAll().Single();

        s.prods.Delete(p.Id);
        Assert.Empty(s.prods.GetAll());
    }

    // ---------- Invoices ----------

    [Fact]
    public void Invoice_Add_EmptyItems_Throws()
    {
        var s = NewServices();
        Assert.Throws<ArgumentException>(() =>
            s.invs.Add(new Invoice { WarehouseId = 1, Type = InvoiceType.Incoming }));
    }

    [Fact]
    public void Invoice_Post_Incoming_IncreasesStock()
    {
        var s = NewServices();
        var (wh, product) = SeedOneProduct(s, stock: 10);

        var inv = new Invoice
        {
            WarehouseId = wh.Id,
            Type = InvoiceType.Incoming,
            Items = new() { new InvoiceItem { ProductId = product.Id, Article = product.Article, ProductName = product.Name, Quantity = 5, Price = product.Price } }
        };
        s.invs.Add(inv);
        s.invs.Post(inv.Id);

        Assert.Equal(15, s.prods.GetById(product.Id)!.Stock);
    }

    [Fact]
    public void Invoice_Post_Outgoing_DecreasesStock()
    {
        var s = NewServices();
        var (wh, product) = SeedOneProduct(s, stock: 10);

        var inv = new Invoice
        {
            WarehouseId = wh.Id,
            Type = InvoiceType.Outgoing,
            Items = new() { new InvoiceItem { ProductId = product.Id, Article = product.Article, ProductName = product.Name, Quantity = 4, Price = product.Price } }
        };
        s.invs.Add(inv);
        s.invs.Post(inv.Id);

        Assert.Equal(6, s.prods.GetById(product.Id)!.Stock);
    }

    [Fact]
    public void Invoice_Post_Outgoing_NotEnoughStock_Throws()
    {
        var s = NewServices();
        var (wh, product) = SeedOneProduct(s, stock: 3);

        var inv = new Invoice
        {
            WarehouseId = wh.Id,
            Type = InvoiceType.Outgoing,
            Items = new() { new InvoiceItem { ProductId = product.Id, Article = product.Article, ProductName = product.Name, Quantity = 5, Price = product.Price } }
        };
        s.invs.Add(inv);

        Assert.Throws<InvalidOperationException>(() => s.invs.Post(inv.Id));
    }

    [Fact]
    public void Invoice_Unpost_RevertsStock()
    {
        var s = NewServices();
        var (wh, product) = SeedOneProduct(s, stock: 20);

        var inv = new Invoice
        {
            WarehouseId = wh.Id,
            Type = InvoiceType.Incoming,
            Items = new() { new InvoiceItem { ProductId = product.Id, Article = product.Article, ProductName = product.Name, Quantity = 7, Price = product.Price } }
        };
        s.invs.Add(inv);
        s.invs.Post(inv.Id);
        Assert.Equal(27, s.prods.GetById(product.Id)!.Stock);

        s.invs.Unpost(inv.Id);
        Assert.Equal(20, s.prods.GetById(product.Id)!.Stock);
    }

    [Fact]
    public void Invoice_Update_Posted_Throws()
    {
        var s = NewServices();
        var (wh, product) = SeedOneProduct(s, stock: 5);
        var inv = new Invoice
        {
            WarehouseId = wh.Id,
            Type = InvoiceType.Incoming,
            Items = new() { new InvoiceItem { ProductId = product.Id, Article = product.Article, ProductName = product.Name, Quantity = 1, Price = product.Price } }
        };
        s.invs.Add(inv);
        s.invs.Post(inv.Id);

        Assert.Throws<InvalidOperationException>(() => s.invs.Update(inv));
    }

    [Fact]
    public void Invoice_Delete_Posted_Throws()
    {
        var s = NewServices();
        var (wh, product) = SeedOneProduct(s, stock: 5);
        var inv = new Invoice
        {
            WarehouseId = wh.Id,
            Type = InvoiceType.Incoming,
            Items = new() { new InvoiceItem { ProductId = product.Id, Article = product.Article, ProductName = product.Name, Quantity = 1, Price = product.Price } }
        };
        s.invs.Add(inv);
        s.invs.Post(inv.Id);

        Assert.Throws<InvalidOperationException>(() => s.invs.Delete(inv.Id));
    }

    // ---------- helpers ----------

    private static (Warehouse wh, Product product) SeedOneProduct(
        (DataStore store, OrganizationService orgs, WarehouseService whs, ProductService prods, InvoiceService invs) s,
        int stock)
    {
        s.orgs.Add(new Organization { Name = "O" });
        var org = s.orgs.GetAll().Single();
        s.whs.Add(new Warehouse { OrganizationId = org.Id, Name = "W" });
        var wh = s.whs.GetAll().Single();
        s.prods.Add(new Product
        {
            WarehouseId = wh.Id,
            Article = "A1",
            Name = "Test",
            Price = 100m,
            Stock = stock
        });
        var product = s.prods.GetAll().Single();
        return (wh, product);
    }
}