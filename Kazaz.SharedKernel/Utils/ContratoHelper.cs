using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kazaz.SharedKernel.Utils;

public class ContratoHelper
{

    public static PapelContrato[] PapeisPorTipo(TipoContrato tipo, bool incluirFiador)
    {
        return tipo switch
        {
            TipoContrato.Locacao => incluirFiador
                ? new[] { PapelContrato.Locador, PapelContrato.Locatario, PapelContrato.Fiador }
                : new[] { PapelContrato.Locador, PapelContrato.Locatario },

            TipoContrato.Venda or TipoContrato.Compra
                => new[] { PapelContrato.Vendedor, PapelContrato.Comprador },

            _ => Array.Empty<PapelContrato>()
        };
    }


}
