using System;

namespace Kazaz.Domain.Entities;

public class ChecklistEtapaGlobal
{
    public Guid Id { get; set; }
    public string TipoChecklist { get; set; } = string.Empty; // "entrada" ou "saida"
    public string Label { get; set; } = string.Empty;
    public string TipoField { get; set; } = string.Empty; // "text", "textarea", "date", "boolean"
    public string Card { get; set; } = string.Empty;
}
