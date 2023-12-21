using Dotnet.Homeworks.Domain.Abstractions.Repositories;
using FluentValidation;

namespace Dotnet.Homeworks.Features.UserManagement.Commands.DeleteUserByAdmin;

public class DeleteUserByAdminValidator : AbstractValidator<DeleteUserByAdminCommand>
{
    private readonly IUserRepository _userRepository;
    public DeleteUserByAdminValidator(IUserRepository userRepository)
    {
        _userRepository = userRepository;
        RuleFor(x => x.Guid)
            .NotEmpty()
            .MustAsync(Exists)
            .WithMessage("User not found");
    }

    private async Task<bool> Exists(Guid id, CancellationToken cancellationToken = default)
    {
        return await _userRepository.GetUserByGuidAsync(id, cancellationToken) is not null;
    }
}