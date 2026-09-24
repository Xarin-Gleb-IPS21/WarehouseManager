using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseManager.Domain.Models;

public enum InvoiceType { Incoming, Outgoing }
public enum InvoiceStatus { Draft, Posted }

public class Invoice
{
    public int Id { get; set; }
    public string Number { get; set; } = string.Empty;
    public DateTime Date { get; set; } = DateTime.Now;
    public InvoiceType Type { get; set; }
    public InvoiceStatus Status { get; set; } = InvoiceStatus.Draft;
    public int WarehouseId { get; set; }
    public string Comment { get; set; } = string.Empty;
    public List<InvoiceItem> Items { get; set; } = new();

    public decimal Total => Items.Sum(i => i.Total);
}
