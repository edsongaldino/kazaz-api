using System;

namespace Kazaz.Domain.Interfaces;

public interface ITenantProvider
{
    Guid? ObterImobiliariaId();
    bool EhAdminSistema();
}
