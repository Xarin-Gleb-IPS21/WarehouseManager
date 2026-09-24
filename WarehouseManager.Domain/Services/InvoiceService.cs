using WarehouseManager.Domain.Models;

namespace WarehouseManager.Domain.Services;

public class InvoiceService : IDataService<Invoice>
{
    private readonly DataStore _store;
    private readonly ProductService _productService;

    public InvoiceService(DataStore store, ProductService productService)
    {
        _store = store;
        _productService = productService;
    }

    public IReadOnlyList<Invoice> GetAll() => _store.Invoices.ToList();

    public Invoice? GetById(int id) =>
        _store.Invoices.FirstOrDefault(i => i.Id == id);

    public IReadOnlyList<Invoice> GetByWarehouse(int warehouseId) =>
        _store.Invoices.Where(i => i.WarehouseId == warehouseId).ToList();

    public void Add(Invoice item)
    {
        if (item is null) throw new ArgumentNullException(nameof(item));
        if (item.Items.Count == 0)
            throw new ArgumentException("Накладная должна содержать хотя бы одну позицию.", nameof(item));

        item.Id = _store.NextInvoiceId();
        item.Number = $"Н-{item.Id:D4}";
        item.Date = DateTime.Now;
        item.Status = InvoiceStatus.Draft;
        _store.Invoices.Add(item);
    }

    public void Update(Invoice item)
    {
        if (item is null) throw new ArgumentNullException(nameof(item));
        var existing = GetById(item.Id)
            ?? throw new InvalidOperationException("Накладная не найдена.");
        if (existing.Status == InvoiceStatus.Posted)
            throw new InvalidOperationException("Проведённую накладную нельзя редактировать.");

        existing.Type = item.Type;
        existing.Comment = item.Comment;
        existing.Items = item.Items.ToList();
    }

    public void Delete(int id)
    {
        var inv = GetById(id);
        if (inv is null) return;
        if (inv.Status == InvoiceStatus.Posted)
            throw new InvalidOperationException("Нельзя удалить проведённую накладную.");

        _store.Invoices.Remove(inv);
    }

    public void Post(int invoiceId)
    {
        var inv = GetById(invoiceId)
            ?? throw new InvalidOperationException("Накладная не найдена.");
        if (inv.Status == InvoiceStatus.Posted)
            throw new InvalidOperationException("Накладная уже проведена.");

        if (inv.Type == InvoiceType.Outgoing)
        {
            foreach (var item in inv.Items)
            {
                var product = _productService.GetById(item.ProductId)
                    ?? throw new InvalidOperationException($"Товар {item.Article} не найден.");
                if (product.Stock < item.Quantity)
                    throw new InvalidOperationException(
                        $"Недостаточно товара «{product.Name}» на складе.");
            }
        }

        foreach (var item in inv.Items)
        {
            var delta = inv.Type == InvoiceType.Incoming ? item.Quantity : -item.Quantity;
            _productService.ChangeStock(item.ProductId, delta);
        }

        inv.Status = InvoiceStatus.Posted;
    }

    public void Unpost(int invoiceId)
    {
        var inv = GetById(invoiceId)
            ?? throw new InvalidOperationException("Накладная не найдена.");
        if (inv.Status != InvoiceStatus.Posted)
            throw new InvalidOperationException("Накладная не проведена.");

        foreach (var item in inv.Items)
        {
            var delta = inv.Type == InvoiceType.Incoming ? -item.Quantity : item.Quantity;
            _productService.ChangeStock(item.ProductId, delta);
        }

        inv.Status = InvoiceStatus.Draft;
    }
}