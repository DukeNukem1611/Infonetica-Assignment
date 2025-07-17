using Infonetica.WorkflowEngine.Models;
using System.Collections.Concurrent;

namespace Infonetica.WorkflowEngine.Storage;

public class InMemoryWorkflowStorage : IWorkflowStorage
{
    private readonly ConcurrentDictionary<string, WorkflowDefinition> _definitions = new();
    private readonly ConcurrentDictionary<string, WorkflowInstance> _instances = new();

    public Task<WorkflowDefinition> CreateDefinitionAsync(WorkflowDefinition definition)
    {
        _definitions.TryAdd(definition.Id, definition);
        return Task.FromResult(definition);
    }

    public Task<WorkflowDefinition?> GetDefinitionAsync(string definitionId)
    {
        _definitions.TryGetValue(definitionId, out var definition);
        return Task.FromResult(definition);
    }

    public Task<List<WorkflowDefinition>> GetAllDefinitionsAsync()
    {
        return Task.FromResult(_definitions.Values.ToList());
    }

    public Task<bool> DeleteDefinitionAsync(string definitionId)
    {
        return Task.FromResult(_definitions.TryRemove(definitionId, out _));
    }

    public Task<WorkflowInstance> CreateInstanceAsync(WorkflowInstance instance)
    {
        _instances.TryAdd(instance.Id, instance);
        return Task.FromResult(instance);
    }

    public Task<WorkflowInstance?> GetInstanceAsync(string instanceId)
    {
        _instances.TryGetValue(instanceId, out var instance);
        return Task.FromResult(instance);
    }

    public Task<List<WorkflowInstance>> GetAllInstancesAsync()
    {
        return Task.FromResult(_instances.Values.ToList());
    }

    public Task<List<WorkflowInstance>> GetInstancesByDefinitionAsync(string definitionId)
    {
        var instances = _instances.Values
            .Where(i => i.DefinitionId == definitionId)
            .ToList();
        return Task.FromResult(instances);
    }

    public Task<WorkflowInstance> UpdateInstanceAsync(WorkflowInstance instance)
    {
        instance.UpdatedAt = DateTime.UtcNow;
        _instances.TryUpdate(instance.Id, instance, _instances[instance.Id]);
        return Task.FromResult(instance);
    }

    public Task<bool> DeleteInstanceAsync(string instanceId)
    {
        return Task.FromResult(_instances.TryRemove(instanceId, out _));
    }
}
