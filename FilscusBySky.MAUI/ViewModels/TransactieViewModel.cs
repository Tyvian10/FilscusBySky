using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FilscusBySky.MAUI.Services;
using FilscusBySky.Models;
using System.Collections.ObjectModel;

namespace FilscusBySky.MAUI.ViewModels;

public partial class TransactiesViewModel : ObservableObject
{
    private readonly ApiService _apiService;

    private ObservableCollection<Transactie> _transacties = new();
    public ObservableCollection<Transactie> Transacties
    {
        get => _transacties;
        set => SetProperty(ref _transacties, value);
    }

    private bool _isLoading;
    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    private int _rekeningId;
    public int RekeningId
    {
        get => _rekeningId;
        set => SetProperty(ref _rekeningId, value);
    }

    public TransactiesViewModel(ApiService apiService)
    {
        _apiService = apiService;
    }

    [RelayCommand]
    public async Task LaadTransactiesAsync()
    {
        IsLoading = true;
        var result = await _apiService.GetTransactiesAsync(RekeningId);
        Transacties = new ObservableCollection<Transactie>(result);
        IsLoading = false;
    }
}