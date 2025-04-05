namespace LearningSemanticKernel.Models;

public class BaseHttpRequest
{
    public string Path { get; set; }
    public HttpMethod Method { get; set; }
}
