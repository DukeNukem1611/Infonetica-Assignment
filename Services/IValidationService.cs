using Infonetica.WorkflowEngine.Models;

namespace Infonetica.WorkflowEngine.Services;

public interface IValidationService
{
    Task ValidateWorkflowDefinitionAsync(WorkflowDefinition definition);
    Task ValidateActionExecutionAsync(WorkflowDefinition definition, WorkflowInstance instance, string actionId);
}
