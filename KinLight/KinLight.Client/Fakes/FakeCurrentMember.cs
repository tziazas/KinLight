using KinLight.Modules.Portal.Client;
using KinLight.Modules.Shared;
using Microsoft.JSInterop;

namespace KinLight.Client.Fakes;

/// <summary>
/// Development stand-in for the signed-in member: three made-up household members, switchable from the app bar.
/// The selection is remembered in localStorage. Identity replaces this in production.
/// </summary>
public sealed class FakeCurrentMember : ICurrentMember
{
    private const string StorageKey = "kinlight.dev.member";

    /// <summary>The made-up members.</summary>
    public static IReadOnlyList<(Guid Id, string Name, MemberRole Role)> Members { get; } =
    [
        (Guid.Parse("00000000-0000-4000-8000-00000000ae01"), "Erato", MemberRole.Owner),
        (Guid.Parse("00000000-0000-4000-8000-00000000ae02"), "Nikos", MemberRole.Family),
        (Guid.Parse("00000000-0000-4000-8000-00000000ae03"), "Eleni", MemberRole.Caregiver),
    ];

    private readonly IJSRuntime _js;
    private int _index;

    /// <summary>Creates the fake. Call <see cref="LoadAsync"/> once at startup.</summary>
    public FakeCurrentMember(IJSRuntime js)
    {
        _js = js;
    }

    /// <inheritdoc />
    public Guid Id => Members[_index].Id;

    /// <inheritdoc />
    public string Name => Members[_index].Name;

    /// <inheritdoc />
    public MemberRole Role => Members[_index].Role;

    /// <inheritdoc />
    public event Action? Changed;

    /// <summary>Restores the last selected member.</summary>
    public async Task LoadAsync()
    {
        try
        {
            var stored = await _js.InvokeAsync<string?>("localStorage.getItem", StorageKey);
            if (Guid.TryParse(stored, out var id))
            {
                var index = Members.ToList().FindIndex(m => m.Id == id);
                if (index >= 0)
                {
                    _index = index;
                }
            }
        }
        catch (JSException)
        {
        }
    }

    /// <summary>Switches to another made-up member.</summary>
    public async Task SwitchAsync(Guid id)
    {
        var index = Members.ToList().FindIndex(m => m.Id == id);
        if (index < 0 || index == _index)
        {
            return;
        }

        _index = index;
        try
        {
            await _js.InvokeVoidAsync("localStorage.setItem", StorageKey, id.ToString());
        }
        catch (JSException)
        {
        }

        Changed?.Invoke();
    }
}
