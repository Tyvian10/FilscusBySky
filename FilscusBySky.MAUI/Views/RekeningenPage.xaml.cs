using FilscusBySky.MAUI.ViewModels;

namespace FilscusBySky.MAUI.Views;

public partial class RekeningenPage : ContentPage
{
    private readonly RekeningenViewModel _viewModel;

    public RekeningenPage(RekeningenViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LaadRekeningenAsync();
    }
}