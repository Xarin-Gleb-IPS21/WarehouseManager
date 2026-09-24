using System.Windows;
using WarehouseManager.Domain.Services;

namespace WarehouseManager.App;

public partial class App : Application
{
    public static DataStore Store { get; } = new();
    public static OrganizationService Organizations { get; private set; } = null!;
    public static WarehouseService Warehouses { get; private set; } = null!;
    public static ProductService Products { get; private set; } = null!;
    public static InvoiceService Invoices { get; private set; } = null!;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        Store.Seed();
        Organizations = new OrganizationService(Store);
        Warehouses = new WarehouseService(Store);
        Products = new ProductService(Store);
        Invoices = new InvoiceService(Store, Products);
    }
}