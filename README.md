# AzBlobDownloader

The AzBlobDownloader is a command-line utility for downloading files from a Azure Blob Storage container. 

## Usage

You must specify either: A Shared Access Signature URI as the --connection-string (-c) parameter, or you must specify *both* a shared access key and account name via the --shared-key (-k) and --account (-a) options, respectively.

`AzBlobDownloader --help` output:

```
PS C:\> AzBlobDownloader --help
Description:
  Azure Blob Downloader

Usage:
  AzBlobDownloader [options]

Options:
  -c, --connection-string <connection-string>  Azure Storage Connection String
  -k, --shared-key <shared-key>                Azure Storage Shared Key
  -a, --account <account>                      Azure Storage Account Name
  -n, --container <container>                  Azure Storage Container Name
  -d, --destination <destination>              The destination directory to save the files. If not provided, the current directory will be used.
  -f, --file <file>                            The name of the file to save as. If not provided, the original file name will be used.
  -p, --pattern <pattern>                      File Pattern in Regex Format. Example: ^file.*\.txt$
  -m, --limit <limit>                          Limit the number of files to download.
  -l, --latest                                 Get the latest files first.
  -s, --silent                                 Silent Mode. No output will be written to the console.
  --version                                    Show version information
  -?, -h, --help                               Show help and usage information
```

Example usage:
```
.\AzBlobDownloader.exe --connection-string "https://<ACCOUNT_NAME>.blob.core.windows.net/?sv=<>&ss=b&srt=sco&sp=<>&se=<>&st=<>&spr=https&sig=<>" --container CONTAINER_NAME --destination . --pattern * --limit 1 --latest --file MyFile.bak
```