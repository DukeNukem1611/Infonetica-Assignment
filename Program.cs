using Infonetica.WorkflowEngine.Services;
using Infonetica.WorkflowEngine.Storage;
using Infonetica.WorkflowEngine.DTOs;
using Infonetica.WorkflowEngine.Exceptions;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure JSON serialization
builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.SerializerOptions.WriteIndented = true;
});

// Register application services
builder.Services.AddSingleton<IWorkflowStorage, InMemoryWorkflowStorage>();
builder.Services.AddScoped<IValidationService, ValidationService>();
builder.Services.AddScoped<IWorkflowService, WorkflowService>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Global exception handler
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var exceptionFeature = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>();
        if (exceptionFeature != null)
        {
            var exception = exceptionFeature.Error;
            var response = context.Response;
            response.ContentType = "application/json";

            var errorResponse = exception switch
            {
                ValidationException => new { error = "Validation Error", message = exception.Message, statusCode = 400 },
                NotFoundException => new { error = "Not Found", message = exception.Message, statusCode = 404 },
                InvalidOperationException => new { error = "Invalid Operation", message = exception.Message, statusCode = 400 },
                _ => new { error = "Internal Server Error", message = "An unexpected error occurred", statusCode = 500 }
            };

            response.StatusCode = errorResponse.statusCode;
            await response.WriteAsync(JsonSerializer.Serialize(errorResponse));
        }
    });
});

// Workflow Definition Endpoints
app.MapPost("/api/workflow-definitions", async (CreateWorkflowDefinitionRequest request, IWorkflowService workflowService) =>
{
    var definition = await workflowService.CreateWorkflowDefinitionAsync(request);
    return Results.Created($"/api/workflow-definitions/{definition.Id}", definition);
})
.WithName("CreateWorkflowDefinition")
.WithOpenApi();

app.MapGet("/api/workflow-definitions", async (IWorkflowService workflowService) =>
{
    var definitions = await workflowService.GetAllWorkflowDefinitionsAsync();
    return Results.Ok(definitions);
})
.WithName("GetAllWorkflowDefinitions")
.WithOpenApi();

app.MapGet("/api/workflow-definitions/{definitionId}", async (string definitionId, IWorkflowService workflowService) =>
{
    var definition = await workflowService.GetWorkflowDefinitionAsync(definitionId);
    return definition != null ? Results.Ok(definition) : Results.NotFound();
})
.WithName("GetWorkflowDefinition")
.WithOpenApi();

app.MapDelete("/api/workflow-definitions/{definitionId}", async (string definitionId, IWorkflowService workflowService) =>
{
    var deleted = await workflowService.DeleteWorkflowDefinitionAsync(definitionId);
    return deleted ? Results.NoContent() : Results.NotFound();
})
.WithName("DeleteWorkflowDefinition")
.WithOpenApi();

// Workflow Instance Endpoints
app.MapPost("/api/workflow-definitions/{definitionId}/instances", async (string definitionId, IWorkflowService workflowService) =>
{
    var instance = await workflowService.StartWorkflowInstanceAsync(definitionId);
    return Results.Created($"/api/workflow-instances/{instance.Id}", instance);
})
.WithName("StartWorkflowInstance")
.WithOpenApi();

app.MapGet("/api/workflow-instances", async (IWorkflowService workflowService) =>
{
    var instances = await workflowService.GetAllWorkflowInstancesAsync();
    return Results.Ok(instances);
})
.WithName("GetAllWorkflowInstances")
.WithOpenApi();

app.MapGet("/api/workflow-instances/{instanceId}", async (string instanceId, IWorkflowService workflowService) =>
{
    var instance = await workflowService.GetWorkflowInstanceAsync(instanceId);
    return instance != null ? Results.Ok(instance) : Results.NotFound();
})
.WithName("GetWorkflowInstance")
.WithOpenApi();

app.MapPost("/api/workflow-instances/{instanceId}/actions", async (string instanceId, ExecuteActionRequest request, IWorkflowService workflowService) =>
{
    var instance = await workflowService.ExecuteActionAsync(instanceId, request.ActionId);
    return Results.Ok(instance);
})
.WithName("ExecuteAction")
.WithOpenApi();

app.MapGet("/api/workflow-definitions/{definitionId}/instances", async (string definitionId, IWorkflowService workflowService) =>
{
    var instances = await workflowService.GetWorkflowInstancesByDefinitionAsync(definitionId);
    return Results.Ok(instances);
})
.WithName("GetWorkflowInstancesByDefinition")
.WithOpenApi();

app.Run();
