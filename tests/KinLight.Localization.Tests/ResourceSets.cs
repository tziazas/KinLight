using System.Xml.Linq;

namespace KinLight.Localization.Tests;

/// <summary>One <c>.resx</c> family: the neutral (English) file and its per-culture translations.</summary>
public sealed record ResourceSet(string Name, IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> ValuesByCulture)
{
    /// <summary>The culture name used for the neutral file.</summary>
    public const string NeutralCulture = "en";

    /// <summary>Cultures this set is translated into, including the neutral one.</summary>
    public IEnumerable<string> Cultures => ValuesByCulture.Keys;

    /// <summary>The repository root, found by walking up from the test assembly to the solution file.</summary>
    public static string RepoRoot
    {
        get
        {
            var dir = new DirectoryInfo(AppContext.BaseDirectory);
            while (dir is not null && !File.Exists(Path.Combine(dir.FullName, "KinLight.slnx")))
            {
                dir = dir.Parent;
            }

            return dir?.FullName ?? throw new InvalidOperationException("KinLight.slnx not found above " + AppContext.BaseDirectory);
        }
    }

    /// <summary>Discovers every resource set under the given repository-relative folders.</summary>
    public static IReadOnlyList<ResourceSet> Discover(params string[] relativeFolders)
    {
        var files = relativeFolders
            .Select(f => Path.Combine(RepoRoot, f))
            .Where(Directory.Exists)
            .SelectMany(f => Directory.EnumerateFiles(f, "*.resx", SearchOption.AllDirectories))
            .Where(p => !p.Split(Path.DirectorySeparatorChar).Any(seg => seg is "bin" or "obj" or "node_modules"));

        var sets = new Dictionary<string, Dictionary<string, IReadOnlyDictionary<string, string>>>(StringComparer.Ordinal);
        foreach (var file in files)
        {
            var (baseName, culture) = Split(file);
            var key = Path.GetRelativePath(RepoRoot, Path.Combine(Path.GetDirectoryName(file)!, baseName));
            if (!sets.TryGetValue(key, out var byCulture))
            {
                sets[key] = byCulture = new Dictionary<string, IReadOnlyDictionary<string, string>>(StringComparer.OrdinalIgnoreCase);
            }

            byCulture[culture] = Read(file);
        }

        return sets.Select(s => new ResourceSet(s.Key, s.Value)).OrderBy(s => s.Name, StringComparer.Ordinal).ToList();
    }

    private static (string BaseName, string Culture) Split(string path)
    {
        // ClockStrings.resx -> (ClockStrings, en); ClockStrings.es.resx -> (ClockStrings, es); Strings.pt-BR.resx -> (Strings, pt-BR)
        var name = Path.GetFileNameWithoutExtension(path);
        var dot = name.LastIndexOf('.');
        if (dot > 0)
        {
            var suffix = name[(dot + 1)..];
            if (LooksLikeCulture(suffix))
            {
                return (name[..dot], suffix);
            }
        }

        return (name, NeutralCulture);
    }

    private static bool LooksLikeCulture(string s)
    {
        try
        {
            return s.Length is >= 2 and <= 10 && System.Globalization.CultureInfo.GetCultureInfo(s).Name.Length > 0;
        }
        catch (System.Globalization.CultureNotFoundException)
        {
            return false;
        }
    }

    private static IReadOnlyDictionary<string, string> Read(string path)
    {
        var doc = XDocument.Load(path);
        return doc.Root!.Elements("data")
            .ToDictionary(
                d => (string)d.Attribute("name")!,
                d => (string?)d.Element("value") ?? string.Empty,
                StringComparer.Ordinal);
    }
}
