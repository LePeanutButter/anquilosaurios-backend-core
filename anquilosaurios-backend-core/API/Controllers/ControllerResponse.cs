namespace aquilosaurios_backend_core.API.Controllers;

public class ControllerResponse<T>
{
    public T? Data { get; set; }
    public string? Message { get; set; }

    public ControllerResponse() { }
    public ControllerResponse(T? data, string? message = null)
    {
        Data = data;
        Message = message ?? (data == null ? "OK" : null);
    }

    public ControllerResponse(string message)
    {
        Message = message ?? "OK";
        Data = default;
    }
}
