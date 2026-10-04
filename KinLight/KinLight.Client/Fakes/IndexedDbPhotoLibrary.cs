using System.Text.Json;
using KinLight.Modules.Shared;
using KinLight.Modules.Widgets.Photos.Client;
using Microsoft.JSInterop;

namespace KinLight.Client.Fakes;

/// <summary>
/// Development stand-in for the household photo library: image bytes in IndexedDB (via wwwroot/js/photo-store.js),
/// metadata in localStorage. Shared by the portal's Photos page and the display's photo widgets. Made-up data only.
/// </summary>
public sealed class IndexedDbPhotoLibrary : IPhotoLibraryApi
{
    private const string StorageKey = "kinlight.dev.photos";
    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web);

    private readonly IJSRuntime _js;
    private readonly SemaphoreSlim _gate = new(1, 1);
    private List<PhotoItem>? _photos;

    /// <summary>Creates the library.</summary>
    public IndexedDbPhotoLibrary(IJSRuntime js)
    {
        _js = js;
    }

    /// <summary>Raised after any change, so displays refresh.</summary>
    public event Func<Task>? Changed;

    /// <inheritdoc />
    public async Task<IReadOnlyList<PhotoItem>> ListAsync(CancellationToken cancellationToken)
    {
        var photos = await LoadAsync(cancellationToken);
        return photos.OrderByDescending(p => p.UploadedUtc).ToList();
    }

    /// <summary>Photos in <paramref name="album"/> (or all), oldest first, as a display shows them.</summary>
    public async Task<IReadOnlyList<PhotoItem>> ListForDisplayAsync(string? album, CancellationToken cancellationToken)
    {
        var photos = await LoadAsync(cancellationToken);
        return photos
            .Where(p => string.IsNullOrWhiteSpace(album) || p.Albums.Contains(album, StringComparer.OrdinalIgnoreCase))
            .OrderBy(p => p.UploadedUtc)
            .ToList();
    }

    /// <inheritdoc />
    public async Task<PhotoItem> UploadAsync(PhotoUpload upload, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(upload);

        var item = new PhotoItem(Guid.NewGuid(), upload.Caption, upload.Albums.ToList(), DateTimeOffset.UtcNow);
        await _js.InvokeVoidAsync("kinlightPhotos.put", cancellationToken, item.Id.ToString(), upload.Content, upload.ContentType);

        await _gate.WaitAsync(cancellationToken);
        try
        {
            var photos = await LoadUnlockedAsync(cancellationToken);
            photos.Add(item);
            await PersistAsync(photos, cancellationToken);
        }
        finally
        {
            _gate.Release();
        }

        await NotifyAsync();
        return item;
    }

    /// <inheritdoc />
    public async Task<PhotoItem> UpdateAsync(Guid id, LocalizedText caption, IReadOnlyList<string> albums, CancellationToken cancellationToken)
    {
        PhotoItem updated;
        await _gate.WaitAsync(cancellationToken);
        try
        {
            var photos = await LoadUnlockedAsync(cancellationToken);
            var index = photos.FindIndex(p => p.Id == id);
            if (index < 0)
            {
                throw new KeyNotFoundException($"Photo {id} does not exist.");
            }

            updated = photos[index] with { Caption = caption, Albums = albums.ToList() };
            photos[index] = updated;
            await PersistAsync(photos, cancellationToken);
        }
        finally
        {
            _gate.Release();
        }

        await NotifyAsync();
        return updated;
    }

    /// <inheritdoc />
    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        await _gate.WaitAsync(cancellationToken);
        try
        {
            var photos = await LoadUnlockedAsync(cancellationToken);
            photos.RemoveAll(p => p.Id == id);
            await PersistAsync(photos, cancellationToken);
        }
        finally
        {
            _gate.Release();
        }

        await _js.InvokeVoidAsync("kinlightPhotos.remove", cancellationToken, id.ToString());
        await NotifyAsync();
    }

    /// <inheritdoc />
    public async Task<string?> GetImageUrlAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            return await _js.InvokeAsync<string?>("kinlightPhotos.url", cancellationToken, id.ToString());
        }
        catch (JSException)
        {
            return null;
        }
    }

    private async Task NotifyAsync()
    {
        if (Changed is { } handlers)
        {
            foreach (var handler in handlers.GetInvocationList().Cast<Func<Task>>())
            {
                await handler();
            }
        }
    }

    private async Task<List<PhotoItem>> LoadAsync(CancellationToken cancellationToken)
    {
        await _gate.WaitAsync(cancellationToken);
        try
        {
            return await LoadUnlockedAsync(cancellationToken);
        }
        finally
        {
            _gate.Release();
        }
    }

    private async Task<List<PhotoItem>> LoadUnlockedAsync(CancellationToken cancellationToken)
    {
        if (_photos is not null)
        {
            return _photos;
        }

        try
        {
            var json = await _js.InvokeAsync<string?>("localStorage.getItem", cancellationToken, StorageKey);
            _photos = string.IsNullOrEmpty(json) ? [] : JsonSerializer.Deserialize<List<PhotoItem>>(json, Json) ?? [];
        }
        catch (JsonException)
        {
            _photos = [];
        }
        catch (JSException)
        {
            _photos = [];
        }

        return _photos;
    }

    private async Task PersistAsync(List<PhotoItem> photos, CancellationToken cancellationToken)
    {
        _photos = photos;
        try
        {
            await _js.InvokeVoidAsync("localStorage.setItem", cancellationToken, StorageKey, JsonSerializer.Serialize(photos, Json));
        }
        catch (JSException)
        {
        }
    }
}
