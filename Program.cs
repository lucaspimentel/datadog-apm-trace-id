using Datadog.Trace;
using Datadog.Trace.Util;

// Console.OutputEncoding = Encoding.UTF8;

Console.WriteLine();

switch (args.Length)
{
    case 1:
    {
        var input = args[0];

        switch (input.Length)
        {
            case 32 when HexString.TryParseTraceId(input, out var id128):
                Console.WriteLine($"{input} looks like a 128-bit id.");
                Console.WriteLine();
                Console.WriteLine($"Upper 64 bits: {input[..16]} (hex) => {id128.Upper} (decimal)");
                Console.WriteLine($"Lower 64 bits: {input[16..]} (hex) => {id128.Lower} (decimal)");

                CheckForTimestamp(input);
                Console.WriteLine();
                return 0;
            case 16 when HexString.TryParseUInt64(input, out var id64):
                Console.WriteLine($"{input} looks like a 64-bit id.");
                Console.WriteLine();
                Console.WriteLine($"{input} (hex) => {id64} (decimal)");
                return 0;
        }

        if (ulong.TryParse(input, out var id))
        {
            var hex64 = HexString.ToHexString(id, lowerCase: true);

            Console.WriteLine($"{input} looks like a 64-bit id.");
            Console.WriteLine();
            Console.WriteLine($"{input} (decimal) => {hex64} (hex)");
            return 0;
        }

        break;
    }
    case 2:
    {
        var upper = args[0];
        var lower = args[1];

        if (ulong.TryParse(upper, out var upperId) &&
            ulong.TryParse(lower, out var lowerId))
        {
            var traceId = new TraceId(upperId, lowerId);
            var traceIdHex = traceId.ToString();

            Console.WriteLine($"Upper 64 bits: {upper} (decimal) => {traceIdHex[..16]} (hex)");
            Console.WriteLine($"Lower 64 bits: {lower} (decimal) => {traceIdHex[16..]} (hex)");
            Console.WriteLine();
            Console.WriteLine($"Full 128b-bit id: {traceId} (hex).");

            CheckForTimestamp(traceIdHex);
            Console.WriteLine();
            return 0;
        }

        if (HexString.TryParseTraceId(upper + lower, out var id))
        {
            Console.WriteLine($"Upper 64 bits: {upper} (hex) => {id.Upper} (decimal)");
            Console.WriteLine($"Lower 64 bits: {lower} (hex) => {id.Lower} (decimal)");
            Console.WriteLine();
            Console.WriteLine($"Full 128b-bit id: {id} (hex).");

            CheckForTimestamp(id.ToString());
            Console.WriteLine();
            return 0;
        }

        return 1;
    }
}

Console.WriteLine("Invalid input. Usage:");
Console.WriteLine("""
                    <command> <128-bit trace id as hexadecimal>
                    <command> <64-bit trace id as hexadecimal>
                    <command> <lower 64 bits of trace id as decimal>
                    <command> <upper 64 bits of trace id as decimal> <lower 64 bits of trace id as decimal>
                  """);

return 1;

void CheckForTimestamp(string s)
{
    // Datadog 128-bit trace ids have a timestamp component in the upper 32 bits,
    // let's show the timestamp if we can
    if (s[8..16] == "00000000")
    {
        var timestampHex = "00000000" + s[..8];

        if (HexString.TryParseUInt64(timestampHex, out var timestamp))
        {
            var date = DateTimeOffset.FromUnixTimeSeconds((long)timestamp);

            // sanity check the timestamp range
            if (date.Year is > 2000 and < 2030)
            {
                Console.WriteLine();
                Console.WriteLine($"This looks like a Datadog 128-bit trace id created on: {date:yyyy-MM-dd HH:mm:ss UTC}");
            }
        }
    }
}
