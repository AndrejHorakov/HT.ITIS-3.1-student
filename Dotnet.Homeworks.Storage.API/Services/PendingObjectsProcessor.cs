using Dotnet.Homeworks.Storage.API.Dto.Internal;

namespace Dotnet.Homeworks.Storage.API.Services;

public class PendingObjectsProcessor : BackgroundService
{
    private readonly IStorageFactory _storageFactory;
    private IStorage<Image> _storage;

    public PendingObjectsProcessor(IStorageFactory storageFactory)
    {
        _storageFactory = storageFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _storage = await _storageFactory
            .CreateImageStorageWithinBucketAsync(Constants.Buckets.Pending);

        while (!stoppingToken.IsCancellationRequested)
        {
            await MoveFilesFromPendingBucketToTarget(stoppingToken);
            await Task.Delay(Constants.PendingObjectProcessor.Period, stoppingToken);
        }
    }

    private async Task MoveFilesFromPendingBucketToTarget(CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
            return;
        var items = await _storage.EnumerateItemNamesAsync(cancellationToken);
        foreach (var item in items)
            await MoveItem(item, cancellationToken);
    }

    private async Task MoveItem(string itemName,
        CancellationToken cancellationToken)
    {
        if (_storage is null)
            return;

        var item = await _storage.GetItemAsync(itemName, cancellationToken);

        if (item is null)
            return;

        if (item.Metadata.TryGetValue(Constants.MetadataKeys.Destination, out var destBucket))
            await _storage.CopyItemToBucketAsync(
                itemName,
                destBucket,
                cancellationToken);

        await _storage.RemoveItemAsync(itemName, cancellationToken);
    }
}