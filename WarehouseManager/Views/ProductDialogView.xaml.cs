using System.Windows;
using WarehouseManager.App.ViewModels;

namespace WarehouseManager.App.Views;

public partial class ProductDialogView : Window
{
    public ProductDialogView() => InitializeComponent();

    private void OnSave(object sender, RoutedEventArgs e)
    {
        if (DataContext is ProductDialogViewModel vm)
        {
            // валидация выполняется повторно уже при TryBuild в вызывающем окне,
            // но для UX проверим пустые поля сразу
            if (string.IsNullOrWhiteSpace(vm.Article) || string.IsNullOrWhiteSpace(vm.Name))
            {
                MessageBox.Show("Артикул и наименование не могут быть пустыми.",
                    "Проверка данных", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            DialogResult = true;
        }
    }

    private void OnCancel(object sender, RoutedEventArgs e) => DialogResult = false;
}