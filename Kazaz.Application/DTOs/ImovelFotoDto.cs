using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kazaz.Application.DTOs;

public sealed record ImovelFotoDto(
	Guid Id, 
	string Url, 
	int Ordem, 
	bool Principal
);
