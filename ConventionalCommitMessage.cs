using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Text;

namespace CharTek.ConventionalCommits;

public partial class ConventionalCommitMessage(
    string commitType,
    string description,
    string? scope = null,
    string? body = null,
    bool isBreakingChange = false)
    : IConventionalCommitMessage
{
    public string CommitType { get; } = commitType;
    public string Description { get; } = description;
    public string? Scope { get; } = scope;
    public string? Body { get; } = body;
    public bool IsBreakingChange { get; } = isBreakingChange;

    public bool IsNewFeature => CommitType.Equals("feat", StringComparison.InvariantCultureIgnoreCase);
    public bool IsBugFix => CommitType.Equals("fix", StringComparison.InvariantCultureIgnoreCase);

    public override string ToString()
    {
        var typeAndScope = Scope is null 
            ? CommitType 
            : $"{CommitType}({Scope})";

        var breakingChange = IsBreakingChange
            ? "!" 
            : string.Empty;

        return Body is null
            ? $"{typeAndScope}{breakingChange}: {Description}"
            : $"""
               {typeAndScope}{breakingChange}: {Description}
               
               {Body}
               """;
    }

    /// <summary>
    /// Attempt to parse a Git commit message and convert it to
    /// a <see cref="ConventionalCommitMessage"/>.
    /// </summary>
    /// <param name="message">
    /// A Git commit message to parse.
    /// </param>
    /// <param name="result">
    /// An instance of <see cref="ConventionalCommitMessage"/> representing the
    /// commit message.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="message"/> was parsed
    /// successfully; otherwise, <see langword="false"/>.
    /// </returns>
    [Pure]
    public static bool TryParse(string message, [NotNullWhen(true)] out ConventionalCommitMessage? result)
    {
        result = null;
        
        try
        {
            result = Parse(message);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
    
    /// <summary>
    /// Parse a Git commit message and convert it to a <see cref="ConventionalCommitMessage"/>.
    /// </summary>
    /// <param name="message">
    /// A Git commit message to parse.
    /// </param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="message"/> is null.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown if <paramref name="message"/> is an empty string, or
    /// only whitespace characters.
    /// </exception>
    /// <exception cref="FormatException">
    /// Thrown if <paramref name="message"/> is not valid, according
    /// to the Conventional Commits v1.0.0 specification.
    /// </exception>
    [Pure]
    public static ConventionalCommitMessage Parse(string message)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(message);
        
        // First, clean up the message, and get the first line.
        var cleanMessage = message.Trim();
        
        using var reader = new StringReader(cleanMessage);
        var firstLine = reader.ReadLine()?.Trim();
        
        if (string.IsNullOrWhiteSpace(firstLine))
            throw new FormatException("Unable to parse, first line of commit message is empty.");

        // Split the first line into `type(scope): description`
        var splitOptions = StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries;
        var messageParts = firstLine.Split(": ",splitOptions);
        
        if (messageParts.Length != 2)
            throw new FormatException("Unable to parse, commit message does not define commit type.");

        // Determine whether the commit is a breaking change.
        var isBreakingChange = messageParts[0].EndsWith('!');
        
        // Split the type/scope apart from the description.
        var typeAndScope = messageParts[0].TrimEnd('!');
        var description = messageParts[1];
        
        // commitType is in the format `type(scope)`, so trim and split on the parentheses.
        var typeAndScopeParts = typeAndScope.TrimEnd(')').Split('(', StringSplitOptions.TrimEntries);

        // Either we have `type(scope)` or `type`, so the split should yield
        // either one or two parts - anything else means the text is invalid.
        var (commitType, scope) = typeAndScopeParts.Length switch
        {
            >2 => throw new FormatException("Unable to parse, more than one scope was specified."),
            2  => (typeAndScopeParts[0], typeAndScopeParts[1]),
            1  => (typeAndScope, null),
            _  => throw new FormatException("Unable to parse, commit type contains an invalid scope.")
        };

        // Assemble body line-by-line and set isBreakingChange if necessary.
        var sb = new StringBuilder();
        var line = reader.ReadLine();
        while (line != null)
        {
            sb.AppendLine(line);
            var cleanLine = line.Trim();
            
            if (cleanLine.StartsWith("BREAKING CHANGE: ") 
                || cleanLine.StartsWith("BREAKING-CHANGE: "))
            {
                isBreakingChange = true;
            }
            
            line = reader.ReadLine();
        }
        
        var body = sb.ToString().Trim();
        
        // If body is empty/whitespace, set it to null.
        if (string.IsNullOrEmpty(body))
            body = null;

        return new ConventionalCommitMessage(commitType, description, scope, body, isBreakingChange);
    }
}