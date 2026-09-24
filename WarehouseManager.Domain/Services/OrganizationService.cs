using WarehouseManager.Domain.Models;

namespace WarehouseManager.Domain.Services;

public class OrganizationService : IDataService<Organization>
{
    private readonly DataStore _store;
    public OrganizationService(DataStore store) => _store = store;

    public IReadOnlyList<Organization> GetAll() => _store.Organizations.ToList();

    public Organization? GetById(int id) =>
        _store.Organizations.FirstOrDefault(o => o.Id == id);

    public void Add(Organization item)
    {
        if (item is null) throw new ArgumentNullException(nameof(item));
        if (string.IsNullOrWhiteSpace(item.Name))
            throw new ArgumentException("Наименование организации не может быть пустым.", nameof(item));

        item.Id = _store.NextOrganizationId();
        _store.Organizations.Add(item);
    }

    public void Update(Organization item)
    {
        if (item is null) throw new ArgumentNullException(nameof(item));
        if (string.IsNullOrWhiteSpace(item.Name))
            throw new ArgumentException("Наименование организации не может быть пустым.", nameof(item));

        var existing = GetById(item.Id)
            ?? throw new InvalidOperationException("Организация не найдена.");
        existing.Name = item.Name;
    }

    public void Delete(int id)
    {
        var org = GetById(id);
        if (org is null) return;

        var warehouseIds = _store.Warehouses
            .Where(w => w.OrganizationId == id)
            .Select(w => w.Id)
            .ToList();

        foreach (var p in _store.Products.Where(p => warehouseIds.Contains(p.WarehouseId)).ToList())
            _store.Products.Remove(p);
        foreach (var w in _store.Warehouses.Where(w => w.OrganizationId == id).ToList())
            _store.Warehouses.Remove(w);

        _store.Organizations.Remove(org);
    }
}