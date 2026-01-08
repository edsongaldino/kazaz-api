using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kazaz.Application.DTOs;

public sealed record UploadArquivoResponse(
    string Nome,
    string Caminho,
    string? ContentType,
    long TamanhoBytes
);