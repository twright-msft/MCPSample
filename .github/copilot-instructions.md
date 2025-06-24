# Copilot Instructions

<!-- Use this file to provide workspace-specific custom instructions to Copilot. For more details, visit https://code.visualstudio.com/docs/copilot/copilot-customization#_use-a-githubcopilotinstructionsmd-file -->

## Project Context
This is an ASP.NET Core MVC application designed to serve as an API for Model Context Protocol (MCP) operations.

## MCP-Specific Guidelines
- This project implements a Model Context Protocol (MCP) API server
- Follow MCP specification guidelines for request/response structures
- You can find more info and examples at https://modelcontextprotocol.io/llms-full.txt
- Implement proper error handling for MCP protocol operations
- Use appropriate HTTP status codes for MCP responses
- Structure models to match MCP data schemas
- Implement controllers for handling MCP protocol operations like tools, resources, and prompts

## Code Style Guidelines
- Use C# naming conventions (PascalCase for public members, camelCase for private fields)
- Follow ASP.NET Core best practices for controllers and models
- Use dependency injection where appropriate
- Implement proper async/await patterns for I/O operations
- Add XML documentation comments for public APIs
- Use data annotations for model validation
