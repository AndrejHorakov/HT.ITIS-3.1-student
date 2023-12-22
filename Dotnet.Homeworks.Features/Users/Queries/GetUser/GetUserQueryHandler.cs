using Dotnet.Homeworks.Domain.Abstractions.Repositories;
using Dotnet.Homeworks.Infrastructure.Cqrs.Queries;
using Dotnet.Homeworks.Infrastructure.Validation.Decorators;
using Dotnet.Homeworks.Infrastructure.Validation.PermissionChecker;
using Dotnet.Homeworks.Shared.Dto;
using FluentValidation;

namespace Dotnet.Homeworks.Features.Users.Queries.GetUser;

public class GetUserQueryHandler : CqrsDecorator<GetUserQuery, Result<GetUserDto>>,
    IQueryHandler<GetUserQuery, GetUserDto>
{
    private readonly IUserRepository _userRepository;

    public GetUserQueryHandler(
        IEnumerable<IValidator<GetUserQuery>> validators,
        IPermissionCheck check,
        IUserRepository userRepository) : base(validators, check)
    {
        _userRepository = userRepository;
    }

    public new async Task<Result<GetUserDto>> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        var validation = await base.Handle(request, cancellationToken);
        if (!validation.IsSuccess)
            return validation;
        
        var user = await _userRepository.GetUserByGuidAsync(request.Guid, cancellationToken);

        return user is null
            ? new Result<GetUserDto>(null, false, "User not found")
            : new Result<GetUserDto>(new GetUserDto(user.Id, user.Name, user.Email), true);
    }
}