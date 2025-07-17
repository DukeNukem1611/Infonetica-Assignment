using Infonetica.WorkflowEngine.Models;
using Infonetica.WorkflowEngine.DTOs;
using Infonetica.WorkflowEngine.Storage;
using Infonetica.WorkflowEngine.Exceptions;

namespace Infonetica.WorkflowEngine.Services;

public class WorkflowService : IWorkflowService
{
    private readonly IWorkflowStorage _storage;
    private readonly IValidationService _validationService;

    public WorkflowService(IWorkflowStorage storage, IValidationService validationService)
    {
        _storage = storage;
        _validationService = validationService;
    }

    public async Task<WorkflowDefinition> CreateWorkflowDefinitionAsync(CreateWorkflowDefinitionRequest request)
    {
        var definition = new WorkflowDefinition
        {
            Id = Guid.NewGuid().ToString(),
            Name = request.Name,
            Description = request.Description,
            States = request.States.Select(s => new State
            {
                Id = s.Id,
                Name = s.Name,
                IsInitial = s.IsInitial,
                IsFinal = s.IsFinal,
                Enabled = s.Enabled,
                Description = s.Description
            }).ToList(),
            Actions = request.Actions.Select(a => new Models.Action
            {
                Id = a.Id,
                Name = a.Name,
                Enabled = a.Enabled,
                FromStates = a.FromStates,
                ToState = a.ToState,
                Description = a.Description
            }).ToList()
        };

        await _validationService.ValidateWorkflowDefinitionAsync(definition);
        return await _storage.CreateDefinitionAsync(definition);
    }

    public async Task<WorkflowDefinition?> GetWorkflowDefinitionAsync(string definitionId)
    {
        return await _storage.GetDefinitionAsync(definitionId);
    }

    public async Task<List<WorkflowDefinition>> GetAllWorkflowDefinitionsAsync()
    {
        return await _storage.GetAllDefinitionsAsync();
    }

    public async Task<bool> DeleteWorkflowDefinitionAsync(string definitionId)
    {
        return await _storage.DeleteDefinitionAsync(definitionId);
    }

    public async Task<WorkflowInstance> StartWorkflowInstanceAsync(string definitionId)
    {
        var definition = await _storage.GetDefinitionAsync(definitionId);
        if (definition == null)
            throw new NotFoundException($"Workflow definition with ID '{definitionId}' not found");

        var initialState = definition.States.FirstOrDefault(s => s.IsInitial);
        if (initialState == null)
            throw new InvalidOperationException($"No initial state found in workflow definition '{definitionId}'");

        var instance = new WorkflowInstance
        {
            Id = Guid.NewGuid().ToString(),
            DefinitionId = definitionId,
            CurrentStateId = initialState.Id,
            History = new List<HistoryEntry>()
        };

        return await _storage.CreateInstanceAsync(instance);
    }

    public async Task<WorkflowInstance> ExecuteActionAsync(string instanceId, string actionId)
    {
        var instance = await _storage.GetInstanceAsync(instanceId);
        if (instance == null)
            throw new NotFoundException($"Workflow instance with ID '{instanceId}' not found");

        var definition = await _storage.GetDefinitionAsync(instance.DefinitionId);
        if (definition == null)
            throw new NotFoundException($"Workflow definition with ID '{instance.DefinitionId}' not found");

        await _validationService.ValidateActionExecutionAsync(definition, instance, actionId);

        var action = definition.Actions.First(a => a.Id == actionId);
        var fromStateId = instance.CurrentStateId;
        var toStateId = action.ToState;

        // Update instance state
        instance.CurrentStateId = toStateId;
        instance.History.Add(new HistoryEntry
        {
            ActionId = actionId,
            FromStateId = fromStateId,
            ToStateId = toStateId
        });

        return await _storage.UpdateInstanceAsync(instance);
    }

    public async Task<WorkflowInstanceResponse?> GetWorkflowInstanceAsync(string instanceId)
    {
        var instance = await _storage.GetInstanceAsync(instanceId);
        if (instance == null) return null;

        var definition = await _storage.GetDefinitionAsync(instance.DefinitionId);
        if (definition == null) return null;

        return MapToResponse(instance, definition);
    }

    public async Task<List<WorkflowInstanceResponse>> GetAllWorkflowInstancesAsync()
    {
        var instances = await _storage.GetAllInstancesAsync();
        var responses = new List<WorkflowInstanceResponse>();

        foreach (var instance in instances)
        {
            var definition = await _storage.GetDefinitionAsync(instance.DefinitionId);
            if (definition != null)
            {
                responses.Add(MapToResponse(instance, definition));
            }
        }

        return responses;
    }

    public async Task<List<WorkflowInstanceResponse>> GetWorkflowInstancesByDefinitionAsync(string definitionId)
    {
        var instances = await _storage.GetInstancesByDefinitionAsync(definitionId);
        var definition = await _storage.GetDefinitionAsync(definitionId);
        
        if (definition == null)
            throw new NotFoundException($"Workflow definition with ID '{definitionId}' not found");

        return instances.Select(i => MapToResponse(i, definition)).ToList();
    }

    private WorkflowInstanceResponse MapToResponse(WorkflowInstance instance, WorkflowDefinition definition)
    {
        var currentState = definition.States.First(s => s.Id == instance.CurrentStateId);
        
        return new WorkflowInstanceResponse
        {
            Id = instance.Id,
            DefinitionId = instance.DefinitionId,
            CurrentStateId = instance.CurrentStateId,
            CurrentStateName = currentState.Name,
            History = instance.History.Select(h => new HistoryEntryResponse
            {
                ActionId = h.ActionId,
                ActionName = definition.Actions.First(a => a.Id == h.ActionId).Name,
                FromStateId = h.FromStateId,
                FromStateName = definition.States.First(s => s.Id == h.FromStateId).Name,
                ToStateId = h.ToStateId,
                ToStateName = definition.States.First(s => s.Id == h.ToStateId).Name,
                ExecutedAt = h.ExecutedAt
            }).ToList(),
            CreatedAt = instance.CreatedAt,
            UpdatedAt = instance.UpdatedAt
        };
    }
}
