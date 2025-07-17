using Infonetica.WorkflowEngine.Models;

namespace Infonetica.WorkflowEngine.Storage;

public interface IWorkflowStorage
{
    // Workflow Definitions
    Task<WorkflowDefinition> CreateDefinitionAsync(WorkflowDefinition definition);
    Task<WorkflowDefinition?> GetDefinitionAsync(string definitionId);
    Task<List<WorkflowDefinition>> GetAllDefinitionsAsync();
    Task<bool> DeleteDefinitionAsync(string definitionId);

    // Workflow Instances
    Task<WorkflowInstance> CreateInstanceAsync(WorkflowInstance instance);
    Task<WorkflowInstance?> GetInstanceAsync(string instanceId);
    Task<List<WorkflowInstance>> GetAllInstancesAsync();
    Task<List<WorkflowInstance>> GetInstancesByDefinitionAsync(string definitionId);
    Task<WorkflowInstance> UpdateInstanceAsync(WorkflowInstance instance);
    Task<bool> DeleteInstanceAsync(string instanceId);
}
