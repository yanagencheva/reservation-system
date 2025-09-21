using SwiftProcessor.Worker.Models;
using System.Text;
using System.Text.RegularExpressions;

namespace SwiftProcessor.Worker.Utils;

public static class SwiftMessageParser
{
    public static SwiftMessage Parse(string raw)
    {
        if (string.IsNullOrWhiteSpace(raw)) throw new ArgumentException("Message is empty");

        var msg = new SwiftMessage();
        var blocks = ExtractBlocks(raw);

        if (blocks.TryGetValue('1', out var b1)) msg.RawBasicHeader = b1;
        if (blocks.TryGetValue('2', out var b2)) msg.RawApplicationHeader = b2;
        if (blocks.TryGetValue('3', out var b3)) msg.RawUserHeader = b3;
        if (blocks.TryGetValue('4', out var b4))
        {
            msg.RawTextBlock = b4;
            ParseBlock4Fields(b4, msg.Fields);
        }
        if (blocks.TryGetValue('5', out var b5))
        {
            msg.RawTrailer = b5;
            ParseTrailerFields(b5, msg.TrailerFields);
        }

        return msg;
    }

    private static Dictionary<char, string> ExtractBlocks(string s)
    {
        var dict = new Dictionary<char, string>();
        int i = 0, n = s.Length;

        while (i < n)
        {
            if (s[i] != '{') { i++; continue; }
            if (i + 2 >= n) break;

            char id = s[i + 1];
            if (id < '1' || id > '5' || s[i + 2] != ':') { i++; continue; }

            int j = i + 3, braceLevel = 1;
            while (j < n && braceLevel > 0)
            {
                if (s[j] == '{') braceLevel++;
                else if (s[j] == '}') braceLevel--;
                j++;
            }

            string content = s.Substring(i + 3, (j - 1) - (i + 3));
            dict[id] = content;
            i = j;
        }

        return dict;
    }

    private static void ParseBlock4Fields(string block4Content, Dictionary<string, List<string>> outFields)
    {
        var normalized = block4Content.Replace("\r\n", "\n").Trim();
        var lines = normalized.Split('\n').Select(l => l.TrimEnd('\r')).ToList();

        while (lines.Count > 0 && string.IsNullOrWhiteSpace(lines[0])) lines.RemoveAt(0);
        while (lines.Count > 0 && lines[^1].Trim() == "-") lines.RemoveAt(lines.Count - 1);

        string currentTag = null;
        var currentBuilder = new StringBuilder();

        foreach (var line in lines)
        {
            if (IsFieldStart(line, out string tag, out string remainder))
            {
                if (!string.IsNullOrEmpty(currentTag))
                {
                    AddField(outFields, currentTag, currentBuilder.ToString().TrimEnd());
                }
                currentTag = tag;
                currentBuilder.Clear();
                if (!string.IsNullOrEmpty(remainder)) currentBuilder.Append(remainder);
            }
            else if (currentTag != null)
            {
                if (currentBuilder.Length > 0) currentBuilder.Append('\n');
                currentBuilder.Append(line);
            }
        }

        if (!string.IsNullOrEmpty(currentTag))
        {
            AddField(outFields, currentTag, currentBuilder.ToString().TrimEnd());
        }
    }

    private static bool IsFieldStart(string line, out string tag, out string remainder)
    {
        tag = null; 
        remainder = null;
        if (string.IsNullOrEmpty(line) || !line.StartsWith(":")) return false;

        int secondColon = line.IndexOf(':', 1);
        if (secondColon <= 1) return false;

        tag = line.Substring(1, secondColon - 1).Trim();
        remainder = line.Length > secondColon + 1 ? line.Substring(secondColon + 1) : string.Empty;
        return true;
    }

    private static void AddField(Dictionary<string, List<string>> dict, string tag, string value)
    {
        if (!dict.TryGetValue(tag, out var list))
        {
            list = new List<string>();
            dict[tag] = list;
        }
        list.Add(value);
    }

    private static void ParseTrailerFields(string trailerContent, Dictionary<string, string> outFields)
    {
        var regex = new Regex(@"\{([A-Z]{3}):([^}]*)\}", RegexOptions.Singleline);
        foreach (Match match in regex.Matches(trailerContent))
        {
            outFields[match.Groups[1].Value] = match.Groups[2].Value.Trim();
        }
    }
}
