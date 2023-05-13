using Gladhen6.Enums;
using Gladhen6.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Gladhen6.Services;

public class PackageServices : IPackageServices
{
    private static bool CheckVersion(ref string? str)
    {
        if (str is null || !Regex.IsMatch(str, @"^[0-9.#-]+$"))
        {
            str = null;
            return false;
        }
        return true;
    }

    private static async Task<string> ExecutePackage(string args)
    {
        var proces = new Process();
        proces.StartInfo.FileName = "vcpkg";
        proces.StartInfo.Arguments = args;
        proces.StartInfo.RedirectStandardOutput = true;
        proces.StartInfo.UseShellExecute = false;
        proces.StartInfo.CreateNoWindow = true;
        proces.Start();

        var output = await proces.StandardOutput.ReadToEndAsync();
        await proces.WaitForExitAsync();
        proces.Close();
        return output;
    }

    private static IEnumerable<PackageModel> ParsePackage(string output)
    {
        var lines = output.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        foreach (var line in lines)
        {
            var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var name = parts[0];
            var version = parts.ElementAtOrDefault(1);
            var desc = new StringBuilder();
            if (!CheckVersion(ref version))
                desc.Append(parts.ElementAtOrDefault(1));
            for (int i = 2; i < parts.Length; i++)
                desc.Append($"{parts[i]} ");
            yield return new PackageModel { Name = name, Version = version, Description = desc.ToString() };
        }
    }

    public async Task<List<PackageModel>> GetInstaledPackagesAsync()
    {
        var output = await ExecutePackage("list --x-full-desc");
        return ParsePackage(output).ToList();
    }

    public async Task<List<PackageModel>> GetAllPackageAsync()
    {
        var output = await ExecutePackage("search --x-full-desc");
        return ParsePackage(output).SkipLast(2).ToList();
    }

    public async Task<ResultEnum> AddPackageAsync(string packageName)
    {
        var output = await ExecutePackage($"install {packageName}");
        if (output.Contains("Error") || output.Contains("failed"))
            return ResultEnum.Error;
        return ResultEnum.Success;
    }

    public async Task<ResultEnum> DeletePackageAsync(string packageName)
    {
        var output = await ExecutePackage($"remove {packageName}");
        if (output.Contains("Error") || output.Contains("failed"))
            return ResultEnum.Error;
        return ResultEnum.Success;
    }
}
