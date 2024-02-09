using System.CommandLine;
using AzBlobDownloader.Common.Extensions;
using Azure.Storage;
using Azure.Storage.Blobs;

var connectionOption = new Option<string?>(
    ["--connection-string", "-c"],
    "Azure Storage Connection String");
var sharedKeyOption = new Option<string?>(
    ["--shared-key", "-k"],
    "Azure Storage Shared Key");
var accountOption = new Option<string?>(
    ["--account", "-a"],
    "Azure Storage Account Name");
var containerOption = new Option<string>(
    ["--container", "-n"],
    "Azure Storage Container Name");
var destinationOption = new Option<string>(
    ["--destination", "-d"],
    "The destination directory to save the files. If not provided, the current directory will be used.");
var fileOption = new Option<string?>(
    ["--file", "-f"],
    "The name of the file to save as. If not provided, the original file name will be used.");
var patternOption = new Option<string?>(
    ["--pattern", "-p"],
    "File Pattern in Regex Format. Example: ^file.*\\.txt$");
var limitOption = new Option<int?>(
    ["--limit", "-m"],
    "Limit the number of files to download.");
var getLatestOption = new Option<bool>(
    ["--latest", "-l"],
    "Get the latest files first.");
var silentOption = new Option<bool>(
    ["--silent", "-s"],
    "Silent Mode. No output will be written to the console.");
var rootCommand = new RootCommand("Azure Blob Downloader")
{
    connectionOption,
    sharedKeyOption,
    accountOption,
    containerOption,
    destinationOption,
    fileOption,
    patternOption,
    limitOption,
    getLatestOption,
    silentOption
};

rootCommand.SetHandler(async context =>
{
    var connection = context.ParseResult.GetValueForOption(connectionOption);
    var key = context.ParseResult.GetValueForOption(sharedKeyOption);
    var account = context.ParseResult.GetValueForOption(accountOption);
    var container = context.ParseResult.GetValueForOption(containerOption);
    var dest = context.ParseResult.GetValueForOption(destinationOption) ?? ".";
    var file = context.ParseResult.GetValueForOption(fileOption);
    var pattern = context.ParseResult.GetValueForOption(patternOption);
    var limit = context.ParseResult.GetValueForOption(limitOption);
    var getLatest = context.ParseResult.GetValueForOption(getLatestOption);
    var silent = context.ParseResult.GetValueForOption(silentOption);
    var token = context.GetCancellationToken();

    if (silent) Console.SetOut(TextWriter.Null);

    if (string.IsNullOrWhiteSpace(connection) &&
        string.IsNullOrWhiteSpace(account) &&
        string.IsNullOrWhiteSpace(key))
    {
        Console.WriteLine("You must either provide a connection string or a shared key and account name combo.");
        // set the exit code to a non-zero value to indicate failure
        context.ExitCode = 1;
        return;
    }

    var blobServiceClient = !string.IsNullOrWhiteSpace(connection)
        ? new BlobServiceClient(new Uri(connection))
        : new BlobServiceClient(
            new Uri($"https://{account}.blob.core.windows.net"),
            new StorageSharedKeyCredential(account, key));

    var blobContainerClient = blobServiceClient.GetBlobContainerClient(container);
    var items = blobContainerClient
        .GetBlobsAsync()
        .Where(b => b.Name.IsMatch(pattern));

    if (getLatest)
        items = items.OrderByDescending(i => i.Properties.LastModified);

    if (limit.HasValue)
        items = items.Take(limit.Value);

    await items
        .ForEachAwaitWithCancellationAsync(async (b, i, c) =>
                await blobContainerClient
                    .GetBlobClient(b.Name)
                    .DownloadBlobAsync(dest, file, i,
                        (f, d) => Console.WriteLine("Downloading {0} to {1}", f, d),
                        bytes => Console.Write(
                            $"\rDownloaded {Math.Ceiling((double)bytes / (b.Properties.ContentLength ?? 1) * 100)}% of {b.Name}"),
                        () =>
                        {
                            // Clear the progress line
                            Console.Write(new string(' ', Console.BufferWidth));
                            Console.Write($"\r{b.Name} Downloaded to {Path.Combine(dest, file ?? b.Name)}");
                        }, c),
            token);
    Console.WriteLine("\nDownload Complete!");
});

await rootCommand.InvokeAsync(args);