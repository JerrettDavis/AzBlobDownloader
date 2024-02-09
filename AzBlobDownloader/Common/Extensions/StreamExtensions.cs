namespace AzBlobDownloader.Common.Extensions;

// Found here: https://stackoverflow.com/questions/39742515/stream-copytoasync-with-progress-reporting-progress-is-reported-even-after-cop
public static class StreamExtensions
{
    public static async Task CopyToAsync(
        this Stream source, 
        Stream destination, 
        IProgress<long> progress, 
        CancellationToken cancellationToken = default, 
        int bufferSize = 0x1000)
    {
        var buffer = new byte[bufferSize];
        int bytesRead;
        long totalRead = 0;
        while ((bytesRead = await source.ReadAsync(buffer, 0, buffer.Length, cancellationToken)) > 0)
        {
            await destination.WriteAsync(buffer, 0, bytesRead, cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
            totalRead += bytesRead;
            progress.Report(totalRead);
        }
    }
}