using Dotnet.Homeworks.Infrastructure.Validation.Decorators;
using Dotnet.Homeworks.Infrastructure.Validation.PermissionChecker;
using Dotnet.Homeworks.Infrastructure.Validation.RequestTypes;
using Dotnet.Homeworks.Mediator;
using Dotnet.Homeworks.Shared.Dto;

namespace Dotnet.Homeworks.Infrastructure.Validation.Behaviors;

public class PermissionCheckBehavior<TRequest, TResponse> : CqrsDecoratorBase<TResponse>,
    IPipelineBehavior<TRequest, TResponse> 
    where  TRequest : IRequest<TResponse>, IAdminRequest
    where TResponse : Result
{
    private readonly IPermissionCheck _permissionCheck;

    public PermissionCheckBehavior(IPermissionCheck permissionCheck)
    {
        _permissionCheck = permissionCheck;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var result = (await _permissionCheck.CheckPermissionAsync(request))
            .ToList();

        if (result.Any(x => !x.IsSuccess))
            return FromFail(result.FirstOrDefault(x => !x.IsSuccess)!.Error!);

        return await next();
    }
}