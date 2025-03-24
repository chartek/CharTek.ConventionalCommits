# CharTek.ConventionalCommits

A .NET library to easily parse Git commit messages into a format compatible
with the [Conventional Commits](https://www.conventionalcommits.org/en/v1.0.0/)
specification v1.0.0.

## Usage

```cs
string message = "feat(lang): add Polish language";

// Parse git commit message string
ConventionalCommitMessage commit = ConventionalCommitMessage.Parse(message);

// Alternatively, TryParse can be used:
bool canParse = ConventionalCommitMessage.TryParse(message, out var alternativeCommit);

// Now we can access properties of the conventional commit:
string type = commit.CommitType;
string desc = commit.Description;
string? scope = commit.Scope;
string? body = commit.Body;
bool isBreaking = commit.IsBreakingChange;
```

## License

CharTek.ConventionalCommits 
Copyright (C) 2025  CharTek Softworks

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as
published by the Free Software Foundation, either version 3 of the
License, or (at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
