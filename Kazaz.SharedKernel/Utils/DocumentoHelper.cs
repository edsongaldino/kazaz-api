using System.Text.RegularExpressions;

namespace Kazaz.SharedKernel.Utils;
public static class DocumentoHelper
{
	/// <summary>
	/// Remove tudo que não for dígito. Se valor for nulo ou vazio, retorna string.Empty.
	/// </summary>
	public static string Limpar(string? valor)
	{
		if (string.IsNullOrWhiteSpace(valor))
			return string.Empty;

		return Regex.Replace(valor, "[^0-9]", string.Empty);
	}
}

