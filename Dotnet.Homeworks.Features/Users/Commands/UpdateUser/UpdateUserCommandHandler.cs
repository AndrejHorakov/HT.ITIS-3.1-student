using Dotnet.Homeworks.Domain.Abstractions.Repositories;
using Dotnet.Homeworks.Infrastructure.Cqrs.Commands;
using Dotnet.Homeworks.Infrastructure.UnitOfWork;
using Dotnet.Homeworks.Infrastructure.Validation.Decorators;
using Dotnet.Homeworks.Infrastructure.Validation.PermissionChecker;
using Dotnet.Homeworks.Shared.Dto;
using FluentValidation;

namespace Dotnet.Homeworks.Features.Users.Commands.UpdateUser;

public class UpdateUserCommandHandler : CqrsDecorator<UpdateUserCommand, Result>,
    ICommandHandler<UpdateUserCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateUserCommandHandler(
        IEnumerable<IValidator<UpdateUserCommand>> validators,
        IPermissionCheck check,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork) : base(validators, check)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public new async Task<Result> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var validation = await base.Handle(request, cancellationToken);

        if (!validation.IsSuccess)
            return validation;
        
        var user = await _userRepository.GetUserByGuidAsync(request.Guid, cancellationToken);
        if (user is null)
            return new Result(false, "User not found");
        try
        {
            await _userRepository.UpdateUserAsync(request.User, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new Result(true);
        }
        catch (Exception e)
        {
            return new Result(false, e.Message);
        }
    }
}