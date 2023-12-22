using Minio.DataModel;

namespace Dotnet.Homeworks.Storage.API.Dto.InternalClasses;

public class GetFileResponse
{
    public GetFileResponse(ObjectStat objectStats, MemoryStream stream)
    {
        ObjectStats = objectStats;
        Stream = stream;
    }

    public readonly ObjectStat ObjectStats;
    public readonly MemoryStream Stream;
}