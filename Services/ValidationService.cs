using Infonetica.WorkflowEngine.Models;
using Infonetica.WorkflowEngine.Exceptions;

namespace Infonetica.WorkflowEngine.Services;

public class ValidationService : IValidationService
{
    public Task ValidateWorkflowDefinitionAsync(WorkflowDefinition definition)
    {
        var errors = new List<string>();

        // Check for exactly one initial state
        var initialStates = definition.States.Where(s => s.IsInitial).ToList();
        if (initialStates.Count == 0)
            errors.Add("Workflow definition must contain exactly one initial state");
        else if (initialStates.Count > 1)
            errors.Add("Workflow definition cannot have more than one initial state");

        // Check for duplicate state IDs
        var stateIds = definition.States.Select(s => s.Id).ToList();
        var duplicateStateIds = stateIds.GroupBy(id => id).Where(g => g.Count() > 1).Select(g => g.Key);
        if (duplicateStateIds.Any())
            errors.Add($"Duplicate state IDs found: {string.Join(", ", duplicateStateIds)}");

        // Check for duplicate action IDs
        var actionIds = definition.Actions.Select(a => a.Id).ToList();
        var duplicateActionIds = actionIds.GroupBy(id => id).Where(g => g.Count() > 1).Select(g => g.Key);
        if (duplicateActionIds.Any())
            errors.Add($"Duplicate action IDs found: {string.Join(", ", duplicateActionIds)}");

        // Validate actions reference existing states
        foreach (var action in definition.Actions)
        {
            foreach (var fromState in action.FromStates)
            {
                if (!stateIds.Contains(fromState))
                    errors.Add($"Action '{action.Id}' references non-existent fromState '{fromState}'");
            }

            if (!stateIds.Contains(action.ToState))
                errors.Add($"Action '{action.Id}' references non-existent toState '{action.ToState}'");
        }

        if (errors.Any())
            throw new ValidationException($"Validation failed: {string.Join("; ", errors)}");

        return Task.CompletedTask;
    }

    public Task ValidateActionExecutionAsync(WorkflowDefinition definition, WorkflowInstance instance, string actionId)
    {
        var action = definition.Actions.FirstOrDefault(a => a.Id == actionId);
        if (action == null)
            throw new NotFoundException($"Action with ID '{actionId}' not found in workflow definition");

        if (!action.Enabled)
            throw new InvalidOperationException($"Action '{actionId}' is disabled");

        if (!action.FromStates.Contains(instance.CurrentStateId))
            throw new InvalidOperationException($"Action '{actionId}' cannot be executed from current state '{instance.CurrentStateId}'");

        var currentState = definition.States.FirstOrDefault(s => s.Id == instance.CurrentStateId);
        if (currentState?.IsFinal == true)
            throw new InvalidOperationException($"Cannot execute actions from final state '{instance.CurrentStateId}'");

        var toState = definition.States.FirstOrDefault(s => s.Id == action.ToState);
        if (toState == null)
            throw new NotFoundException($"Target state '{action.ToState}' not found in workflow definition");

        return Task.CompletedTask;
    }
}
