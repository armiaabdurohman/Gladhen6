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

    public async Task<List<PackageModel>> GetPackagesAsync()
    {
        var proces = new Process();
        proces.StartInfo.FileName = "vcpkg";
        proces.StartInfo.Arguments = "list --x-full-desc";
        proces.StartInfo.RedirectStandardOutput = true;
        proces.StartInfo.UseShellExecute = false;
        proces.StartInfo.CreateNoWindow = true;
        proces.Start();

        var output = await proces.StandardOutput.ReadToEndAsync();
        await proces.WaitForExitAsync();
        proces.Close();
        var packages = new List<PackageModel>();
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
            packages.Add(new PackageModel { Name = name, Version = version, Description = desc.ToString() });
        }
        return packages;
    }
}
