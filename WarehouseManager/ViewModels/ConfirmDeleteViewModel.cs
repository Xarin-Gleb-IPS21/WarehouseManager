namespace WarehouseManager.App.ViewModels;

public class ConfirmDeleteViewModel : ViewModelBase
{
    private string _input = string.Empty;

    public string ExpectedName { get; }

    public string Input
    {
        get => _input;
        set
        {
            if (Set(ref _input, value))
                OnPropertyChanged(nameof(IsMatch));
        }
    }

    public bool IsMatch =>
        string.Equals(Input.Trim(), ExpectedName, StringComparison.Ordinal);

    public ConfirmDeleteViewModel(string expectedName) => ExpectedName = expectedName;
}