public interface ICaracteristicaService
{
    Task<IReadOnlyList<CaracteristicaListDto>> ListarAsync(
        bool? ativo,
        string? grupo,
        CancellationToken ct);
}