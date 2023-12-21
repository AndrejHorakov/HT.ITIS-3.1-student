using Dotnet.Homeworks.Infrastructure.Validation.PermissionChecker;
using Dotnet.Homeworks.Mediator;
using Dotnet.Homeworks.Shared.Dto;

namespace Dotnet.Homeworks.Infrastructure.Validation.Decorators;

public class PermissionCheckDecorator<TRequest, TResponse> : CqrsDecoratorBase<TResponse>,
    IRequestHandler<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : Result
{
    private readonly IPermissionCheck _check;

    public PermissionCheckDecorator(IPermissionCheck check)
    {
        _check = check;
    }

    public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken)
    {
        var result = (await _check.CheckPermissionAsync(request)).ToList();

        return result.Any(x => !x.IsSuccess)
            ? FromFail(result.FirstOrDefault(x => x.Error is not null)?.Error ?? "")
            : FromSuccess();
    }
}