using Azure.Storage.Blobs.Specialized;

namespace AzBlobDownloader.Common.Extensions;

public static class BlobClientExtensions
{
    public static async Task DownloadBlobAsync(
        this BlobBaseClient blobClient, 
        string destination, 
        string? file, 
        int index,
        Action<string, string>? startCallback = null,
        Action<long>? progressCallback = null,
        Action? completeCallback = null,
        CancellationToken cancellationToken = default)
    {
        var blobDownloadInfo = await blobClient.DownloadAsync(cancellationToken);
        if (string.IsNullOrWhiteSpace(file))
            file = blobClient.Name;
        var filePath = Path.Combine(destination, file);
        if (index > 0 && !file.Equals(blobClient.Name)) // We have multiple files with the same name
            filePath = Path.Combine(destination, $"{Path.GetFileNameWithoutExtension(file)}_{index}{Path.GetExtension(file)}");
        
        startCallback?.Invoke(blobClient.Name, filePath);

        var progress = new Progress<long>();
        progress.ProgressChanged += (_, e) => progressCallback?.Invoke(e);
        
        await using var fileStream = File.OpenWrite(filePath);
        await blobDownloadInfo.Value.Content.CopyToAsync(fileStream, progress, cancellationToken: cancellationToken);
        fileStream.Close();
        
        completeCallback?.Invoke();
    }
}