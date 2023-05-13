using Gladhen6.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gladhen6.Services;
public interface IPackageServices
{
    Task<List<PackageModel>> GetPackagesAsync();
}