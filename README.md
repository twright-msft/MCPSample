# MCP Calculator System

A comprehensive **Model Context Protocol (MCP)** implementation featuring a complete calculator system with web interface, API server, command-line client, and automated testing. This project demonstrates best practices for building MCP-compliant applications with ASP.NET Core.

## 🏗️ System Architecture

```
🌐 MCPWebApp (ASP.NET Core MVC)
     ↕️ HTTP/JSON API Communication
💻 MCPCalculatorClient (Console Application)
     ↕️ Process Execution & Output Parsing
🧪 MCPCalculatorTestHarness (Automated Testing)
```

## 📦 Project Components

### 1. **MCPWebApp** - Main Web Server
- **Interactive Web UI** for testing MCP tools
- **RESTful API** implementing MCP protocol
- **Swagger Documentation** at `/api-docs`
- **Three MCP Tools**: Calculator, Echo, Word Counter

### 2. **MCPCalculatorClient** - Console Client
- **Command-line interface** for calculator API
- **Flexible parameter support** with optional API URL
- **Machine-readable output** with `-NumberOnly` flag
- **Comprehensive error handling**

### 3. **MCPCalculatorTestHarness** - Automated Testing
- **21 comprehensive test cases** covering all operations
- **Automated pass/fail validation**
- **Detailed test reporting** with success metrics
- **Error scenario testing** (division by zero)

## 🚀 Getting Started

### Prerequisites
- **.NET 9.0 SDK** or later
- **Visual Studio Code** (recommended) or Visual Studio
- **PowerShell** (for Windows users)

### � Installation & Setup

1. **Clone or download the project:**
   ```bash
   git clone <repository-url>
   cd MCPSample
   ```

2. **Build all projects:**
   ```bash
   # Build the main web application
   cd MCPWebApp
   dotnet build

   # Build the calculator client
   cd ../MCPCalculatorClient
   dotnet build

   # Build the test harness
   cd ../MCPCalculatorTestHarness
   dotnet build
   ```

## 🏃‍♂️ Running the System

### 1. Start the Web Server

```bash
cd MCPWebApp
dotnet run
```

**Server will start at:** `http://localhost:5272`

**Available URLs:**
- 🏠 **Home Page:** `http://localhost:5272`
- 📚 **API Docs:** `http://localhost:5272/api-docs`
- 🧮 **API Endpoint:** `http://localhost:5272/api/mcp/tools/call`

### 2. Using the Interactive Web Interface

1. Open `http://localhost:5272` in your browser
2. Test the three available tools:
   - **🧮 Calculator**: Perform mathematical operations
   - **📢 Echo**: Echo back text with optional repetition
   - **📝 Word Counter**: Count words, characters, and lines

### 3. Using the Command-Line Calculator Client

#### Basic Usage
```bash
cd MCPCalculatorClient
dotnet run -- <first_number> <operation> <second_number> [api_url] [-NumberOnly]
```

#### Examples
```bash
# Basic calculation (uses default URL: http://localhost:5202)
dotnet run -- 10 + 5

# With custom API URL
dotnet run -- 7.5 * 2.5 http://localhost:5272

# Machine-readable output (numbers only)
dotnet run -- 15 / 3 http://localhost:5272 -NumberOnly

# Complex operations
dotnet run -- -10 - -5
dotnet run -- 1000000 + 2000000
```

#### Parameters
- **`first_number`**: First operand (decimal number)
- **`operation`**: Mathematical operation (`+`, `-`, `*`, `/`)
- **`second_number`**: Second operand (decimal number)
- **`api_url`**: Optional. MCP API base URL (default: `http://localhost:5202`)
- **`-NumberOnly`**: Optional. Output only the numeric result

#### Sample Output
```bash
# Normal output
🧮 MCP Calculator Client
========================
📊 Calculating: 10 + 5
🌐 API URL: http://localhost:5272

🔗 Sending request to: http://localhost:5272/api/mcp/tools/call
📤 Request body: {"method":"tools/call","name":"calculator"...}

📥 Response status: OK
📥 Response body: {"result":{"content":[{"type":"text","text":"Result: 10 + 5 = 15"}]}}

✅ Success!
📋 Result: Result: 10 + 5 = 15

# -NumberOnly output
15
```

### 4. Running the Automated Test Harness

The test harness runs comprehensive validation of the calculator system:

```bash
cd MCPCalculatorTestHarness
dotnet run
```

#### Test Coverage (21 Test Cases)

**✅ Addition Tests (5 cases):**
- Basic addition: `10 + 5`
- Decimal addition: `7.5 + 2.3`
- Negative addition: `-5 + 3`
- Zero addition: `0 + 42`
- Large numbers: `1000000 + 2000000`

**✅ Subtraction Tests (5 cases):**
- Basic subtraction: `15 - 7`
- Decimal subtraction: `10.5 - 3.2`
- Negative subtraction: `-10 - -5`
- Zero subtraction: `25 - 0`
- Result becomes negative: `5 - 10`

**✅ Multiplication Tests (5 cases):**
- Basic multiplication: `6 * 7`
- Decimal multiplication: `2.5 * 4.0`
- Negative multiplication: `-3 * 4`
- Zero multiplication: `999 * 0`
- Fractional multiplication: `0.5 * 8`

**✅ Division Tests (6 cases):**
- Basic division: `20 / 4`
- Decimal division: `15.0 / 3.0`
- Negative division: `-12 / 3`
- Fractional result: `7 / 2`
- Division by one: `42 / 1`
- **Division by zero: Error handling**

#### Sample Test Output
```
🧪 MCP Calculator Client Test Harness
=====================================
📍 Using calculator client: ...\MCPCalculatorClient.exe
🧮 Running 21 test cases...

🔍 Test: Basic addition
   Expression: 10 + 5
   ✅ PASS - Result: 15

🔍 Test: Decimal addition
   Expression: 7.5 + 2.3
   ✅ PASS - Result: 9.8

...

📊 Test Summary
===============
✅ Passed: 21
❌ Failed: 0
📋 Total: 21
📈 Success Rate: 100.0%
```

## 🛠️ API Reference

### MCP Tools Available

#### 1. Calculator Tool
```json
{
  "method": "tools/call",
  "name": "calculator",
  "arguments": {
    "operation": "+",
    "a": 10,
    "b": 5
  }
}
```

#### 2. Echo Tool
```json
{
  "method": "tools/call",
  "name": "echo",
  "arguments": {
    "text": "Hello, MCP!",
    "repeat": 1
  }
}
```

#### 3. Word Counter Tool
```json
{
  "method": "tools/call",
  "name": "word_counter",
  "arguments": {
    "text": "Count these words"
  }
}
```

### Core MCP Endpoints

- **`GET /api/mcp/capabilities`** - Get server capabilities
- **`POST /api/mcp/tools/list`** - List available tools
- **`POST /api/mcp/tools/call`** - Execute a tool
- **`POST /api/mcp/resources/list`** - List available resources
- **`POST /api/mcp/resources/read`** - Read a resource
- **`POST /api/mcp/prompts/list`** - List available prompts
- **`POST /api/mcp/prompts/get`** - Get a prompt template

## 📁 Project Structure

```
MCPSample/
├── MCPWebApp/                          # Main web application
│   ├── Controllers/
│   │   ├── HomeController.cs           # Web interface controller
│   │   └── McpController.cs            # MCP API implementation
│   ├── Views/                          # Razor views
│   │   ├── Home/Index.cshtml           # Interactive web interface
│   │   └── Shared/_Layout.cshtml       # Layout template
│   ├── wwwroot/                        # Static web assets
│   │   ├── css/                        # Stylesheets
│   │   ├── js/site.js                  # Interactive tool JavaScript
│   │   └── lib/                        # Bootstrap, jQuery
│   ├── Program.cs                      # Application configuration
│   └── MCPWebApp.csproj               # Web project file
│
├── MCPCalculatorClient/                # Console calculator client
│   ├── Program.cs                      # Client implementation
│   └── MCPCalculatorClient.csproj      # Client project file
│
├── MCPCalculatorTestHarness/           # Automated testing
│   ├── Program.cs                      # Test harness implementation
│   └── MCPCalculatorTestHarness.csproj # Test project file
│
├── MCPSample.sln                       # Solution file
└── README.md                           # This documentation
```

## 🔧 Configuration

### Default URLs
- **Web Server**: `http://localhost:5272` (MCPWebApp)
- **Client Default**: `http://localhost:5202` (configurable)

### Environment Configuration
The web application uses standard ASP.NET Core configuration:
- `appsettings.json` - Base configuration
- `appsettings.Development.json` - Development overrides

## 🧪 Testing & Validation

### Manual Testing
1. **Web Interface**: Use the interactive UI at `http://localhost:5272`
2. **Command Line**: Test individual calculations with the console client
3. **API Direct**: Use Swagger UI at `http://localhost:5272/api-docs`

### Automated Testing
1. **Full Test Suite**: Run the test harness for comprehensive validation
2. **Continuous Integration**: Test harness can be integrated into CI/CD pipelines
3. **Error Scenarios**: Includes testing of error conditions and edge cases

## ⚠️ Error Handling

The system includes comprehensive error handling for:
- **Invalid operations** (unsupported operators)
- **Invalid numbers** (non-numeric inputs)
- **Division by zero** (mathematical errors)
- **Network errors** (API connectivity issues)
- **Malformed requests** (JSON parsing errors)
- **Server errors** (HTTP status codes)

## 🌟 Features & Benefits

### ✅ Complete MCP Implementation
- Full compliance with Model Context Protocol specification
- Standardized request/response formats
- Proper error handling and status codes

### ✅ Production-Ready Code
- Comprehensive error handling
- Input validation and sanitization
- Logging and monitoring capabilities
- Swagger/OpenAPI documentation

### ✅ Developer-Friendly
- Clear separation of concerns
- Modular architecture
- Extensive documentation
- Interactive testing interface

### ✅ Automated Testing
- 21 comprehensive test cases
- 100% automated validation
- Detailed reporting
- CI/CD ready

## 📚 Learn More

- **[Model Context Protocol Specification](https://modelcontextprotocol.io/)** - Official MCP documentation
- **[ASP.NET Core Documentation](https://docs.microsoft.com/aspnet/core/)** - Web framework documentation
- **[Swagger/OpenAPI](https://swagger.io/)** - API documentation standard

## 📄 License

This project is for demonstration and educational purposes, showcasing best practices for MCP implementation with .NET technologies.

---

**Built with ❤️ using ASP.NET Core, following Model Context Protocol standards.**
