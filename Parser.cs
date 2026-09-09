static class Parser
{
    static List<(Token token, int line)> tokens = [];
    static int position = 0;

    static Token Look => tokens[position].token;
    static int Line => tokens[position].line;

    public static bool Parse(List<(Token token, int line)> sourceTokens)
    {
        tokens = sourceTokens;
        position = 0;
        try
        {
            Programa();
            return true;
        }
        catch (SyntaxErrorException error)
        {
            Console.WriteLine(error.Message);
            return false;
        }
    }

    static void Programa()
    {
        Expect(TokenType.RESERVED_START);
        Expect(TokenType.NEWLINE);
        Lineas();
        Expect(TokenType.RESERVED_FINISH);
        while (Check(TokenType.NEWLINE))
            Advance();
        if (Look.type != TokenType.EOF)
            throw Error("fin del archivo");
    }

    static void Lineas()
    {
        while (IsPrimerosLinea(Look.type))
            Linea();
    }

    static bool IsPrimerosLinea(TokenType type)
    {
        return type is TokenType.NEWLINE
            or TokenType.RESERVED_WHOLE
            or TokenType.RESERVED_DEC
            or TokenType.IDENTIFIER
            or TokenType.RESERVED_GETS
            or TokenType.RESERVED_PUTS
            or TokenType.RESERVED_IF
            or TokenType.RESERVED_WHILE;
    }

    static void Linea()
    {
        if (Check(TokenType.NEWLINE))
        {
            Advance();
            return;
        }
        Instruccion();
        Expect(TokenType.NEWLINE);
    }

    static void Instruccion()
    {
        switch (Look.type)
        {
            case TokenType.RESERVED_WHOLE or TokenType.RESERVED_DEC:
                Declaracion();
                break;
            case TokenType.IDENTIFIER:
                Asignacion();
                break;
            case TokenType.RESERVED_GETS:
                Entrada();
                break;
            case TokenType.RESERVED_PUTS:
                Salida();
                break;
            case TokenType.RESERVED_IF:
                Condicional();
                break;
            case TokenType.RESERVED_WHILE:
                CicloWhile();
                break;
            default:
                throw Error("una instruccion");
        }
    }

    static void Declaracion()
    {
        if (Look.type is TokenType.RESERVED_WHOLE or TokenType.RESERVED_DEC)
            Advance();
        else
            throw Error("'whole' o 'dec'");
        Expect(TokenType.IDENTIFIER);
    }

    static void Asignacion()
    {
        Expect(TokenType.IDENTIFIER);
        Expect(TokenType.ASIGNATION);
        Expresion();
    }

    static void Entrada()
    {
        Expect(TokenType.RESERVED_GETS);
        Expect(TokenType.IDENTIFIER);
    }

    static void Salida()
    {
        Expect(TokenType.RESERVED_PUTS);
        ElemSalida();
        while (Check(TokenType.COMA))
        {
            Advance();
            ElemSalida();
        }
    }

    static void ElemSalida()
    {
        if (Check(TokenType.STRING))
            Advance();
        else
            Expresion();
    }

    static void Condicional()
    {
        Expect(TokenType.RESERVED_IF);
        Condicion();
        Expect(TokenType.NEWLINE);
        Lineas();
        while (Check(TokenType.RESERVED_ELSIF))
        {
            Advance();
            Condicion();
            Expect(TokenType.NEWLINE);
            Lineas();
        }
        if (Check(TokenType.RESERVED_ELSE))
        {
            Advance();
            Expect(TokenType.NEWLINE);
            Lineas();
        }
        Expect(TokenType.RESERVED_END);
    }

    static void CicloWhile()
    {
        Expect(TokenType.RESERVED_WHILE);
        Condicion();
        Expect(TokenType.NEWLINE);
        Lineas();
        Expect(TokenType.RESERVED_END);
    }

    static void Condicion()
    {
        Expresion();
        if (Look.type is TokenType.LESS_THAN
            or TokenType.LESS_OR_EQUAL_THAN
            or TokenType.GREATER_THAN
            or TokenType.GREATER_OR_EQUAL_THAN
            or TokenType.EQUALITY
            or TokenType.INEQUALITY)
            Advance();
        else
            throw Error("un operador relacional");
        Expresion();
    }

    static void Expresion()
    {
        Termino();
        while (Look.type is TokenType.SUM or TokenType.SUBSTRACTION)
        {
            Advance();
            Termino();
        }
    }

    static void Termino()
    {
        Factor();
        while (Look.type is TokenType.MULTIPLICATION or TokenType.DIVISION)
        {
            Advance();
            Factor();
        }
    }

    static void Factor()
    {
        if (Check(TokenType.OPENING_PARENTHESIS))
        {
            Advance();
            Expresion();
            Expect(TokenType.CLOSING_PARENTHESIS);
        }
        else if (Look.type is TokenType.IDENTIFIER or TokenType.INT or TokenType.REAL)
            Advance();
        else
            throw Error("id, num_entero, num_real o '('");
    }

    static bool Check(TokenType type) => Look.type == type;

    static void Advance()
    {
        if (position < tokens.Count - 1)
            position++;
    }

    static void Expect(TokenType type)
    {
        if (Look.type != type)
            throw Error(Describe(type));
        Advance();
    }

    static SyntaxErrorException Error(string esperado)
    {
        string encontrado = Look.type switch
        {
            TokenType.EOF => "fin del archivo",
            TokenType.ERROR => $"token no valido '{Look.lexeme}'",
            _ => $"'{Look.lexeme}'",
        };
        return new SyntaxErrorException($"Error de sintaxis en linea {Line}: se esperaba {esperado}, se encontro {encontrado}.");
    }

    static string Describe(TokenType type) => type switch
    {
        TokenType.IDENTIFIER => "id",
        TokenType.NEWLINE => "salto de linea",
        TokenType.INT => "num_entero",
        TokenType.REAL => "num_real",
        TokenType.STRING => "cadena",
        TokenType.ASIGNATION => "'='",
        TokenType.EQUALITY => "'=='",
        TokenType.INEQUALITY => "'!='",
        TokenType.LESS_THAN => "'<'",
        TokenType.LESS_OR_EQUAL_THAN => "'<='",
        TokenType.GREATER_THAN => "'>'",
        TokenType.GREATER_OR_EQUAL_THAN => "'>='",
        TokenType.SUM => "'+'",
        TokenType.SUBSTRACTION => "'-'",
        TokenType.MULTIPLICATION => "'*'",
        TokenType.DIVISION => "'/'",
        TokenType.OPENING_PARENTHESIS => "'('",
        TokenType.CLOSING_PARENTHESIS => "')'",
        TokenType.COMA => "','",
        TokenType.RESERVED_START => "'start'",
        TokenType.RESERVED_FINISH => "'finish'",
        TokenType.RESERVED_WHOLE => "'whole'",
        TokenType.RESERVED_DEC => "'dec'",
        TokenType.RESERVED_IF => "'if'",
        TokenType.RESERVED_ELSIF => "'elsif'",
        TokenType.RESERVED_ELSE => "'else'",
        TokenType.RESERVED_END => "'end'",
        TokenType.RESERVED_WHILE => "'while'",
        TokenType.RESERVED_GETS => "'gets'",
        TokenType.RESERVED_PUTS => "'puts'",
        TokenType.EOF => "fin del archivo",
        _ => "un token valido",
    };
}

class SyntaxErrorException : Exception
{
    public SyntaxErrorException(string message) : base(message) { }
}
