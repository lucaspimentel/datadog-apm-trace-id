# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This is a command-line utility for parsing and converting Datadog APM trace IDs between hexadecimal and decimal representations. It handles both 64-bit and 128-bit trace IDs and can extract timestamp information from Datadog 128-bit trace IDs.

## Build & Run

```bash
# Build the project
dotnet build

# Run the application
dotnet run -- <args>

# Publish as AOT executable (optimized)
dotnet publish -c Release -r win-x64
dotnet publish -c Release -r linux-x64
```

## Usage Examples

```bash
# Parse 128-bit trace id (32 hex chars)
dd-trace-id a1b2c3d4e5f6789012345678abcdef01

# Parse 64-bit trace id (16 hex chars)
dd-trace-id 0123456789abcdef

# Parse from decimal values
dd-trace-id 1234567890  # single 64-bit decimal
dd-trace-id 123456 7890123456  # upper and lower 64-bit decimals (space-separated)
```

## Architecture

### Core Components

- **Program.cs**: Entry point that parses command-line arguments and orchestrates conversion logic. Contains timestamp extraction logic for Datadog 128-bit trace IDs (Program.cs:98-114).

- **TraceId.cs**: Defines `TraceId` record struct representing a 128-bit trace ID as two `ulong` values (Upper and Lower). Includes comparison operators and conversion from smaller integer types.

- **HexString.cs**: Utility class wrapping `HexConverter` to convert between trace IDs and hexadecimal strings. Key methods:
  - `ToHexString()`: Converts `ulong` or `TraceId` to hex string
  - `TryParseTraceId()`: Parses 16 or 32 hex characters into a `TraceId`
  - `TryParseUInt64()`: Parses exactly 16 hex characters into a `ulong`

- **HexConverter.cs**: Vendored auto-generated code from .NET runtime for high-performance hex encoding/decoding. Uses SIMD optimizations on supported platforms.

- **ThrowHelper.cs**: Centralized exception throwing with `[DoesNotReturn]` attributes for better JIT optimization.

### Datadog 128-bit Trace ID Format

Datadog 128-bit trace IDs encode a Unix timestamp in the upper 32 bits:
- Bits 96-127 (first 8 hex chars): Unix timestamp in seconds
- Bits 64-95 (next 8 hex chars): Padding zeros (00000000)
- Bits 0-63 (last 16 hex chars): Random lower 64 bits

The tool automatically detects and displays this timestamp if the middle 8 hex characters are all zeros and the timestamp falls within a reasonable range (2000-2030).

## Build Configuration

The project uses aggressive Native AOT compilation settings for minimal binary size and maximum performance:

- **PublishAot=true**: Ahead-of-time compilation to native code
- **PublishTrimmed=true**: Removes unused code
- **OptimizationPreference=Speed**: Prioritizes runtime performance
- **InvariantGlobalization=true**: Removes globalization support to reduce size
- **AllowUnsafeBlocks=true**: Enables unsafe code for high-performance hex conversion

Target framework: .NET 8.0
