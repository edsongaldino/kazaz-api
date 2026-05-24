public enum AlvoDocumento { Pessoa = 1, Imovel = 2 }

public enum EstadoCivil
{
    NaoInformado = 0,
    Solteiro = 1,
    Casado = 2,
    Divorciado = 3,
    Viuvo = 4,
    UniaoEstavel = 5,
    Separado = 6
}

public enum FinalidadeImovel
{
	Venda = 1,
	Aluguel = 2,
	Temporada = 3,
	UsoProprio = 4
}

public enum StatusImovel
{
	Ativo = 1,
	Inativo = 2,
	EmNegociacao = 3,
	Vendido = 4,
	Alugado = 5
}

public enum TipoContrato
{
    Locacao = 1,
    Venda = 2,
    Compra = 3 // opcional; você pode manter só Venda futuramente
}

public enum StatusContrato
{
    Rascunho = 1,
    Ativo = 2,
    Encerrado = 3,
    Cancelado = 4,
    EmAnalise = 5
}

public enum PapelContrato
{
    // Locação
    Locador = 1,     // vinculado ao imóvel (ImovelProprietario), não mais ao convite de link
    Locatario = 2,
    Fiador = 3,

    // Venda / Compra
    Vendedor = 10,
    Comprador = 11
}

/// <summary>
/// Forma de garantia escolhida na geração do link de locação.
/// Define quais perfis de convite serão criados.
/// </summary>
public enum FormaGarantiaLocacao
{
    Fiador = 1,
    SeguroFianca = 2
}
