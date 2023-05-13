using Gladhen6.Enums;
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
    public DelegateCommand RefreshCommand => _refreshCommand ??=
        new DelegateCommand(ExecuteRefreshCommand, CanExecutePackage).ObservesProperty(() => Status);

    async void ExecuteRefreshCommand()
    {
        IsLocalEnable = false;
        Status = "Getting Packages";
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
        SearchCommand.Execute();
        IsLocalEnable = true;
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

    private bool CanExecutePackage()
    {
        return Status == "Ready";
    }

    private DelegateCommand? _searchCommand;
    public DelegateCommand SearchCommand => _searchCommand ??=
        new DelegateCommand(ExecuteSearchCommand, CanExecutePackage).ObservesProperty(() => Status);

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
        new DelegateCommand(ExecuteAddPackageCommand, CanExecuteAddDeletePackageCommand)
        .ObservesProperty(() => SelectedIndex).ObservesProperty(() => Status);

    async void ExecuteAddPackageCommand()
    {
        var packageName = Packages[SelectedIndex].Name;
        if (packageName is null)
            return;
        IsLocalEnable = false;
        Status = $"Adding {packageName}";
        var res = await _packageServices.AddPackageAsync(packageName);
        if (res == ResultEnum.Success)
            MessageBox.Show($"Succesfull adding {packageName}");
        else
            MessageBox.Show($"Failed adding {packageName}");
        Status = "Ready";
        RefreshCommand.Execute();
        IsLocalEnable = true;
    }

    bool CanExecuteAddDeletePackageCommand()
    {
        return SelectedIndex > -1 && SelectedIndex < Packages.Count && Status == "Ready";
    }

    private DelegateCommand? _deletePackageCommand;
    public DelegateCommand DeletePackageCommand => _deletePackageCommand ??=
        new DelegateCommand(ExecuteDeletePackageCommand, CanExecuteAddDeletePackageCommand)
        .ObservesProperty(() => SelectedIndex).ObservesProperty(() => Status);

    async void ExecuteDeletePackageCommand()
    {
        var packageName = Packages[SelectedIndex].Name;
        if (packageName is null)
            return;
        IsLocalEnable = false;
        Status = $"Deleting {packageName}";
        var res = await _packageServices.DeletePackageAsync(packageName);
        if (res == ResultEnum.Success)
            MessageBox.Show($"Succesfull deleting {packageName}");
        else
            MessageBox.Show($"Failed deleting {packageName}");
        Status = "Ready";
        RefreshCommand.Execute();
        IsLocalEnable = true;
    }

    private bool _isLocalEnable = true;
    public bool IsLocalEnable
    {
        get => _isLocalEnable;
        set => SetProperty(ref _isLocalEnable, value);
    }
}
