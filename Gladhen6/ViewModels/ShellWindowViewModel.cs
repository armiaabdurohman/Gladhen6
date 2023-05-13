using Gladhen6.Models;
using Gladhen6.Services;
using ImTools;
using Prism.Commands;
using Prism.Mvvm;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gladhen6.ViewModels;

public class ShellWindowViewModel : BindableBase
{
    private string? _title;

    public string? Title
    {
        get => _title;
        set => SetProperty(ref _title, value);
    }

    public ShellWindowViewModel(IPackageServices packageServices)
    {
        Title = "Gladhen 6";
        _packages = new List<PackageModel>();
        _packageServices = packageServices;
        RefreshCommand.Execute();
    }

    private readonly IPackageServices _packageServices;
    private List<PackageModel>? _actualPackage;

    private List<PackageModel> _packages;

    public List<PackageModel> Packages
    {
        get => _packages;
        set => SetProperty(ref _packages, value);
    }

    private DelegateCommand? _refreshCommand;
    public DelegateCommand RefreshCommand => _refreshCommand ??= new DelegateCommand(ExecuteRefreshCommand);

    async void ExecuteRefreshCommand()
    {
        Status = "Load Packages";
        Packages = await _packageServices.GetPackagesAsync();
        _actualPackage = new List<PackageModel>(Packages);
        Status = "Ready";
    }

    private string? _status;
    public string? Status
    {
        get { return _status; }
        set { SetProperty(ref _status, value); }
    }

    private string _searchText = string.Empty;
    public string SearchText
    {
        get { return _searchText; }
        set { SetProperty(ref _searchText, value); }
    }

    private DelegateCommand? _searchCommand;
    public DelegateCommand SearchCommand => _searchCommand ??= new DelegateCommand(ExecuteSearchCommand);

    void ExecuteSearchCommand()
    {
        Packages = _actualPackage!.Where(x => x.Name!.Trim().ToLower().StartsWith(SearchText.ToLower().Trim())).ToList();
    }
}
