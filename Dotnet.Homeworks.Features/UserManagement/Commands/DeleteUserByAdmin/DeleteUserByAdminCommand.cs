using Dotnet.Homeworks.Infrastructure.Cqrs.Commands;
using Dotnet.Homeworks.Infrastructure.Validation.RequestTypes;

namespace Dotnet.Homeworks.Features.UserManagement.Commands.DeleteUserByAdmin;

public class DeleteUserByAdminCommand : IAdminRequest, ICommand 
{
    public Guid Guid { get; }

    public DeleteUserByAdminCommand(Guid guid)
    {
        Guid = guid;
    }
}