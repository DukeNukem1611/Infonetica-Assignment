namespace Infonetica.WorkflowEngine.Exceptions;

public class WorkflowEngineException : Exception
{
    public WorkflowEngineException(string message) : base(message) { }
    public WorkflowEngineException(string message, Exception innerException) : base(message, innerException) { }
}

public class ValidationException : WorkflowEngineException
{
    public ValidationException(string message) : base(message) { }
}

public class NotFoundException : WorkflowEngineException
{
    public NotFoundException(string message) : base(message) { }
}

public class InvalidOperationException : WorkflowEngineException
{
    public InvalidOperationException(string message) : base(message) { }
}
