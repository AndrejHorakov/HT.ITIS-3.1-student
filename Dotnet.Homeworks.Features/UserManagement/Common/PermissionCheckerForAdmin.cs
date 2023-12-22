using System.Security.Claims;
using Dotnet.Homeworks.Infrastructure.Utils;
using Dotnet.Homeworks.Infrastructure.Validation.PermissionChecker;
using Dotnet.Homeworks.Infrastructure.Validation.PermissionChecker.Enums;
using Dotnet.Homeworks.Infrastructure.Validation.RequestTypes;
using Microsoft.AspNetCore.Http;

namespace Dotnet.Homeworks.Features.UserManagement.Common;

public class PermissionCheckerForAdmin : IPermissionChecker<IAdminRequest>
{
    private readonly IHttpContextAccessor _context;

    public PermissionCheckerForAdmin(IHttpContextAccessor context)
    {
        _context = context;
    }

    public Task<PermissionResult> ValidateAsync(IAdminRequest request)
    {
        var role = _context.HttpContext!.User.Claims
            .FirstOrDefault(cl => cl.Type is ClaimTypes.Role)?.Value;

        return role == Roles.Admin.ToString()
            ? Task.FromResult(new PermissionResult(true))
            : Task.FromResult(new PermissionResult(false, "Access is denied!"));
    }
}