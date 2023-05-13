using DryIoc;
using Gladhen6.Services;
using Gladhen6.Views;
using Prism.DryIoc;
using Prism.Ioc;
using System.Windows;

namespace Gladhen6;
/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : PrismApplication
{
    protected override Window CreateShell()
    {
        return Container.Resolve<ShellWindow>();
    }

    protected override void RegisterTypes(IContainerRegistry containerRegistry)
    {
        containerRegistry.RegisterSingleton<IPackageServices, PackageServices>();
    }
}