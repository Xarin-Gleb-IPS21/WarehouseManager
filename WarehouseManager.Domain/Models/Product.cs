using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseManager.Domain.Models;

public class Product
{
    public int Id { get; set; }
    public int WarehouseId { get; set; }
    public string Article { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Manufacturer { get; set; } = string.Empty;
    public string Supplier { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public double DiscountPercent { get; set; }
    public string Description { get; set; } = string.Empty;
    public string ImagePath { get; set; } = string.Empty;
}
