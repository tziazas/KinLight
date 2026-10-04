using KinLight.Modules.Shared;

namespace KinLight.Localization.Tests;

public class ResourceCompletenessTests
{
    private static readonly string[] AllFolders = ["Modules/Widgets", "Modules/Display", "Modules/Portal"];

    // A resource set belongs to the portal side when it lives in a Portal project (Modules/Portal/** or Modules/Widgets/*/Portal/**).
    private static bool IsPortalSide(ResourceSet set) => set.Name.Split('/').Contains("Portal");

    private static IReadOnlyList<ResourceSet> DisplaySets => ResourceSet.Discover(AllFolders).Where(s => !IsPortalSide(s)).ToList();
    private static IReadOnlyList<ResourceSet> PortalSets => ResourceSet.Discover(AllFolders).Where(IsPortalSide).ToList();

    [Fact]
    public void Every_translation_has_exactly_the_keys_of_its_English_source()
    {
        var problems = new List<string>();

        foreach (var set in ResourceSet.Discover(AllFolders))
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
        var empty = ResourceSet.Discover(AllFolders)
            .SelectMany(set => set.ValuesByCulture.SelectMany(c => c.Value.Where(v => string.IsNullOrWhiteSpace(v.Value)).Select(v => $"{set.Name}.{c.Key}: {v.Key}")))
            .ToList();

        Assert.True(empty.Count == 0, string.Join(Environment.NewLine, empty));
    }

    [Fact]
    public void Display_languages_offered_to_families_are_exactly_the_fully_translated_ones()
    {
        // A language can be chosen for a display only when every widget and display resource set has it (ARCHITECTURE.md §15).
        AssertOfferedEqualsComplete(KnownCultures.Display, DisplaySets, nameof(KnownCultures.Display));
    }

    [Fact]
    public void Portal_languages_offered_are_exactly_the_fully_translated_ones()
    {
        AssertOfferedEqualsComplete(KnownCultures.Portal, PortalSets, nameof(KnownCultures.Portal));
    }

    private static void AssertOfferedEqualsComplete(IReadOnlyList<string> offered, IReadOnlyList<ResourceSet> sets, string listName)
    {
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
            "Translate every .resx for that side (" + string.Join(", ", sets.Select(s => s.Name)) + ") or remove them from the list.");
        Assert.True(notOffered.Count == 0,
            $"Fully translated languages missing from {listName}: {string.Join(", ", notOffered)}. Add them to the list.");
    }
}
