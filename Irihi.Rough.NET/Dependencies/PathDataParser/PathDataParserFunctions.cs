using System.Globalization;
using System.Text.RegularExpressions;

namespace Irihi.Rough.NET.Dependencies.PathDataParser;

internal partial class RegexHelper
{
    [GeneratedRegex(@"^([ \t\r\n,]+)")]
    internal static partial Regex WhiteSpaceRegex();

    [GeneratedRegex(@"^([aAcChHlLmMqQsStTvVzZ])")]
    internal static partial Regex CommandRegex();

    [GeneratedRegex(@"^(([-+]?[0-9]+(\.[0-9]*)?|[-+]?\.[0-9]+)([eE][-+]?[0-9]+)?)")]
    internal static partial Regex NumberRegex();
}

public static class PathDataParserFunctions
{
    private static readonly IReadOnlyDictionary<char, int> CommandParameters = new Dictionary<char, int>
    {
        { 'A', 7 }, { 'C', 6 }, { 'H', 1 }, { 'L', 2 }, { 'M', 2 },
        { 'Q', 4 }, { 'S', 4 }, { 'T', 2 }, { 'V', 1 }, { 'Z', 0 }
    };

    internal static List<PathToken> Tokenize(string d)
    {
        List<PathToken> tokens = [];
        var span = d.AsSpan();

        while (span.Length > 0)
        {
            if (RegexHelper.WhiteSpaceRegex().IsMatch(span))
            {
                foreach (var match in RegexHelper.WhiteSpaceRegex().EnumerateMatches(span))
                {
                    if (match.Length == 0) continue;
                    span = span[match.Length..];
                    break;
                }
            }
            else if (RegexHelper.CommandRegex().IsMatch(span))
            {
                foreach (var match in RegexHelper.CommandRegex().EnumerateMatches(span))
                {
                    if (match.Length == 0) continue;
                    tokens.Add(new PathToken(TokenType.Command, span.Slice(match.Index, match.Length).ToString()));
                    span = span[match.Length..];
                    break;
                }
            }
            else if (RegexHelper.NumberRegex().IsMatch(span))
            {
                foreach (var match in RegexHelper.NumberRegex().EnumerateMatches(span))
                {
                    if (match.Length == 0) continue;
                    tokens.Add(new PathToken(TokenType.Number, span.Slice(match.Index, match.Length).ToString()));
                    span = span[match.Length..];
                    break;
                }
            }
            else
            {
                return [];
            }
        }

        tokens.Add(PathToken.End);
        return tokens;
    }

    public static List<Segment> ParsePath(string d)
    {
        List<Segment> segments = [];
        var tokens = Tokenize(d);
        var mode = "BOD";
        var index = 0;

        while (index < tokens.Count && tokens[index].Type != TokenType.Eod)
        {
            var token = tokens[index];
            int paramsCount = 0;
            List<double> parameters = [];

            if (mode == "BOD")
            {
                if (token.Text is "M" or "m")
                {
                    index++;
                    paramsCount = CommandParameters[char.ToUpper(token.Text[0])];
                    mode = token.Text;
                }
                else
                {
                    return ParsePath("M0,0" + d);
                }
            }
            else if (token.Type == TokenType.Number)
            {
                paramsCount = CommandParameters[char.ToUpper(mode[0])];
            }
            else
            {
                index++;
                paramsCount = CommandParameters[char.ToUpper(token.Text[0])];
                mode = token.Text;
            }

            if (index + paramsCount < tokens.Count)
            {
                for (var i = index; i < index + paramsCount; i++)
                {
                    var numberToken = tokens[i];
                    if (numberToken.Type == TokenType.Number)
                    {
                        parameters.Add(double.Parse(numberToken.Text, CultureInfo.InvariantCulture));
                    }
                    else
                    {
                        throw new Exception($"Parameter not a number: {mode},{numberToken.Text}");
                    }
                }

                if (CommandParameters.ContainsKey(char.ToUpper(mode[0])))
                {
                    segments.Add(new Segment(mode[0], parameters.ToArray()));
                    index += paramsCount;
                    mode = mode switch
                    {
                        "M" => "L",
                        "m" => "l",
                        _ => mode
                    };
                }
                else
                {
                    throw new Exception($"Bad segment: {mode}");
                }
            }
            else
            {
                throw new Exception("Path data ended short");
            }
        }

        return segments;
    }

    public static string Serialize(List<Segment> segments)
    {
        List<string> tokens = [];
        foreach (var segment in segments)
        {
            tokens.Add(segment.Key.ToString());
            switch (segment.Key)
            {
                case 'C':
                case 'c':
                    tokens.Add(segment.Data[0].ToString(CultureInfo.InvariantCulture));
                    tokens.Add($"{segment.Data[1].ToString(CultureInfo.InvariantCulture)},");
                    tokens.Add(segment.Data[2].ToString(CultureInfo.InvariantCulture));
                    tokens.Add($"{segment.Data[3].ToString(CultureInfo.InvariantCulture)},");
                    tokens.Add(segment.Data[4].ToString(CultureInfo.InvariantCulture));
                    tokens.Add(segment.Data[5].ToString(CultureInfo.InvariantCulture));
                    break;
                case 'S':
                case 's':
                case 'Q':
                case 'q':
                    tokens.Add(segment.Data[0].ToString(CultureInfo.InvariantCulture));
                    tokens.Add($"{segment.Data[1].ToString(CultureInfo.InvariantCulture)},");
                    tokens.Add(segment.Data[2].ToString(CultureInfo.InvariantCulture));
                    tokens.Add(segment.Data[3].ToString(CultureInfo.InvariantCulture));
                    break;
                default:
                    tokens.AddRange(segment.Data.Select(d => d.ToString(CultureInfo.InvariantCulture)));
                    break;
            }
        }

        return string.Join(" ", tokens);
    }
}

internal enum TokenType
{
    Command = 0,
    Number = 1,
    Eod = 2
}

internal record struct PathToken(TokenType Type, string Text)
{
    public static PathToken End => new(TokenType.Eod, string.Empty);
}

public record struct Segment(char Key, double[] Data);