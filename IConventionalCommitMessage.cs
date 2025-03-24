namespace CharTek.ConventionalCommits;

public interface IConventionalCommitMessage
{
    /// <summary>
    /// A noun which defines what kind of change was made.
    /// <example>"feat", "fix", etc.</example>
    /// </summary>
    string CommitType { get; }
    
    /// <summary>
    /// Plain-text short description of the change made.
    /// </summary>
    string Description { get; }
    
    /// <summary>
    /// A noun which defines the scope of the change, e.g. which area of the
    /// codebase is affected.
    /// </summary>
    string? Scope { get; }
    
    /// <summary>
    /// Any additional text in the commit message.
    /// </summary>
    string? Body { get; }
    
    /// <summary>
    /// Whether this commit represents a breaking change, i.e. whether this
    /// commit should also increment the MAJOR version under semver.
    /// </summary>
    bool IsBreakingChange { get; }
    
    /// <summary>
    /// Whether this commit represents a new feature, i.e. whether this
    /// commit should also increment the MINOR version under semver.
    /// </summary>
    bool IsNewFeature { get; }
    
    /// <summary>
    /// Whether this commit represents a bug fix, i.e. whether this
    /// commit should also increment the PATCH version under semver.
    /// </summary>
    bool IsBugFix { get; }
}