using WarehouseManager.Domain.Models;

namespace WarehouseManager.App.ViewModels;

public class OrganizationDialogViewModel : ViewModelBase
{
    private string _name = string.Empty;

    public string OrganizationName
    {
        get => _name;
        set => Set(ref _name, value);
    }

    public string Title => IsEdit ? "Редактирование организации" : "Новая организация";
    public bool IsEdit { get; }

    public OrganizationDialogViewModel(Organization? org)
    {
        IsEdit = org != null;
        if (org != null) _name = org.Name;
    }
}