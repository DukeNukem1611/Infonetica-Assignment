namespace Infonetica.WorkflowEngine.DTOs;

public class WorkflowInstanceResponse
{
    public required string Id { get; set; }
    public required string DefinitionId { get; set; }
    public required string CurrentStateId { get; set; }
    public required string CurrentStateName { get; set; }
    public required List<HistoryEntryResponse> History { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class HistoryEntryResponse
{
    public required string ActionId { get; set; }
    public required string ActionName { get; set; }
    public required string FromStateId { get; set; }
    public required string FromStateName { get; set; }
    public required string ToStateId { get; set; }
    public required string ToStateName { get; set; }
    public DateTime ExecutedAt { get; set; }
}
