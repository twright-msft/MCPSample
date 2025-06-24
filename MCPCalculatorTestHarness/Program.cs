using System.Diagnostics;

namespace MCPCalculatorTestHarness;

/// <summary>
/// Test harness for the MCP Calculator Client
/// Runs comprehensive tests with various number types and operations
/// </summary>
class Program
{
    private static readonly string CalculatorClientPath = Path.Combine(
        AppDomain.CurrentDomain.BaseDirectory, 
        "..", "..", "..", "..", 
        "MCPCalculatorClient", "bin", "Debug", "net9.0", "MCPCalculatorClient.exe");

    private static readonly TestCase[] TestCases = new[]
    {
        // Addition tests
        new TestCase("Basic addition", "10", "+", "5", "15"),
        new TestCase("Decimal addition", "7.5", "+", "2.3", "9.8"),
        new TestCase("Negative addition", "-5", "+", "3", "-2"),
        new TestCase("Zero addition", "0", "+", "42", "42"),
        new TestCase("Large numbers", "1000000", "+", "2000000", "3000000"),

        // Subtraction tests
        new TestCase("Basic subtraction", "15", "-", "7", "8"),
        new TestCase("Decimal subtraction", "10.5", "-", "3.2", "7.3"),
        new TestCase("Negative subtraction", "-10", "-", "-5", "-5"),
        new TestCase("Zero subtraction", "25", "-", "0", "25"),
        new TestCase("Result becomes negative", "5", "-", "10", "-5"),

        // Multiplication tests
        new TestCase("Basic multiplication", "6", "*", "7", "42"),
        new TestCase("Decimal multiplication", "2.5", "*", "4.0", "10"),
        new TestCase("Negative multiplication", "-3", "*", "4", "-12"),
        new TestCase("Zero multiplication", "999", "*", "0", "0"),
        new TestCase("Fractional multiplication", "0.5", "*", "8", "4"),        // Division tests
        new TestCase("Basic division", "20", "/", "4", "5"),
        new TestCase("Decimal division", "15.0", "/", "3.0", "5"),
        new TestCase("Negative division", "-12", "/", "3", "-4"),
        new TestCase("Fractional result", "7", "/", "2", "3.5"),
        new TestCase("Division by one", "42", "/", "1", "42"),

        // Exponentiation tests
        new TestCase("Basic exponentiation", "2", "^", "3", "8"),
        new TestCase("Alternative power notation", "3", "**", "4", "81"),
        new TestCase("Power of zero", "5", "^", "0", "1"),
        new TestCase("Power of one", "7", "^", "1", "7"),
        new TestCase("Square calculation", "9", "^", "2", "81"),
        new TestCase("Cube calculation", "4", "^", "3", "64"),
        new TestCase("Square root", "16", "^", "0.5", "4"),
        new TestCase("Square root of 9", "9", "**", "0.5", "3"),
        new TestCase("Cube root", "27", "^", "0.333333", "3"),
        new TestCase("Decimal base power", "2.5", "^", "2", "6.25"),
        new TestCase("Negative base even power", "-3", "^", "2", "9"),
        new TestCase("Negative base odd power", "-2", "^", "3", "-8"),
        new TestCase("Large exponentiation", "10", "^", "3", "1000"),

        // Edge cases (should fail gracefully)
        new TestCase("Division by zero", "10", "/", "0", "ERROR_EXPECTED")
    };

    static async Task<int> Main(string[] args)
    {
        Console.WriteLine("🧪 MCP Calculator Client Test Harness");
        Console.WriteLine("=====================================");
        Console.WriteLine();

        // Check if calculator client exists
        if (!File.Exists(CalculatorClientPath))
        {
            Console.WriteLine($"❌ Calculator client not found at: {CalculatorClientPath}");
            Console.WriteLine("Please build the MCPCalculatorClient project first.");
            Console.WriteLine("Run: dotnet build MCPCalculatorClient");
            return 1;
        }

        Console.WriteLine($"📍 Using calculator client: {CalculatorClientPath}");
        Console.WriteLine($"🧮 Running {TestCases.Length} test cases...");
        Console.WriteLine();

        int passed = 0;
        int failed = 0;
        var results = new List<TestResult>();

        foreach (var testCase in TestCases)
        {
            Console.WriteLine($"🔍 Test: {testCase.Description}");
            Console.WriteLine($"   Expression: {testCase.FirstNumber} {testCase.Operation} {testCase.SecondNumber}");
            
            var result = await RunTest(testCase);
            results.Add(result);

            if (result.Success)
            {
                Console.WriteLine($"   ✅ PASS - Result: {result.ActualResult}");
                passed++;
            }
            else
            {
                Console.WriteLine($"   ❌ FAIL - {result.ErrorMessage}");
                if (!string.IsNullOrEmpty(result.ActualResult))
                {
                    Console.WriteLine($"   📋 Actual: {result.ActualResult}");
                }
                if (!string.IsNullOrEmpty(result.ExpectedResult))
                {
                    Console.WriteLine($"   📋 Expected: {result.ExpectedResult}");
                }
                failed++;
            }
            Console.WriteLine();
        }

        // Summary
        Console.WriteLine("📊 Test Summary");
        Console.WriteLine("===============");
        Console.WriteLine($"✅ Passed: {passed}");
        Console.WriteLine($"❌ Failed: {failed}");
        Console.WriteLine($"📋 Total: {TestCases.Length}");
        Console.WriteLine($"📈 Success Rate: {(double)passed / TestCases.Length * 100:F1}%");
        Console.WriteLine();

        // Detailed failure report
        if (failed > 0)
        {
            Console.WriteLine("🔍 Failed Test Details:");
            Console.WriteLine("=======================");
            foreach (var result in results.Where(r => !r.Success))
            {
                Console.WriteLine($"• {result.TestCase.Description}");
                Console.WriteLine($"  Expression: {result.TestCase.FirstNumber} {result.TestCase.Operation} {result.TestCase.SecondNumber}");
                Console.WriteLine($"  Error: {result.ErrorMessage}");
                Console.WriteLine();
            }
        }

        return failed == 0 ? 0 : 1;
    }    private static async Task<TestResult> RunTest(TestCase testCase)
    {
        try
        {
            var arguments = $"{testCase.FirstNumber} {testCase.Operation} {testCase.SecondNumber} http://localhost:5272 -NumberOnly";
            
            using var process = new Process();
            process.StartInfo.FileName = CalculatorClientPath;
            process.StartInfo.Arguments = arguments;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.CreateNoWindow = true;

            var outputBuilder = new System.Text.StringBuilder();
            var errorBuilder = new System.Text.StringBuilder();

            process.OutputDataReceived += (sender, e) => {
                if (e.Data != null) outputBuilder.AppendLine(e.Data);
            };
            process.ErrorDataReceived += (sender, e) => {
                if (e.Data != null) errorBuilder.AppendLine(e.Data);
            };

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            // Wait for the process with a timeout
            bool exited = await Task.Run(() => process.WaitForExit(10000)); // 10 second timeout

            if (!exited)
            {
                process.Kill();
                return new TestResult(testCase, false, "", "", "Process timed out after 10 seconds");
            }

            var output = outputBuilder.ToString();
            var error = errorBuilder.ToString();

            // Check for division by zero case
            if (testCase.ExpectedResult == "ERROR_EXPECTED")
            {
                if (process.ExitCode != 0 || output.Contains("divide by zero", StringComparison.OrdinalIgnoreCase) || 
                    output.Contains("Cannot divide by zero", StringComparison.OrdinalIgnoreCase))
                {
                    return new TestResult(testCase, true, "Error correctly handled", testCase.ExpectedResult, "");
                }
                else
                {
                    return new TestResult(testCase, false, output.Trim(), testCase.ExpectedResult, "Expected error but got success");
                }
            }

            // For normal cases, check exit code and extract result
            if (process.ExitCode != 0)
            {
                return new TestResult(testCase, false, error.Trim(), testCase.ExpectedResult, $"Process exited with code {process.ExitCode}");
            }            // With -NumberOnly, the output should be just the numeric result
            var actualResultStr = output.Trim();
            
            // For floating point comparisons, we need some tolerance
            if (double.TryParse(actualResultStr, out var actualValue) && 
                double.TryParse(testCase.ExpectedResult, out var expectedValue))
            {
                var tolerance = 0.0001;
                var difference = Math.Abs(actualValue - expectedValue);
                
                if (difference <= tolerance)
                {
                    return new TestResult(testCase, true, actualResultStr, testCase.ExpectedResult, "");
                }
                else
                {
                    return new TestResult(testCase, false, actualResultStr, testCase.ExpectedResult, 
                        $"Numeric difference: {difference:F6} (tolerance: {tolerance})");
                }
            }

            // String comparison fallback
            if (actualResultStr.Equals(testCase.ExpectedResult, StringComparison.OrdinalIgnoreCase))
            {
                return new TestResult(testCase, true, actualResultStr, testCase.ExpectedResult, "");
            }

            return new TestResult(testCase, false, actualResultStr, testCase.ExpectedResult, "Result mismatch");
        }
        catch (Exception ex)
        {
            return new TestResult(testCase, false, "", testCase.ExpectedResult, $"Exception: {ex.Message}");
        }    }
}

public record TestCase(string Description, string FirstNumber, string Operation, string SecondNumber, string ExpectedResult);

public record TestResult(TestCase TestCase, bool Success, string ActualResult, string ExpectedResult, string ErrorMessage);
