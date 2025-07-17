namespace Infonetica.WorkflowEngine.DTOs;

public class CreateWorkflowDefinitionRequest
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public required List<StateDto> States { get; set; } = new();
    public required List<ActionDto> Actions { get; set; } = new();
}

public class StateDto
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public bool IsInitial { get; set; }
    public bool IsFinal { get; set; }
    public bool Enabled { get; set; } = true;
    public string? Description { get; set; }
}

public class ActionDto
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public bool Enabled { get; set; } = true;
    public required List<string> FromStates { get; set; } = new();
    public required string ToState { get; set; }
    public string? Description { get; set; }
}
