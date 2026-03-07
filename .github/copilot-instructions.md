# GitHub Copilot Instructions for QuantSharp

## Architecture

- Follow **Clean Architecture** principles
- Separate concerns into layers: Domain, Application, Infrastructure, and Presentation
- Dependencies should point inward (Infrastructure → Application → Domain)
- Use dependency injection for loose coupling

## Code Style

- **All comments must be in English**
- Use XML documentation comments for public APIs
- Follow C# coding conventions and .NET naming guidelines
- Use meaningful variable and method names that express intent

## Project Structure

```
src/
├── QuantSharp/           # Core library
├── QuantSharp.Domain/    # Domain entities and interfaces
├── QuantSharp.Application/ # Business logic and use cases
└── QuantSharp.Infrastructure/ # External dependencies
```

## Coding Standards

### Naming Conventions
- Use PascalCase for class names, method names, and properties
- Use camelCase for local variables and parameters
- Use meaningful names: `CalculateMovingAverage` instead of `CalcMA`

### Design Patterns
- Repository pattern for data access
- CQRS for complex operations
- Strategy pattern for different calculation algorithms
- Factory pattern for object creation when appropriate

### Error Handling
- Use exceptions for exceptional cases only
- Return Result types for expected failures
- Validate input parameters and throw `ArgumentException` with clear messages

### Testing
- Write unit tests for all business logic
- Use AAA pattern (Arrange, Act, Assert)
- Mock external dependencies
- Aim for high code coverage on Domain and Application layers

## Financial Domain Specific

- Use `decimal` for monetary values and financial calculations
- Use `double` for statistical calculations and performance metrics
- Always consider timezone when working with DateTime
- Document calculation formulas in comments
- Include references to financial literature when implementing known algorithms

## Code Reviews

- Ensure all code follows Clean Architecture principles
- Verify all comments are in English
- Check for proper error handling
- Validate test coverage
- Review for performance implications in financial calculations

## Example Code Structure

```csharp
namespace QuantSharp.Domain.Entities;

/// <summary>
/// Represents a financial instrument with price data.
/// </summary>
public class Security
{
    /// <summary>
    /// Gets the unique identifier for the security.
    /// </summary>
    public string Symbol { get; init; }
    
    /// <summary>
    /// Gets the current market price.
    /// </summary>
    public decimal Price { get; init; }
    
    /// <summary>
    /// Calculates the simple moving average over the specified period.
    /// </summary>
    /// <param name="period">The number of periods to calculate.</param>
    /// <returns>The simple moving average value.</returns>
    public decimal CalculateSimpleMovingAverage(int period)
    {
        // Implementation following Clean Architecture
        throw new NotImplementedException();
    }
}
```
