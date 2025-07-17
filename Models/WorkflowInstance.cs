namespace Infonetica.WorkflowEngine.Models;

public class WorkflowInstance
{
    public required string Id { get; set; }
    public required string DefinitionId { get; set; }
    public required string CurrentStateId { get; set; }
    public required List<HistoryEntry> History { get; set; } = new();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}

public class HistoryEntry
{
    public required string ActionId { get; set; }
    public required string FromStateId { get; set; }
    public required string ToStateId { get; set; }
    public DateTime ExecutedAt { get; set; } = DateTime.UtcNow;
}
