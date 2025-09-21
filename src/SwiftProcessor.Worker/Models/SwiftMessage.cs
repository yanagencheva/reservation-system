using System.Text.RegularExpressions;

namespace SwiftProcessor.Worker.Models;

public class SwiftMessage
{
    // Raw blocks
    public string? RawBasicHeader { get; set; }        // {1:...}
    public string? RawApplicationHeader { get; set; }  // {2:...}
    public string? RawUserHeader { get; set; }         // {3:...}
    public string? RawTextBlock { get; set; }          // {4:...}
    public string? RawTrailer { get; set; }            // {5:...}

    // Parsed fields
    public Dictionary<string, List<string>> Fields { get; } = new(StringComparer.OrdinalIgnoreCase);
    public Dictionary<string, string> TrailerFields { get; } = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Checks if this message is MT103+ (Block 3 must contain {119:STP}).
    /// </summary>
    public bool IsMt103Plus()
    {
        if (string.IsNullOrEmpty(RawUserHeader)) return false;
        return RawUserHeader.Contains("{119:STP}", StringComparison.OrdinalIgnoreCase)
            || Regex.IsMatch(RawUserHeader, @"\{119:\s*STP\s*\}", RegexOptions.IgnoreCase);
    }

    /// <summary>
    /// Helper to get all values for a given field tag (e.g. "20", "32A").
    /// </summary>
    public IEnumerable<string> GetField(string tag)
    {
        var key = tag.Trim(':');
        return Fields.TryGetValue(key, out var list) ? list : Array.Empty<string>();
    }
}
