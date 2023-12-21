using Dotnet.Homeworks.Shared.Dto;

namespace Dotnet.Homeworks.Infrastructure.Validation.Decorators;

public abstract class CqrsDecoratorBase<TResponse> where TResponse : Result
{
    private protected static TResponse FromFail(string error)
    {
        return GetResponse(false, error);
    }

    private protected static TResponse FromSuccess()
    {
        return GetResponse(true, null);
    }

    private static TResponse GetResponse(bool success, string? error)
    {
        if (typeof(TResponse) == typeof(Result))
            return (new Result(success, error) as TResponse)!;

        var genericArg = typeof(TResponse).GetGenericArguments()[0];
        var targetResult = typeof(Result<>)
            .GetGenericTypeDefinition()
            .MakeGenericType(genericArg)
            .GetConstructor(new[] { genericArg,  typeof(bool), typeof(string) })!
            .Invoke(new object?[]{ null, success, error });

        return (targetResult as TResponse)!;
    }
}