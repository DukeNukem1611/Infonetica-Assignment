# Infonetica Workflow Engine

A lightweight, configurable workflow engine built with .NET 8 and ASP.NET Core Minimal APIs.

## Features

- **Configurable State Machines**: Define workflows with states and transitions
- **Runtime Validation**: Comprehensive validation of workflow definitions and actions
- **Instance Management**: Start and track workflow instances with full execution history
- **Minimal API**: Clean, RESTful API endpoints with OpenAPI documentation
- **In-Memory Storage**: Simple persistence for rapid development and testing

## Quick Start

1. **Clone and Build**
```
git clone <repository-url>
cd Infonetica.WorkflowEngine
dotnet build
```

2. **Run the Application**
```
dotnet run
```

3. **Access Swagger UI**
Navigate to `https://localhost:5001/swagger` to explore the API

## API Endpoints

### Workflow Definitions
- `POST /api/workflow-definitions` - Create a new workflow definition
- `GET /api/workflow-definitions` - Get all workflow definitions
- `GET /api/workflow-definitions/{id}` - Get a specific workflow definition
- `DELETE /api/workflow-definitions/{id}` - Delete a workflow definition

### Workflow Instances
- `POST /api/workflow-definitions/{id}/instances` - Start a new workflow instance
- `GET /api/workflow-instances` - Get all workflow instances
- `GET /api/workflow-instances/{id}` - Get a specific workflow instance
- `POST /api/workflow-instances/{id}/actions` - Execute an action on an instance

## Example Usage

### 1. Create a Workflow Definition
```
{
"name": "Order Processing",
"description": "Simple order processing workflow",
"states": [
{
"id": "pending",
"name": "Pending",
"isInitial": true,
"isFinal": false,
"enabled": true
},
{
"id": "approved",
"name": "Approved",
"isInitial": false,
"isFinal": false,
"enabled": true
},
{
"id": "shipped",
"name": "Shipped",
"isInitial": false,
"isFinal": true,
"enabled": true
}
],
"actions": [
{
"id": "approve",
"name": "Approve Order",
"enabled": true,
"fromStates": ["pending"],
"toState": "approved"
},
{
"id": "ship",
"name": "Ship Order",
"enabled": true,
"fromStates": ["approved"],
"toState": "shipped"
}
]
}
```

### 2. Start a Workflow Instance
```
POST /api/workflow-definitions/{definitionId}/instances
```
### 3. Execute an Action
```
{
"actionId": "approve"
}
```

## Architecture

- **Clean Architecture**: Separated concerns with clear boundaries
- **Dependency Injection**: Interface-based design for extensibility
- **Validation**: Comprehensive business rule validation
- **Error Handling**: Graceful error responses with detailed messages

## Extension Points

- **Storage**: Implement `IWorkflowStorage` for database persistence
- **Validation**: Extend `IValidationService` for custom validation rules
- **Notifications**: Add event handling for workflow state changes
- **Authentication**: Integrate with ASP.NET Core Identity
- **Logging**: Add structured logging with Serilog

## Testing

The solution includes comprehensive validation for:
- Workflow definition integrity
- State machine rules enforcement
- Action execution validation
- Error handling scenarios

## Known Limitations

- In-memory storage (data is lost on restart)
- No authentication/authorization
- Basic error handling
- No audit logging beyond execution history

## Future Enhancements

- Database persistence layer
- Web-based workflow designer
- Advanced scheduling capabilities
- Workflow versioning
- Performance monitoring
- Integration with external systems
