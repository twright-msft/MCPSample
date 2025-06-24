using System.Text.Json;

namespace MCPCalculatorClient;

/// <summary>
/// Console client for calling the MCP Calculator API
/// Usage: MCPCalculatorClient.exe <first_number> <operation> <second_number> [api_url]
/// Example: MCPCalculatorClient.exe 10 + 5
/// Example: MCPCalculatorClient.exe 7 * 8 http://localhost:5202
/// </summary>
class Program
{
    private static readonly HttpClient httpClient = new();
    private const string DefaultApiUrl = "http://localhost:5202";    static async Task<int> Main(string[] args)
    {
        try
        {            // Parse command line arguments
            if (!TryParseArguments(args, out var operation, out var firstNumber, out var secondNumber, out var apiUrl, out var numberOnly))
            {
                ShowUsage();
                return 1;
            }

            if (!numberOnly)
            {
                Console.WriteLine("🧮 MCP Calculator Client");
                Console.WriteLine("========================");
                Console.WriteLine($"📊 Calculating: {firstNumber} {operation} {secondNumber}");
                Console.WriteLine($"🌐 API URL: {apiUrl}");
                Console.WriteLine();
            }// Call the MCP API
            var result = await CallCalculatorApi(apiUrl, operation, firstNumber, secondNumber, numberOnly);
            
            if (numberOnly)
            {
                // Extract just the numeric result from the response
                var numericResult = ExtractNumericResult(result);
                Console.WriteLine(numericResult);
            }
            else
            {
                Console.WriteLine("✅ Success!");
                Console.WriteLine($"📋 Result: {result}");
            }
            
            return 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error: {ex.Message}");
            return 1;
        }
    }    private static bool TryParseArguments(string[] args, out string operation, out double firstNumber, out double secondNumber, out string apiUrl, out bool numberOnly)
    {
        operation = string.Empty;
        firstNumber = 0;
        secondNumber = 0;
        apiUrl = DefaultApiUrl;
        numberOnly = false;

        // Check for -NumberOnly flag
        var argsList = new List<string>(args);
        if (argsList.Contains("-NumberOnly"))
        {
            numberOnly = true;
            argsList.Remove("-NumberOnly");
        }

        // Convert back to array for processing
        args = argsList.ToArray();

        if (args.Length < 3)
        {
            return false;
        }

        // Parse first number
        if (!double.TryParse(args[0], out firstNumber))
        {
            if (!numberOnly) Console.WriteLine($"❌ Invalid first number: {args[0]}");
            return false;
        }

        // Parse operation
        operation = args[1];
        if (operation != "+" && operation != "-" && operation != "*" && operation != "/")
        {
            if (!numberOnly) Console.WriteLine($"❌ Invalid operation: {operation}");
            if (!numberOnly) Console.WriteLine("Valid operations: +, -, *, /");
            return false;
        }

        // Parse second number
        if (!double.TryParse(args[2], out secondNumber))
        {
            if (!numberOnly) Console.WriteLine($"❌ Invalid second number: {args[2]}");
            return false;
        }

        // Parse optional API URL
        if (args.Length >= 4)
        {
            apiUrl = args[3];
            
            // Validate URL format
            if (!Uri.TryCreate(apiUrl, UriKind.Absolute, out var uri) || 
                (uri.Scheme != "http" && uri.Scheme != "https"))
            {
                if (!numberOnly) Console.WriteLine($"❌ Invalid API URL: {apiUrl}");
                return false;
            }
        }

        return true;
    }

    private static async Task<string> CallCalculatorApi(string apiUrl, string operation, double firstNumber, double secondNumber, bool numberOnly = false)
    {
        var endpoint = $"{apiUrl.TrimEnd('/')}/api/mcp/tools/call";
        
        var requestBody = new
        {
            method = "tools/call",
            name = "calculator",
            arguments = new
            {
                operation = operation,
                a = firstNumber,
                b = secondNumber
            }
        };        var json = JsonSerializer.Serialize(requestBody, new JsonSerializerOptions 
        { 
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase 
        });

        if (!numberOnly)
        {
            Console.WriteLine($"🔗 Sending request to: {endpoint}");
            Console.WriteLine($"📤 Request body: {json}");
            Console.WriteLine();
        }

        using var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
        
        try
        {
            var response = await httpClient.PostAsync(endpoint, content);
            var responseContent = await response.Content.ReadAsStringAsync();            if (!numberOnly)
            {
                Console.WriteLine($"📥 Response status: {response.StatusCode}");
                Console.WriteLine($"📥 Response body: {responseContent}");
                Console.WriteLine();
            }

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"API call failed with status {response.StatusCode}: {responseContent}");
            }

            using var document = JsonDocument.Parse(responseContent);
            var root = document.RootElement;

            if (root.TryGetProperty("result", out var result) &&
                result.TryGetProperty("content", out var content_array) &&
                content_array.GetArrayLength() > 0)
            {
                var firstContent = content_array[0];
                if (firstContent.TryGetProperty("text", out var text))
                {
                    return text.GetString() ?? "No result text";
                }
            }

            if (root.TryGetProperty("error", out var error) &&
                error.TryGetProperty("message", out var errorMessage))
            {
                throw new InvalidOperationException($"API error: {errorMessage.GetString()}");
            }

            throw new InvalidOperationException("Unexpected response format");
        }
        catch (HttpRequestException ex)
        {
            throw new HttpRequestException($"Failed to connect to API at {endpoint}. Make sure the MCP server is running. Details: {ex.Message}");
        }
        catch (TaskCanceledException)
        {
            throw new TimeoutException($"Request to {endpoint} timed out. Make sure the MCP server is running and accessible.");
        }
    }

    private static void ShowUsage()
    {
        Console.WriteLine();        Console.WriteLine("📖 Usage:");
        Console.WriteLine("  MCPCalculatorClient.exe <first_number> <operation> <second_number> [api_url] [-NumberOnly]");
        Console.WriteLine();
        Console.WriteLine("🔧 Parameters:");
        Console.WriteLine("  first_number  : First operand (decimal number)");
        Console.WriteLine("  operation     : Mathematical operation (+, -, *, /)");
        Console.WriteLine("  second_number : Second operand (decimal number)");
        Console.WriteLine("  api_url       : Optional. MCP API base URL (default: http://localhost:5202)");
        Console.WriteLine("  -NumberOnly   : Optional. Output only the numeric result, no other text");
        Console.WriteLine();
        Console.WriteLine("💡 Examples:");
        Console.WriteLine("  MCPCalculatorClient.exe 10 + 5");
        Console.WriteLine("  MCPCalculatorClient.exe 7.5 * 2.5 -NumberOnly");
        Console.WriteLine("  MCPCalculatorClient.exe 100 / 25");
        Console.WriteLine("  MCPCalculatorClient.exe 50 - 30 http://localhost:5202");
        Console.WriteLine("  MCPCalculatorClient.exe 15 + 25 https://my-mcp-server.com -NumberOnly");
        Console.WriteLine();
        Console.WriteLine("⚠️  Note: Make sure the MCP API server is running before using this client.");
    }

    private static string ExtractNumericResult(string resultText)
    {
        // The result text is in format: "Result: 10 + 5 = 15"
        // We want to extract just "15"
        var equalIndex = resultText.LastIndexOf('=');
        if (equalIndex >= 0 && equalIndex < resultText.Length - 1)
        {
            var afterEqual = resultText.Substring(equalIndex + 1).Trim();
            return afterEqual;
        }

        // Fallback: try to extract any number from the string
        var match = System.Text.RegularExpressions.Regex.Match(resultText, @"-?\d+(?:\.\d+)?$");
        if (match.Success)
        {
            return match.Value;
        }

        return resultText.Trim();
    }
}
