using System.Text.Json.Serialization;

namespace MCPSample.Models;

/// <summary>
/// Base class for all MCP requests
/// </summary>
public abstract class McpRequest
{
    [JsonPropertyName("method")]
    public required string Method { get; set; }

    [JsonPropertyName("params")]
    public object? Params { get; set; }
}

/// <summary>
/// Base class for all MCP responses
/// </summary>
public abstract class McpResponse
{
    [JsonPropertyName("result")]
    public object? Result { get; set; }

    [JsonPropertyName("error")]
    public McpError? Error { get; set; }
}

/// <summary>
/// MCP error structure
/// </summary>
public class McpError
{
    [JsonPropertyName("code")]
    public int Code { get; set; }

    [JsonPropertyName("message")]
    public required string Message { get; set; }

    [JsonPropertyName("data")]
    public object? Data { get; set; }
}

/// <summary>
/// MCP tool definition
/// </summary>
public class McpTool
{
    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("inputSchema")]
    public object? InputSchema { get; set; }
}

/// <summary>
/// MCP resource definition
/// </summary>
public class McpResource
{
    [JsonPropertyName("uri")]
    public required string Uri { get; set; }

    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("mimeType")]
    public string? MimeType { get; set; }
}

/// <summary>
/// MCP prompt definition
/// </summary>
public class McpPrompt
{
    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("arguments")]
    public List<McpPromptArgument>? Arguments { get; set; }
}

/// <summary>
/// MCP prompt argument
/// </summary>
public class McpPromptArgument
{
    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("required")]
    public bool Required { get; set; }
}

/// <summary>
/// MCP server capabilities
/// </summary>
public class McpCapabilities
{
    [JsonPropertyName("tools")]
    public bool Tools { get; set; }

    [JsonPropertyName("resources")]
    public bool Resources { get; set; }

    [JsonPropertyName("prompts")]
    public bool Prompts { get; set; }

    [JsonPropertyName("logging")]
    public bool Logging { get; set; }
}
