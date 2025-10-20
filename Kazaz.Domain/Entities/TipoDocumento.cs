using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kazaz.Domain.Entities;

public class TipoDocumento
{
    public Guid Id { get; set; }
    public required string Nome { get; set; }         // "RG", "CPF", "Comprovante de Endereço"…
    public AlvoDocumento Alvo { get; set; }          // Pessoa ou Imovel
    public bool Obrigatorio { get; set; }
    public int Ordem { get; set; } = 0;
    public bool Ativo { get; set; } = true;
    public string? Descricao { get; set; }
}