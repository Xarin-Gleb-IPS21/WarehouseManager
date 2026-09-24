using WarehouseManager.Domain.Models;

namespace WarehouseManager.Domain.Services;

public class WarehouseService : IDataService<Warehouse>
{
    private readonly DataStore _store;
    public WarehouseService(DataStore store) => _store = store;

    public IReadOnlyList<Warehouse> GetAll() => _store.Warehouses.ToList();

    public Warehouse? GetById(int id) =>
        _store.Warehouses.FirstOrDefault(w => w.Id == id);

    public IReadOnlyList<Warehouse> GetByOrganization(int organizationId) =>
        _store.Warehouses.Where(w => w.OrganizationId == organizationId).ToList();

    public void Add(Warehouse item)
    {
        if (item is null) throw new ArgumentNullException(nameof(item));
        if (string.IsNullOrWhiteSpace(item.Name))
            throw new ArgumentException("Наименование склада не может быть пустым.", nameof(item));
        if (!_store.Organizations.Any(o => o.Id == item.OrganizationId))
            throw new InvalidOperationException("Организация не найдена.");

        item.Id = _store.NextWarehouseId();
        _store.Warehouses.Add(item);
    }

    public void Update(Warehouse item)
    {
        if (item is null) throw new ArgumentNullException(nameof(item));
        if (string.IsNullOrWhiteSpace(item.Name))
            throw new ArgumentException("Наименование склада не может быть пустым.", nameof(item));

        var existing = GetById(item.Id)
            ?? throw new InvalidOperationException("Склад не найден.");
        existing.Name = item.Name;
        existing.Address = item.Address;
    }

    public void Delete(int id)
    {
        var wh = GetById(id);
        if (wh is null) return;

        foreach (var p in _store.Products.Where(p => p.WarehouseId == id).ToList())
            _store.Products.Remove(p);

        _store.Warehouses.Remove(wh);
    }
}