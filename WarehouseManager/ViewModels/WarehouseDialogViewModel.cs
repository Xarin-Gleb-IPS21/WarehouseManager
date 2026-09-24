using WarehouseManager.Domain.Models;

namespace WarehouseManager.App.ViewModels;

public class WarehouseDialogViewModel : ViewModelBase
{
    private string _name = string.Empty;
    private string _address = string.Empty;

    public string WarehouseName
    {
        get => _name;
        set => Set(ref _name, value);
    }

    public string Address
    {
        get => _address;
        set => Set(ref _address, value);
    }

    public string Title => IsEdit ? "Редактирование склада" : "Новый склад";
    public bool IsEdit { get; }

    public WarehouseDialogViewModel(Warehouse? wh)
    {
        IsEdit = wh != null;
        if (wh != null) { _name = wh.Name; _address = wh.Address; }
    }
}