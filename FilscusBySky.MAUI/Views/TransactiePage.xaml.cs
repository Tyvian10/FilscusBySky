using FilscusBySky.MAUI.ViewModels;

namespace FilscusBySky.MAUI.Views;

public partial class TransactiesPage : ContentPage
{
    private readonly TransactiesViewModel _viewModel;

    public TransactiesPage(TransactiesViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LaadTransactiesAsync();
    }
}