using Dotnet.Homeworks.Infrastructure.Validation.Decorators;
using Dotnet.Homeworks.Infrastructure.Validation.RequestTypes;
using Dotnet.Homeworks.Mediator;
using Dotnet.Homeworks.Shared.Dto;
using FluentValidation;

namespace Dotnet.Homeworks.Infrastructure.Validation.Behaviors;

public class ValidatorBehavior<TRequest, TResponse> : CqrsDecoratorBase<TResponse>,
    IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>, IAdminRequest
    where TResponse : Result
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidatorBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        try
        {
            if (!_validators.Any())
                return await next();

            var errors = _validators
                .Select(async validator => await validator.ValidateAsync(request, cancellationToken))
                .SelectMany(res => res.Result.Errors)
                .ToList();

            return errors.Any() 
                ? FromFail(errors.FirstOrDefault(fail => fail is not null)!.ErrorMessage) 
                : await next();
        }
        catch (Exception e)
        {
            return FromFail(e.Message);
        }
    }
}