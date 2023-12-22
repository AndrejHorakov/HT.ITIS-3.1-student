using Dotnet.Homeworks.Infrastructure.Validation.PermissionChecker;
using Dotnet.Homeworks.Mediator;
using Dotnet.Homeworks.Shared.Dto;
using FluentValidation;

namespace Dotnet.Homeworks.Infrastructure.Validation.Decorators;

public abstract class ValidationDecorator<TRequest, TResponse> 
    : PermissionCheckDecorator<TRequest, TResponse>,
    IRequestHandler<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : Result
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationDecorator(IEnumerable<IValidator<TRequest>> validators,
        IPermissionCheck check) : base(check)
    {
        _validators = validators;
    }

    public new async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken)
    {
        if (!_validators.Any())
            return await base.Handle(request, cancellationToken);
        
        var result = _validators
            .Select(async validator => await validator.ValidateAsync(request, cancellationToken))
            .SelectMany(res => res.Result.Errors)
            .FirstOrDefault(x => x.ErrorMessage is not null);

        return result is not null
            ? FromFail(result.ErrorMessage ?? "")
            : await base.Handle(request, cancellationToken);
    }
}