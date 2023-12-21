using Dotnet.Homeworks.Infrastructure.Cqrs.Queries;
using Dotnet.Homeworks.Infrastructure.Validation.RequestTypes;
using Dotnet.Homeworks.Mediator;
using Dotnet.Homeworks.Shared.Dto;

namespace Dotnet.Homeworks.Features.Users.Queries.GetUser;

public class GetUserQuery : IQuery<GetUserDto>, IClientRequest
{
    public Guid Guid { get; init; }

    public GetUserQuery(Guid guid)
    {
        Guid = guid;
    }
};