using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kazaz.Application.DTOs;

public sealed record ImovelDocumentoDto(
	Guid Id, 
	string Nome,
	string Url
);