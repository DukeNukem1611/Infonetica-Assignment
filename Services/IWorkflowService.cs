using Infonetica.WorkflowEngine.Models;
using Infonetica.WorkflowEngine.DTOs;

namespace Infonetica.WorkflowEngine.Services;

public interface IWorkflowService
{
    Task<WorkflowDefinition> CreateWorkflowDefinitionAsync(CreateWorkflowDefinitionRequest request);
    Task<WorkflowDefinition?> GetWorkflowDefinitionAsync(string definitionId);
    Task<List<WorkflowDefinition>> GetAllWorkflowDefinitionsAsync();
    Task<bool> DeleteWorkflowDefinitionAsync(string definitionId);

    Task<WorkflowInstance> StartWorkflowInstanceAsync(string definitionId);
    Task<WorkflowInstance> ExecuteActionAsync(string instanceId, string actionId);
    Task<WorkflowInstanceResponse?> GetWorkflowInstanceAsync(string instanceId);
    Task<List<WorkflowInstanceResponse>> GetAllWorkflowInstancesAsync();
    Task<List<WorkflowInstanceResponse>> GetWorkflowInstancesByDefinitionAsync(string definitionId);
}
