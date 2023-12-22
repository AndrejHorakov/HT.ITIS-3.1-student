using System.Reactive.Linq;
using Dotnet.Homeworks.Shared.Dto;
using Dotnet.Homeworks.Storage.API.Dto.Internal;
using Dotnet.Homeworks.Storage.API.Dto.InternalClasses;
using Minio;

namespace Dotnet.Homeworks.Storage.API.Services;

public class ImageStorage : IStorage<Image>
{
    private readonly MinioClient _minioClient;
    private readonly string _bucketName;

    public ImageStorage(MinioClient minioClient, string bucketName)
    {
        _minioClient = minioClient;
        _bucketName = bucketName;
    }

    public async Task<Result> PutItemAsync(Image item, CancellationToken cancellationToken = default)
    {
        if (item.Content is null)
            return new Result(false, "Content is empty");
        
        var check = await ItemExists(item.FileName, cancellationToken);
        if (check.IsSuccess)
            return new Result(false, "Item already exists");
        
        item.Metadata[Constants.MetadataKeys.Destination] = _bucketName;
        
        var putArgs = new PutObjectArgs()
            .WithBucket(Constants.Buckets.Pending)
            .WithObject(item.FileName)
            .WithContentType(item.ContentType)
            .WithHeaders(item.Metadata)
            .WithObjectSize(item.Content.Length)
            .WithStreamData(item.Content);
        try
        {
            await _minioClient.PutObjectAsync(putArgs, cancellationToken);
        }
        catch (Exception e)
        {
            return new Result(
                false,
                $"Error was occured, when putting object to storage: {e.Message}");
        }

        return new Result(true);
    }

    public async Task<Image?> GetItemAsync(
        string itemName,
        CancellationToken cancellationToken = default)
    {
        var check = await ItemExists(itemName, cancellationToken);

        return !check.IsSuccess
            ? null
            : new Image(
                check.Value!.Stream,
                check.Value.ObjectStats.ObjectName,
                check.Value.ObjectStats.ContentType,
                check.Value.ObjectStats.MetaData);
    }

    public async Task<Result> RemoveItemAsync(
        string itemName,
        CancellationToken cancellationToken = default)
    {
        var removeArgs = new RemoveObjectArgs()
            .WithBucket(_bucketName)
            .WithObject(itemName);

        try
        {
            await _minioClient.RemoveObjectAsync(removeArgs, cancellationToken);
        }
        catch (Exception e)
        {
            return new Result(
                false,
                $"Error was occured, when removing item: {e.Message}");
        }

        return new Result(true);
    }

    public async Task<IEnumerable<string>> EnumerateItemNamesAsync(
        CancellationToken cancellationToken = default)
    {
        var listArgs = new ListObjectsArgs()
            .WithBucket(_bucketName);

        return await _minioClient.ListObjectsAsync(listArgs, cancellationToken)
            .Select(obj => obj.Key)
            .ToList();
    }

    public async Task<Result> CopyItemToBucketAsync(
        string itemName,
        string destinationBucketName,
        CancellationToken cancellationToken = default)
    {
        var copySrcArgs = new CopySourceObjectArgs()
            .WithBucket(_bucketName)
            .WithObject(itemName);

        var copyArgs = new CopyObjectArgs()
            .WithBucket(destinationBucketName)
            .WithObject(itemName)
            .WithCopyObjectSource(copySrcArgs);

        try
        {
            await _minioClient.CopyObjectAsync(copyArgs, cancellationToken);
        }
        catch (Exception e)
        {
            return new Result(
                false,
                $"Error was occured when copy item: {e.Message}");
        }

        return new Result(true);
    }

    private async Task<Result<GetFileResponse>> ItemExists(
        string fileName,
        CancellationToken cancellationToken)
    {
        var outStream = new MemoryStream();
        try
        {
            var getArgs = new GetObjectArgs()
                .WithBucket(_bucketName)
                .WithObject(fileName)
                .WithCallbackStream(stream => stream.CopyTo(outStream));
            
            var check = await _minioClient.GetObjectAsync(getArgs, cancellationToken);
            outStream.Position = 0;
            
            return check is not null
                ? new Result<GetFileResponse>(new GetFileResponse(check, outStream),true)
                : new Result<GetFileResponse>(
                    null,
                    false,
                    "Item already exists");
        }
        catch (Exception e)
        {
            return new Result<GetFileResponse>(
                null,
                false,
                $"Error was occured, when checking existence: {e.Message}");
        }
        
    }
    
}