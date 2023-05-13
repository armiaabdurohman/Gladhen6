using Gladhen6.Enums;
using Gladhen6.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gladhen6.Services;
public interface IPackageServices
{
    Task<ResultEnum> AddPackageAsync(string packageName);
    Task<ResultEnum> DeletePackageAsync(string packageName);

    /// <summary>
    /// Get all aviable package from vcpkg
    /// </summary>
    /// <returns>list of package</returns>
    Task<List<PackageModel>> GetAllPackageAsync();
    /// <summary>
    /// Get only installed package from vcpkg
    /// </summary>
    /// <returns>list of package</returns>
    Task<List<PackageModel>> GetInstaledPackagesAsync();
}