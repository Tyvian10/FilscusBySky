using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FilscusBySky.MAUI.Services;
using FilscusBySky.Models;
using System.Collections.ObjectModel;

namespace FilscusBySky.MAUI.ViewModels;

public partial class RekeningenViewModel : ObservableObject
{
    private readonly ApiService _apiService;

    private ObservableCollection<Rekening> _rekeningen = new();
    public ObservableCollection<Rekening> Rekeningen
    {
        get => _rekeningen;
        set => SetProperty(ref _rekeningen, value);
    }

    private bool _isLoading;
    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    public RekeningenViewModel(ApiService apiService)
    {
        _apiService = apiService;
    }

    [RelayCommand]
    public async Task LaadRekeningenAsync()
    {
        IsLoading = true;
        var result = await _apiService.GetRekeningenAsync();
        Rekeningen = new ObservableCollection<Rekening>(result);
        IsLoading = false;
    }
}