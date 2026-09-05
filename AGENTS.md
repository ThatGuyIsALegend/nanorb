# AGENTS.md

Single .NET 10 console project (SDK 10.0.400): a lexer for a Ruby-styled toy language. No test project, no CI, no external dependencies.

## Commands

- `dotnet run` — the only verification available. `Program.cs` lexes a hardcoded snippet and prints tokens; check output by eye.
- `dotnet build` — build only. There is no `dotnet test` and no lint/format config.

## DFA invariants (break silently)

The lexer is a hand-maintained DFA; enum member ORDER is load-bearing:

- `Lexer.FinalStateToTokenType` computes `(int)state - (int)State.S100`, so the first members of `TokenType` (IDENTIFIER..COMA) must stay in exact lockstep with final states `S100`–`S118` in `State.cs`. Never insert or reorder enum members in the middle; append.
- `transition_matrix` rows are indexed by `(int)State`, columns by `(int)CharacterClass`. Adding a `CharacterClass` member requires adding a column to all 11 rows; adding a `State` requires a row.
- `S500`–`S503` are error states: not final, never converted to tokens.
- Reserved words have no final states — they are IDENTIFIERs rewritten by `getReserved()` in Lexer.cs. New keywords need both an appended `RESERVED_*` in `TokenType` and an entry in `getReserved()`.
- The driver loop in `Program.cs` slices input with `code.Substring(token.length)` each iteration; `Token.length` is captured before rollback/trim in `getNextToken`, so changes to lexeme/length handling must keep that loop consistent.

## Conventions

- No namespaces; all types are global. `Lexer` is a static class of static methods; `Token` uses public fields, no properties.
- Commit messages use `Add:` / `Feat:` / `Fix:` prefixes.
