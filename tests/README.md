# MixEmul Test Suite

This directory contains the unit test projects for the MixEmul solution.

## Test Projects

### MixLib.Tests
Tests for the core MixLib library, including:

- **Type Tests** (`Type/`)
  - `MixByteTests.cs` - Tests for the 6-bit MixByte type (constructors, operators, conversions)
  - `FullWordTests.cs` - Tests for 5-byte FullWord type (value operations, sign handling, cloning)
  - `WordTests.cs` - Tests for Word base class (static methods, slicing, magnitude operations)
  - `FieldSpecTests.cs` - Tests for field specifications (bounds validation, byte indexing)
  - `ExtensionMethodsTests.cs` - Tests for extension methods (sign operations, comparison values)
  - `MixByteCollectionTests.cs` - Tests for MixByte collections (loading, cloning, enumeration)
  - `WordFieldTests.cs` - Tests for word field operations (loading, applying, comparing)
  - `IndexRegisterTests.cs` - Tests for 2-byte index registers with padding
  - `AddressRegisterTests.cs` - Tests for 2-byte address registers
  - `FullWordRegisterTests.cs` - Tests for 5-byte registers

- **Misc Tests** (`Misc/`)
  - `ValidationErrorTests.cs` - Tests for validation error messages and formatting

**Total: 279 tests**

### MixAssembler.Tests
Tests for the MixAssembler library, including:

- **Finding Tests** (`Finding/`)
  - `AssemblyFindingTests.cs` - Tests for assembly errors and warnings (severity, location tracking, message formatting)

- **Value Tests** (`Value/`)
  - `NumberValueTests.cs` - Tests for numeric literal parsing and value operations

**Total: 46 tests**

## Running Tests

### Run all tests in the solution:
```powershell
dotnet test MixEmul.sln
```

### Run tests for a specific project:
```powershell
dotnet test tests\MixLib.Tests\MixLib.Tests.csproj
dotnet test tests\MixAssembler.Tests\MixAssembler.Tests.csproj
```

### Run tests with detailed output:
```powershell
dotnet test --verbosity detailed
```

### Run tests with coverage:
```powershell
dotnet test --collect:"XPlat Code Coverage"
```

## Test Framework

- **xUnit 2.9.2** - Modern, extensible test framework
- **FluentAssertions 7.0.0** - Readable assertion library
- **Target Framework**: .NET 10 (net10.0-windows7.0)

## Test Philosophy

These tests are designed to:
1. **Validate intent**, not just implementation - Tests verify proper behavior, which may help identify bugs in production code
2. **Follow best practices** - Using modern patterns like Theory/InlineData for parameterized tests
3. **Be maintainable** - Clear naming, logical organization, and comprehensive coverage of core functionality
4. **Be fast** - Unit tests run in milliseconds, suitable for frequent execution during development

## Coverage Focus

The current test suite focuses on:
- Core type system (MixByte, Word, FullWord, FieldSpec)
- Type conversions and operators
- Validation and error reporting
- Extension methods for type manipulation

Future test additions could cover:
- Instruction execution
- Device I/O operations
- Memory and register operations
- Assembly parsing
- Mix computer simulation
