using System.Security.Claims;
using Dotnet.Homeworks.Infrastructure.Utils;
using Dotnet.Homeworks.Infrastructure.Validation.PermissionChecker;
using Dotnet.Homeworks.Infrastructure.Validation.RequestTypes;
using Microsoft.AspNetCore.Http;

namespace Dotnet.Homeworks.Features.Users.Common;

public class PermissionCheckerForUser : IPermissionChecker<IClientRequest>
{
    private readonly IHttpContextAccessor _context;

    public PermissionCheckerForUser(IHttpContextAccessor context)
    {
        _context = context;
    }

    public Task<PermissionResult> ValidateAsync(IClientRequest request)
    {
        var id = _context.HttpContext!.User.Claims
            .FirstOrDefault(cl => cl.Type is ClaimTypes.NameIdentifier)?.Value;
        
        
        return id == request.Guid.ToString()
            ? Task.FromResult(new PermissionResult(true))
            : Task.FromResult(new PermissionResult(false, "Access is denied!"));
    }
}