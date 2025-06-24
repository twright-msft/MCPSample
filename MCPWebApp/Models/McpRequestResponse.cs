using System.Text.Json.Serialization;

namespace MCPSample.Models;

/// <summary>
/// Request to list available tools
/// </summary>
public class ListToolsRequest : McpRequest
{
    public ListToolsRequest()
    {
        Method = "tools/list";
    }
}

/// <summary>
/// Response containing list of tools
/// </summary>
public class ListToolsResponse : McpResponse
{
    [JsonPropertyName("tools")]
    public List<McpTool> Tools { get; set; } = new();
}

/// <summary>
/// Request to call a specific tool
/// </summary>
public class CallToolRequest : McpRequest
{
    public CallToolRequest()
    {
        Method = "tools/call";
    }

    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("arguments")]
    public Dictionary<string, object>? Arguments { get; set; }
}

/// <summary>
/// Response from calling a tool
/// </summary>
public class CallToolResponse : McpResponse
{
    [JsonPropertyName("content")]
    public List<McpContent> Content { get; set; } = new();

    [JsonPropertyName("isError")]
    public bool IsError { get; set; }
}

/// <summary>
/// Request to list available resources
/// </summary>
public class ListResourcesRequest : McpRequest
{
    public ListResourcesRequest()
    {
        Method = "resources/list";
    }
}

/// <summary>
/// Response containing list of resources
/// </summary>
public class ListResourcesResponse : McpResponse
{
    [JsonPropertyName("resources")]
    public List<McpResource> Resources { get; set; } = new();
}

/// <summary>
/// Request to read a resource
/// </summary>
public class ReadResourceRequest : McpRequest
{
    public ReadResourceRequest()
    {
        Method = "resources/read";
    }

    [JsonPropertyName("uri")]
    public required string Uri { get; set; }
}

/// <summary>
/// Response containing resource content
/// </summary>
public class ReadResourceResponse : McpResponse
{
    [JsonPropertyName("contents")]
    public List<McpContent> Contents { get; set; } = new();
}

/// <summary>
/// Request to list available prompts
/// </summary>
public class ListPromptsRequest : McpRequest
{
    public ListPromptsRequest()
    {
        Method = "prompts/list";
    }
}

/// <summary>
/// Response containing list of prompts
/// </summary>
public class ListPromptsResponse : McpResponse
{
    [JsonPropertyName("prompts")]
    public List<McpPrompt> Prompts { get; set; } = new();
}

/// <summary>
/// Request to get a prompt
/// </summary>
public class GetPromptRequest : McpRequest
{
    public GetPromptRequest()
    {
        Method = "prompts/get";
    }

    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("arguments")]
    public Dictionary<string, object>? Arguments { get; set; }
}

/// <summary>
/// Response containing prompt content
/// </summary>
public class GetPromptResponse : McpResponse
{
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("messages")]
    public List<McpMessage> Messages { get; set; } = new();
}

/// <summary>
/// MCP content structure
/// </summary>
public class McpContent
{
    [JsonPropertyName("type")]
    public required string Type { get; set; }

    [JsonPropertyName("text")]
    public string? Text { get; set; }

    [JsonPropertyName("data")]
    public string? Data { get; set; }

    [JsonPropertyName("mimeType")]
    public string? MimeType { get; set; }
}

/// <summary>
/// MCP message structure
/// </summary>
public class McpMessage
{
    [JsonPropertyName("role")]
    public required string Role { get; set; }

    [JsonPropertyName("content")]
    public required McpContent Content { get; set; }
}
