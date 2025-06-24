using Microsoft.AspNetCore.Mvc;
using MCPSample.Models;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace MCPSample.Controllers;

/// <summary>
/// Main MCP API controller handling Model Context Protocol operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class McpController : ControllerBase
{
    private readonly ILogger<McpController> _logger;

    public McpController(ILogger<McpController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Get server capabilities
    /// </summary>
    /// <returns>Server capabilities</returns>
    [HttpGet("capabilities")]
    public ActionResult<McpCapabilities> GetCapabilities()
    {
        _logger.LogInformation("Getting server capabilities");
        
        var capabilities = new McpCapabilities
        {
            Tools = true,
            Resources = true,
            Prompts = true,
            Logging = false
        };

        return Ok(capabilities);
    }

    /// <summary>
    /// List available tools
    /// </summary>
    /// <returns>List of available tools</returns>
    [HttpPost("tools/list")]
    public ActionResult<ListToolsResponse> ListTools([FromBody] ListToolsRequest request)
    {
        _logger.LogInformation("Listing available tools");

        var tools = new List<McpTool>
        {
            new()
            {
                Name = "echo",
                Description = "Echo back the provided text",
                InputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        text = new { type = "string", description = "Text to echo back" }
                    },
                    required = new[] { "text" }
                }
            },            new()
            {
                Name = "calculator",
                Description = "Perform basic mathematical calculations",
                InputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        operation = new { type = "string", description = "Mathematical operation (+, -, *, /)" },
                        a = new { type = "number", description = "First operand" },
                        b = new { type = "number", description = "Second operand" }
                    },
                    required = new[] { "operation", "a", "b" }
                }
            },
            new()
            {
                Name = "word_counter",
                Description = "Count words, characters, and sentences in text",
                InputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        text = new { type = "string", description = "Text to analyze" },
                        include_spaces = new { type = "boolean", description = "Include spaces in character count (default: true)" }
                    },
                    required = new[] { "text" }
                }
            }
        };

        var response = new ListToolsResponse
        {
            Result = new { tools }
        };

        return Ok(response);
    }

    /// <summary>
    /// Call a specific tool
    /// </summary>
    /// <param name="request">Tool call request</param>
    /// <returns>Tool execution result</returns>
    [HttpPost("tools/call")]
    public ActionResult<CallToolResponse> CallTool([FromBody] CallToolRequest request)
    {
        _logger.LogInformation("Calling tool: {ToolName}", request.Name);

        try
        {            var result = request.Name switch
            {
                "echo" => HandleEchoTool(request.Arguments),
                "calculator" => HandleCalculatorTool(request.Arguments),
                "word_counter" => HandleWordCounterTool(request.Arguments),
                _ => throw new InvalidOperationException($"Unknown tool: {request.Name}")
            };

            var response = new CallToolResponse
            {
                Result = new { content = result }
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling tool {ToolName}", request.Name);
            
            var errorResponse = new CallToolResponse
            {
                Error = new McpError
                {
                    Code = -32602,
                    Message = ex.Message
                }
            };

            return BadRequest(errorResponse);
        }
    }

    /// <summary>
    /// List available resources
    /// </summary>
    /// <returns>List of available resources</returns>
    [HttpPost("resources/list")]
    public ActionResult<ListResourcesResponse> ListResources([FromBody] ListResourcesRequest request)
    {
        _logger.LogInformation("Listing available resources");

        var resources = new List<McpResource>
        {
            new()
            {
                Uri = "file:///sample.txt",
                Name = "Sample Text File",
                Description = "A sample text file for demonstration",
                MimeType = "text/plain"
            },
            new()
            {
                Uri = "file:///data.json",
                Name = "Sample JSON Data",
                Description = "Sample JSON data for testing",
                MimeType = "application/json"
            }
        };

        var response = new ListResourcesResponse
        {
            Result = new { resources }
        };

        return Ok(response);
    }    /// <summary>
    /// Read a specific resource
    /// </summary>
    /// <param name="request">Resource read request</param>
    /// <returns>Resource content</returns>
    [HttpPost("resources/read")]
    public ActionResult<ReadResourceResponse> ReadResource([FromBody] ReadResourceRequest request)
    {
        _logger.LogInformation("Reading resource: {Uri}", request.Uri);

        try
        {
            var content = request.Uri switch
            {
                "file:///sample.txt" => new List<McpContent>
                {
                    new() { 
                        Type = "text", 
                        Text = System.IO.File.ReadAllText(Path.Combine("Resources", "sample.txt")),
                        MimeType = "text/plain"
                    }
                },
                "file:///data.json" => new List<McpContent>
                {
                    new() { 
                        Type = "text", 
                        Text = System.IO.File.ReadAllText(Path.Combine("Resources", "data.json")),
                        MimeType = "application/json"
                    }
                },
                _ => throw new FileNotFoundException($"Resource not found: {request.Uri}")
            };

            var response = new ReadResourceResponse
            {
                Result = new { contents = content }
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reading resource {Uri}", request.Uri);
            
            var errorResponse = new ReadResourceResponse
            {
                Error = new McpError
                {
                    Code = -32603,
                    Message = ex.Message
                }
            };

            return NotFound(errorResponse);
        }
    }

    /// <summary>
    /// List available prompts
    /// </summary>
    /// <returns>List of available prompts</returns>
    [HttpPost("prompts/list")]
    public ActionResult<ListPromptsResponse> ListPrompts([FromBody] ListPromptsRequest request)
    {
        _logger.LogInformation("Listing available prompts");

        var prompts = new List<McpPrompt>
        {
            new()
            {
                Name = "greeting",
                Description = "Generate a personalized greeting",
                Arguments = new List<McpPromptArgument>
                {
                    new() { Name = "name", Description = "Name of the person to greet", Required = true }
                }
            },
            new()
            {
                Name = "summarize",
                Description = "Summarize the provided text",
                Arguments = new List<McpPromptArgument>
                {
                    new() { Name = "text", Description = "Text to summarize", Required = true },
                    new() { Name = "length", Description = "Desired summary length", Required = false }
                }
            }
        };

        var response = new ListPromptsResponse
        {
            Result = new { prompts }
        };

        return Ok(response);
    }

    /// <summary>
    /// Get a specific prompt
    /// </summary>
    /// <param name="request">Prompt get request</param>
    /// <returns>Prompt content</returns>
    [HttpPost("prompts/get")]
    public ActionResult<GetPromptResponse> GetPrompt([FromBody] GetPromptRequest request)
    {
        _logger.LogInformation("Getting prompt: {PromptName}", request.Name);

        try
        {
            var messages = request.Name switch
            {
                "greeting" => new List<McpMessage>
                {
                    new()
                    {
                        Role = "user",
                        Content = new McpContent
                        {
                            Type = "text",
                            Text = $"Generate a friendly greeting for {request.Arguments?.GetValueOrDefault("name", "there")}"
                        }
                    }
                },
                "summarize" => new List<McpMessage>
                {
                    new()
                    {
                        Role = "user",
                        Content = new McpContent
                        {
                            Type = "text",
                            Text = $"Please summarize the following text in {request.Arguments?.GetValueOrDefault("length", "a few sentences")}: {request.Arguments?.GetValueOrDefault("text", "[no text provided]")}"
                        }
                    }
                },
                _ => throw new InvalidOperationException($"Unknown prompt: {request.Name}")
            };

            var response = new GetPromptResponse
            {
                Result = new { messages }
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting prompt {PromptName}", request.Name);
            
            var errorResponse = new GetPromptResponse
            {
                Error = new McpError
                {
                    Code = -32602,
                    Message = ex.Message
                }
            };

            return BadRequest(errorResponse);
        }
    }

    /// <summary>
    /// Download sample.txt resource file
    /// </summary>
    /// <returns>Text file content</returns>
    [HttpGet("resources/download/sample.txt")]
    public IActionResult DownloadSampleText()
    {
        try
        {
            var filePath = Path.Combine("Resources", "sample.txt");
            var content = System.IO.File.ReadAllText(filePath);
            return File(System.Text.Encoding.UTF8.GetBytes(content), "text/plain", "sample.txt");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error downloading sample.txt");
            return NotFound("File not found");
        }
    }

    /// <summary>
    /// Download data.json resource file
    /// </summary>
    /// <returns>JSON file content</returns>
    [HttpGet("resources/download/data.json")]
    public IActionResult DownloadSampleJson()
    {
        try
        {
            var filePath = Path.Combine("Resources", "data.json");
            var content = System.IO.File.ReadAllText(filePath);
            return File(System.Text.Encoding.UTF8.GetBytes(content), "application/json", "data.json");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error downloading data.json");
            return NotFound("File not found");
        }
    }

    private List<McpContent> HandleEchoTool(Dictionary<string, object>? arguments)
    {
        if (arguments == null || !arguments.ContainsKey("text"))
        {
            throw new ArgumentException("Missing required argument: text");
        }

        var text = arguments["text"].ToString();
        return new List<McpContent>
        {
            new() { Type = "text", Text = $"Echo: {text}" }
        };
    }    private List<McpContent> HandleCalculatorTool(Dictionary<string, object>? arguments)
    {
        if (arguments == null)
        {
            throw new ArgumentException("Missing required arguments");
        }

        if (!arguments.ContainsKey("operation") || !arguments.ContainsKey("a") || !arguments.ContainsKey("b"))
        {
            throw new ArgumentException("Missing required arguments: operation, a, b");
        }

        var operation = arguments["operation"].ToString();
        
        // Handle JsonElement conversion for numbers
        double a, b;
        try
        {
            if (arguments["a"] is JsonElement jsonA)
            {
                a = jsonA.GetDouble();
            }
            else
            {
                a = Convert.ToDouble(arguments["a"]);
            }
            
            if (arguments["b"] is JsonElement jsonB)
            {
                b = jsonB.GetDouble();
            }
            else
            {
                b = Convert.ToDouble(arguments["b"]);
            }
        }
        catch (Exception ex)
        {
            throw new ArgumentException($"Invalid numeric arguments: {ex.Message}");
        }

        var result = operation switch
        {
            "+" => a + b,
            "-" => a - b,
            "*" => a * b,
            "/" when b != 0 => a / b,
            "/" => throw new DivideByZeroException("Cannot divide by zero"),
            _ => throw new ArgumentException($"Unsupported operation: {operation}")
        };        return new List<McpContent>
        {
            new() { Type = "text", Text = $"Result: {a} {operation} {b} = {result}" }
        };
    }

    private List<McpContent> HandleWordCounterTool(Dictionary<string, object>? arguments)
    {
        if (arguments == null || !arguments.ContainsKey("text"))
        {
            throw new ArgumentException("Missing required argument: text");
        }

        var text = arguments["text"].ToString() ?? string.Empty;
        
        // Check for include_spaces parameter (default to true)
        bool includeSpaces = true;
        if (arguments.ContainsKey("include_spaces"))
        {
            if (arguments["include_spaces"] is JsonElement jsonSpaces)
            {
                includeSpaces = jsonSpaces.GetBoolean();
            }
            else
            {
                includeSpaces = Convert.ToBoolean(arguments["include_spaces"]);
            }
        }

        // Count statistics
        var words = text.Split(new char[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
        var wordCount = words.Length;
        
        var characterCount = includeSpaces ? text.Length : text.Replace(" ", "").Replace("\t", "").Replace("\n", "").Replace("\r", "").Length;
        
        var sentences = text.Split(new char[] { '.', '!', '?' }, StringSplitOptions.RemoveEmptyEntries)
                           .Where(s => !string.IsNullOrWhiteSpace(s))
                           .Count();

        var paragraphs = text.Split(new string[] { "\n\n", "\r\n\r\n" }, StringSplitOptions.RemoveEmptyEntries)
                            .Where(p => !string.IsNullOrWhiteSpace(p))
                            .Count();

        var result = $"Text Analysis Results:\n" +
                    $"📝 Words: {wordCount}\n" +
                    $"🔤 Characters: {characterCount}" + (includeSpaces ? " (with spaces)" : " (without spaces)") + "\n" +
                    $"📄 Sentences: {sentences}\n" +
                    $"📋 Paragraphs: {paragraphs}";

        if (wordCount > 0)
        {
            var avgWordsPerSentence = sentences > 0 ? Math.Round((double)wordCount / sentences, 1) : 0;
            var avgCharsPerWord = Math.Round((double)characterCount / wordCount, 1);
            result += $"\n\n📊 Averages:\n" +
                     $"• Words per sentence: {avgWordsPerSentence}\n" +
                     $"• Characters per word: {avgCharsPerWord}";
        }

        return new List<McpContent>
        {
            new() { Type = "text", Text = result }
        };
    }
}
