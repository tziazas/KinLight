using KinLight.Modules.Shared;

namespace KinLight.Localization.Tests;

public class ResourceCompletenessTests
{
    private static readonly string[] DisplayFolders = ["Modules/Widgets", "Modules/Display"];
    private static readonly string[] PortalFolders = ["Modules/Portal"];

    [Fact]
    public void Every_translation_has_exactly_the_keys_of_its_English_source()
    {
        var problems = new List<string>();

        foreach (var set in ResourceSet.Discover([.. DisplayFolders, .. PortalFolders]))
        {
            Assert.True(set.ValuesByCulture.ContainsKey(ResourceSet.NeutralCulture), $"{set.Name} has no neutral (English) .resx");
            var english = set.ValuesByCulture[ResourceSet.NeutralCulture].Keys.ToHashSet(StringComparer.Ordinal);

            foreach (var (culture, values) in set.ValuesByCulture)
            {
                if (culture == ResourceSet.NeutralCulture)
                {
                    continue;
                }

                var missing = english.Except(values.Keys).Order().ToList();
                var extra = values.Keys.Except(english).Order().ToList();
                if (missing.Count > 0)
                {
                    problems.Add($"{set.Name}.{culture}: missing {string.Join(", ", missing)}");
                }

                if (extra.Count > 0)
                {
                    problems.Add($"{set.Name}.{culture}: extra {string.Join(", ", extra)}");
                }
            }
        }

        Assert.True(problems.Count == 0, string.Join(Environment.NewLine, problems));
    }

    [Fact]
    public void No_resource_value_is_empty()
    {
        var empty = ResourceSet.Discover([.. DisplayFolders, .. PortalFolders])
            .SelectMany(set => set.ValuesByCulture.SelectMany(c => c.Value.Where(v => string.IsNullOrWhiteSpace(v.Value)).Select(v => $"{set.Name}.{c.Key}: {v.Key}")))
            .ToList();

        Assert.True(empty.Count == 0, string.Join(Environment.NewLine, empty));
    }

    [Fact]
    public void Display_languages_offered_to_families_are_exactly_the_fully_translated_ones()
    {
        // A language can be chosen for a display only when every widget and display resource set has it (ARCHITECTURE.md §15).
        AssertOfferedEqualsComplete(KnownCultures.Display, DisplayFolders, nameof(KnownCultures.Display));
    }

    [Fact]
    public void Portal_languages_offered_are_exactly_the_fully_translated_ones()
    {
        AssertOfferedEqualsComplete(KnownCultures.Portal, PortalFolders, nameof(KnownCultures.Portal));
    }

    private static void AssertOfferedEqualsComplete(IReadOnlyList<string> offered, string[] folders, string listName)
    {
        var sets = ResourceSet.Discover(folders);
        Assert.NotEmpty(sets);

        var complete = sets
            .Select(s => s.Cultures.ToHashSet(StringComparer.OrdinalIgnoreCase))
            .Aggregate((a, b) => { a.IntersectWith(b); return a; })
            .Order(StringComparer.OrdinalIgnoreCase)
            .ToList();

        var offeredSorted = offered.Order(StringComparer.OrdinalIgnoreCase).ToList();

        var notTranslated = offeredSorted.Except(complete, StringComparer.OrdinalIgnoreCase).ToList();
        var notOffered = complete.Except(offeredSorted, StringComparer.OrdinalIgnoreCase).ToList();

        Assert.True(notTranslated.Count == 0,
            $"{listName} offers languages that are not fully translated: {string.Join(", ", notTranslated)}. " +
            "Translate every .resx under " + string.Join(", ", folders) + " or remove them from the list.");
        Assert.True(notOffered.Count == 0,
            $"Fully translated languages missing from {listName}: {string.Join(", ", notOffered)}. Add them to the list.");
    }
}
