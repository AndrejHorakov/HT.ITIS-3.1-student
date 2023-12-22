using Dotnet.Homeworks.Storage.API.Dto.Internal;
using Minio;

namespace Dotnet.Homeworks.Storage.API.Services;

public class StorageFactory : IStorageFactory
{
    private readonly MinioClient _minioClient;

    public StorageFactory(MinioClient minioClient)
    {
        _minioClient = minioClient;
    }

    public async Task<IStorage<Image>> CreateImageStorageWithinBucketAsync(string bucketName)
    {
        var bucketExistArgs = new BucketExistsArgs()
            .WithBucket(bucketName);
        if (!(await _minioClient.BucketExistsAsync(bucketExistArgs)))
        {
            var makeBucketArgs = new MakeBucketArgs()
                .WithBucket(bucketName);
            await _minioClient.MakeBucketAsync(makeBucketArgs);
        }

        return new ImageStorage(_minioClient, bucketName);
    }
}