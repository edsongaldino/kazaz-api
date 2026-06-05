using System;
using Kazaz.Domain.Entities;

namespace Kazaz.Domain.Interfaces;

public interface IMultiTenant
{
    Guid? ImobiliariaId { get; set; }
    Imobiliaria? Imobiliaria { get; set; }
}
