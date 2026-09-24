using System.Collections.ObjectModel;
using System.Windows;
using WarehouseManager.App.Commands;
using WarehouseManager.Domain.Models;

namespace WarehouseManager.App.ViewModels;

public class InvoiceDialogViewModel : ViewModelBase
{
    private InvoiceType _type = InvoiceType.Incoming;
    private string _comment = string.Empty;
    private InvoiceItem? _selectedItem;

    public Warehouse Warehouse { get; }
    public Invoice? Editing { get; }

    public ObservableCollection<InvoiceItem> Items { get; } = new();
    public Array Types { get; } = Enum.GetValues(typeof(InvoiceType));

    public InvoiceType Type
    {
        get => _type;
        set => Set(ref _type, value);
    }

    public string Comment
    {
        get => _comment;
        set => Set(ref _comment, value);
    }

    public InvoiceItem? SelectedItem
    {
        get => _selectedItem;
        set => Set(ref _selectedItem, value);
    }

    public string Title => Editing is null ? "Новая накладная" : $"Накладная {Editing.Number}";
    public bool IsEdit => Editing != null;

    public RelayCommand AddItemCommand { get; }
    public RelayCommand RemoveItemCommand { get; }

    public InvoiceDialogViewModel(Warehouse warehouse, Invoice? editing)
    {
        Warehouse = warehouse;
        Editing = editing;

        if (editing != null)
        {
            _type = editing.Type;
            _comment = editing.Comment;
            foreach (var item in editing.Items)
                Items.Add(new InvoiceItem
                {
                    ProductId = item.ProductId,
                    Article = item.Article,
                    ProductName = item.ProductName,
                    Quantity = item.Quantity,
                    Price = item.Price
                });
        }

        AddItemCommand = new RelayCommand(_ => AddItem());
        RemoveItemCommand = new RelayCommand(_ => Items.Remove(SelectedItem!), _ => SelectedItem != null);
    }

    private void AddItem()
    {
        var vm = new InvoiceItemDialogViewModel(Warehouse);
        var dlg = new Views.InvoiceItemDialogView { DataContext = vm, Owner = Application.Current.MainWindow };
        if (dlg.ShowDialog() == true && vm.Result != null)
        {
            var existing = Items.FirstOrDefault(i => i.ProductId == vm.Result.ProductId);
            if (existing != null)
                existing.Quantity += vm.Result.Quantity;
            else
                Items.Add(vm.Result);
        }
    }

    /// <summary>Сохраняет накладную. Возвращает true при успехе.</summary>
    public bool TrySave(out string? error)
    {
        error = null;
        if (Items.Count == 0)
        {
            error = "Добавьте хотя бы один товар в накладную.";
            return false;
        }

        try
        {
            if (Editing is null)
            {
                var invoice = new Invoice
                {
                    WarehouseId = Warehouse.Id,
                    Type = Type,
                    Comment = Comment,
                    Items = Items.Select(i => new InvoiceItem
                    {
                        ProductId = i.ProductId,
                        Article = i.Article,
                        ProductName = i.ProductName,
                        Quantity = i.Quantity,
                        Price = i.Price
                    }).ToList()
                };
                App.Invoices.Add(invoice);
            }
            else
            {
                Editing.Type = Type;
                Editing.Comment = Comment;
                Editing.Items = Items.Select(i => new InvoiceItem
                {
                    ProductId = i.ProductId,
                    Article = i.Article,
                    ProductName = i.ProductName,
                    Quantity = i.Quantity,
                    Price = i.Price
                }).ToList();
                App.Invoices.Update(Editing);
            }
            return true;
        }
        catch (Exception ex)
        {
            error = ex.Message;
            return false;
        }
    }
}