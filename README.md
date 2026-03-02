# datadog-apm-trace-id

CLI tool to convert Datadog APM trace IDs and span IDs between 64-bit decimal and 128-bit hexadecimal representations. Automatically detects the ID format and extracts the embedded timestamp from Datadog 128-bit trace IDs.

## Usage

```
dd-trace-id <128-bit trace id as hexadecimal>
dd-trace-id <64-bit trace id as hexadecimal>
dd-trace-id <lower 64 bits of trace id as decimal>
dd-trace-id <upper 64 bits as decimal> <lower 64 bits as decimal>
```

### Examples

```bash
# 128-bit hex → upper/lower decimal + embedded timestamp
dd-trace-id a1b2c3d400000000abcdef0123456789

# 64-bit hex → decimal
dd-trace-id 0123456789abcdef

# 64-bit decimal → hex
dd-trace-id 81985529216486895

# upper + lower decimal → 128-bit hex
dd-trace-id 2712847316 12379813738877118345
```

## License

This project is licensed under the [MIT License](LICENSE).
