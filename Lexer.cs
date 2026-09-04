static class Lexer
{
    static readonly State[][] transition_matrix =
    [
        [State.S1, State.S101, State.S2, State.S5, State.S6, State.S7, State.S8, State.S9, State.S112, State.S113, State.S114, State.S115, State.S116, State.S117, State.S118, State.S500, State.S10, State.S500, State.S500, State.S0, State.S0],
        [State.S1, State.S100, State.S1, State.S100, State.S100, State.S100, State.S100, State.S100, State.S100, State.S100, State.S100, State.S100, State.S100, State.S100, State.S100, State.S100, State.S100, State.S100, State.S100, State.S100, State.S100],
        [State.S102, State.S102, State.S2, State.S102, State.S102, State.S102, State.S102, State.S102, State.S102, State.S102, State.S102, State.S102, State.S102, State.S102, State.S3, State.S3, State.S102, State.S102, State.S102, State.S102, State.S102],
        [State.S501, State.S501, State.S4, State.S501, State.S501, State.S501, State.S501, State.S501, State.S501, State.S501, State.S501, State.S501, State.S501, State.S501, State.S501, State.S501, State.S501, State.S501, State.S501, State.S501, State.S501],
        [State.S103, State.S103, State.S4, State.S103, State.S103, State.S103, State.S103, State.S103, State.S103, State.S103, State.S103, State.S103, State.S103, State.S103, State.S103, State.S103, State.S103, State.S103, State.S103, State.S103, State.S103],
        [State.S5, State.S502, State.S5, State.S104, State.S5, State.S5, State.S5, State.S5, State.S5, State.S5, State.S5, State.S5, State.S5, State.S5, State.S5, State.S5, State.S5, State.S5, State.S5, State.S5, State.S5],
        [State.S105, State.S105, State.S105, State.S105, State.S106, State.S105, State.S105, State.S105, State.S105, State.S105, State.S105, State.S105, State.S105, State.S105, State.S105, State.S105, State.S105, State.S105, State.S105, State.S105, State.S105],
        [State.S503, State.S503, State.S503, State.S503, State.S107, State.S503, State.S503, State.S503, State.S503, State.S503, State.S503, State.S503, State.S503, State.S503, State.S503, State.S503, State.S503, State.S503, State.S503, State.S503, State.S503],
        [State.S108, State.S108, State.S108, State.S108, State.S109, State.S108, State.S108, State.S108, State.S108, State.S108, State.S108, State.S108, State.S108, State.S108, State.S108, State.S108, State.S108, State.S108, State.S108, State.S108, State.S108],
        [State.S110, State.S110, State.S110, State.S110, State.S111, State.S110, State.S110, State.S110, State.S110, State.S110, State.S110, State.S110, State.S110, State.S110, State.S110, State.S110, State.S110, State.S110, State.S110, State.S110, State.S110],
        [State.S10, State.S0, State.S10, State.S10, State.S10, State.S10, State.S10, State.S10, State.S10, State.S10, State.S10, State.S10, State.S10, State.S10, State.S10, State.S10, State.S10, State.S10, State.S10, State.S10, State.S10],
    ];

    internal static bool isFinalState(State state)
    {
        return state >= State.S100 && state <= State.S118;
    }

    internal static TokenType FinalStateToTokenType(State state)
    {
        return (TokenType)((int)state - (int)State.S100);
    }

    internal static CharacterClass classifyCharacter(char a)
    {
        if (Char.IsLetter(a))
            return CharacterClass.CHAR;
        else if (Char.IsDigit(a))
            return CharacterClass.DIGIT;
        else if (a == '\n')
            return CharacterClass.NEWLINE;
        else if (a == '"')
            return CharacterClass.APOSTROPHE;
        else if (a == '=')
            return CharacterClass.EQUAL;
        else if (a == '!')
            return CharacterClass.EXCLAMATION;
        else if (a == '<')
            return CharacterClass.LESS_THAN;
        else if (a == '>')
            return CharacterClass.GREATER_THAN;
        else if (a == '+')
            return CharacterClass.PLUS;
        else if (a == '-')
            return CharacterClass.MINUS;
        else if (a == '*')
            return CharacterClass.STAR;
        else if (a == '/')
            return CharacterClass.SLASH;
        else if (a == '(')
            return CharacterClass.OPENING_PARENTHESIS;
        else if (a == ')')
            return CharacterClass.CLOSING_PARENTHESIS;
        else if (a == ',')
            return CharacterClass.COMA;
        else if (a == '.')
            return CharacterClass.PERIOD;
        else if (a == '#')
            return CharacterClass.POUND_SIGN;
        else if (a == '\t')
            return CharacterClass.TAB;
        else if (a == ' ')
            return CharacterClass.SPACE;

        return CharacterClass.OTHER_CHAR;
    }

    public static Token getNextToken(string code)
    {
        string lexeme = "";
        State state = State.S0;
        State old_state = State.S0;
        char nextChar = code[0];

        while (!isFinalState(state))
        {
            nextChar = code.Length > 0 ? code[0] : '\0';                        // gets next char
            code = code.Length > 0 ? code.Substring(1) : code; // removes the char from code

            if (nextChar == '\0')
            {
                // Something here?
            }

            lexeme += nextChar;
            old_state = state;
            state = transition_matrix[(int)state][(int)classifyCharacter(nextChar)];
        }

        // Rollback
        code = nextChar + code;
        if (lexeme.Length > 1 && FinalStateToTokenType(state) != TokenType.STRING)
            lexeme = lexeme.Remove(lexeme.Length - 1);
        return new Token(lexeme, FinalStateToTokenType(state));
    }
}
