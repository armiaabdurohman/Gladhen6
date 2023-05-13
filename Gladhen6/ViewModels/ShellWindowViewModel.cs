using Gladhen6.Models;
using Gladhen6.Services;
using ImTools;
using Prism.Commands;
using Prism.Mvvm;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using System.Windows;

namespace Gladhen6.ViewModels;

public class ShellWindowViewModel : BindableBase
{
    private List<PackageModel>? _allPackages;
    private List<PackageModel>? _localPackages;

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
        if (IsLocal)
        {
            _localPackages = new List<PackageModel>(await _packageServices.GetInstaledPackagesAsync());
            Packages = _localPackages;
        }
        else
        {
            _allPackages = new List<PackageModel>(await _packageServices.GetAllPackageAsync());
            Packages = _allPackages;
        }
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
        if (IsLocal)
            Packages = _localPackages!.Where(x => x.Name!.ToLower().StartsWith(SearchText.ToLower().Trim())).ToList();
        else
            Packages = _allPackages!.Where(x => x.Name!.ToLower().StartsWith(SearchText.ToLower().Trim())).ToList();
    }

    private bool _isLocal;
    public bool IsLocal
    {
        get => _isLocal;
        set
        {
            SetProperty(ref _isLocal, value);
            RefreshCommand.Execute();
            SearchCommand.Execute();
        }
    }

    private int _selectedIndex;
    public int SelectedIndex
    {
        get => _selectedIndex;
        set => SetProperty(ref _selectedIndex, value);
    }

    private DelegateCommand? _addPackageCommand;
    public DelegateCommand AddPackageCommand => _addPackageCommand ??=
        new DelegateCommand(ExecuteAddPackageCommand, CanExecuteAddDeletePackageCommand).ObservesProperty(() => SelectedIndex);

    void ExecuteAddPackageCommand()
    {
        // TODO: Add logic for add command
    }

    bool CanExecuteAddDeletePackageCommand()
    {
        return SelectedIndex > -1 && SelectedIndex < Packages.Count;
    }

    private DelegateCommand? _deletePackageCommand;
    public DelegateCommand DeletePackageCommand => _deletePackageCommand ??=
        new DelegateCommand(ExecuteDeletePackageCommand, CanExecuteAddDeletePackageCommand).ObservesProperty(() => SelectedIndex);

    void ExecuteDeletePackageCommand()
    {
        // TODO: Add logic for delete command
    }
}
