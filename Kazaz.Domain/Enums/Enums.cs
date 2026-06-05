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
    Comprador = 11,

    // Proprietário unificado
    Proprietario = 4
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

public enum LeadStatus
{
    Novo = 1,
    EmAtendimento = 2,
    Convertido = 3,
    Descartado = 4
}

public enum TipoLancamento
{
    Receita = 1, // Contas a Receber
    Despesa = 2  // Contas a Pagar
}

public enum StatusLancamento
{
    Pendente = 1,
    Pago = 2
}

public enum CargoColaborador
{
    Corretor = 1,
    Gerente = 2,
    Recepcionista = 3,
    Administrativo = 4,
    Diretor = 5,
    Outro = 6
}

public enum EspecialidadePrestador
{
    Pintor = 1,
    Encanador = 2,
    Eletricista = 3,
    Pedreiro = 4,
    Jardineiro = 5,
    Limpeza = 6,
    Chaveiro = 7,
    Gesseiro = 8,
    Outro = 9
}

