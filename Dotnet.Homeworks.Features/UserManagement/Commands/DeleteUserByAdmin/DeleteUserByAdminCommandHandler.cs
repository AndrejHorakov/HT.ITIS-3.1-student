using Dotnet.Homeworks.Domain.Abstractions.Repositories;
using Dotnet.Homeworks.Infrastructure.Cqrs.Commands;
using Dotnet.Homeworks.Infrastructure.UnitOfWork;
using Dotnet.Homeworks.Shared.Dto;

namespace Dotnet.Homeworks.Features.UserManagement.Commands.DeleteUserByAdmin;

public class DeleteUserByAdminCommandHandler : ICommandHandler<DeleteUserByAdminCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteUserByAdminCommandHandler(IUserRepository productRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = productRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteUserByAdminCommand request, CancellationToken cancellationToken)
    {
        var check = await _userRepository.GetUserByGuidAsync(request.Guid, cancellationToken);
        if (check is null)
            return new Result(false, "User not found");
        
        await _userRepository.DeleteUserByGuidAsync(request.Guid, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new Result(true);
    }
}