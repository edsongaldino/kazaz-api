using System;
using System.Security.Claims;
using Kazaz.Domain.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Kazaz.API.Security;

public class TenantProvider : ITenantProvider
{
    private readonly IHttpContextAccessor _accessor;

    public TenantProvider(IHttpContextAccessor accessor)
    {
        _accessor = accessor;
    }

    public Guid? ObterImobiliariaId()
    {
        var user = _accessor.HttpContext?.User;
        var claim = user?.FindFirst("imobiliariaId")?.Value;
        if (Guid.TryParse(claim, out var guid))
            return guid;
        return null;
    }

    public bool EhAdminSistema()
    {
        var user = _accessor.HttpContext?.User;
        var role = user?.FindFirst(ClaimTypes.Role)?.Value;
        return role == "Administrador do Sistema";
    }
}
