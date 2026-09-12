static class Parser
{
    static List<(Token token, int line)> tokens = [];
    static int position = 0;

    static Token Look => tokens[position].token;
    static int Line => tokens[position].line;

    public static SyntaxNode? Parse(List<(Token token, int line)> sourceTokens)
    {
        tokens = sourceTokens;
        position = 0;
        try
        {
            return Programa();
        }
        catch (SyntaxErrorException error)
        {
            Console.WriteLine(error.Message);
            return null;
        }
    }

    static SyntaxNode Programa()
    {
        SyntaxNode node = new("Programa");
        node.Add(Expect(TokenType.RESERVED_START));
        node.Add(Expect(TokenType.NEWLINE));
        node.Add(Lineas());
        node.Add(Expect(TokenType.RESERVED_FINISH));
        while (Check(TokenType.NEWLINE))
            node.Add(Expect(TokenType.NEWLINE));
        if (Look.type != TokenType.EOF)
            throw Error("fin del archivo");
        return node;
    }

    static SyntaxNode Lineas()
    {
        SyntaxNode node = new("Lineas");
        while (IsPrimerosLinea(Look.type))
        {
            SyntaxNode? linea = Linea();
            if (linea != null)
                node.Add(linea);
        }
        return node;
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

    static SyntaxNode? Linea()
    {
        if (Check(TokenType.NEWLINE))
        {
            Advance();
            return null;
        }
        SyntaxNode instruccion = Instruccion();
        Expect(TokenType.NEWLINE);
        return instruccion;
    }

    static SyntaxNode Instruccion()
    {
        switch (Look.type)
        {
            case TokenType.RESERVED_WHOLE or TokenType.RESERVED_DEC:
                return Declaracion();
            case TokenType.IDENTIFIER:
                return Asignacion();
            case TokenType.RESERVED_GETS:
                return Entrada();
            case TokenType.RESERVED_PUTS:
                return Salida();
            case TokenType.RESERVED_IF:
                return Condicional();
            case TokenType.RESERVED_WHILE:
                return CicloWhile();
            default:
                throw Error("una instruccion");
        }
    }

    static SyntaxNode Declaracion()
    {
        SyntaxNode node = new("Declaracion");
        if (Look.type is TokenType.RESERVED_WHOLE or TokenType.RESERVED_DEC)
            node.Add(Leaf(Advance()));
        else
            throw Error("'whole' o 'dec'");
        node.Add(Expect(TokenType.IDENTIFIER));
        return node;
    }

    static SyntaxNode Asignacion()
    {
        SyntaxNode node = new("Asignacion");
        node.Add(Expect(TokenType.IDENTIFIER));
        node.Add(Expect(TokenType.ASIGNATION));
        node.Add(Expresion());
        return node;
    }

    static SyntaxNode Entrada()
    {
        SyntaxNode node = new("Entrada");
        node.Add(Expect(TokenType.RESERVED_GETS));
        node.Add(Expect(TokenType.IDENTIFIER));
        return node;
    }

    static SyntaxNode Salida()
    {
        SyntaxNode node = new("Salida");
        node.Add(Expect(TokenType.RESERVED_PUTS));
        node.Add(ElemSalida());
        while (Check(TokenType.COMA))
        {
            node.Add(Expect(TokenType.COMA));
            node.Add(ElemSalida());
        }
        return node;
    }

    static SyntaxNode ElemSalida()
    {
        SyntaxNode node = new("ElemSalida");
        if (Check(TokenType.STRING))
            node.Add(Expect(TokenType.STRING));
        else
            node.Add(Expresion());
        return node;
    }

    static SyntaxNode Condicional()
    {
        SyntaxNode node = new("Condicional");
        node.Add(Expect(TokenType.RESERVED_IF));
        node.Add(Condicion());
        node.Add(Expect(TokenType.NEWLINE));
        node.Add(Lineas());
        while (Check(TokenType.RESERVED_ELSIF))
        {
            node.Add(Expect(TokenType.RESERVED_ELSIF));
            node.Add(Condicion());
            node.Add(Expect(TokenType.NEWLINE));
            node.Add(Lineas());
        }
        if (Check(TokenType.RESERVED_ELSE))
        {
            node.Add(Expect(TokenType.RESERVED_ELSE));
            node.Add(Expect(TokenType.NEWLINE));
            node.Add(Lineas());
        }
        node.Add(Expect(TokenType.RESERVED_END));
        return node;
    }

    static SyntaxNode CicloWhile()
    {
        SyntaxNode node = new("CicloWhile");
        node.Add(Expect(TokenType.RESERVED_WHILE));
        node.Add(Condicion());
        node.Add(Expect(TokenType.NEWLINE));
        node.Add(Lineas());
        node.Add(Expect(TokenType.RESERVED_END));
        return node;
    }

    static SyntaxNode Condicion()
    {
        SyntaxNode node = new("Condicion");
        node.Add(Expresion());
        if (Look.type is TokenType.LESS_THAN
            or TokenType.LESS_OR_EQUAL_THAN
            or TokenType.GREATER_THAN
            or TokenType.GREATER_OR_EQUAL_THAN
            or TokenType.EQUALITY
            or TokenType.INEQUALITY)
            node.Add(Leaf(Advance()));
        else
            throw Error("un operador relacional");
        node.Add(Expresion());
        return node;
    }

    static SyntaxNode Expresion()
    {
        SyntaxNode left = Termino();
        while (Look.type is TokenType.SUM or TokenType.SUBSTRACTION)
        {
            SyntaxNode node = new("Expresion");
            node.Add(left);
            node.Add(Leaf(Advance()));
            node.Add(Termino());
            left = node;
        }
        if (left.label != "Expresion")
        {
            SyntaxNode wrapper = new("Expresion");
            wrapper.Add(left);
            return wrapper;
        }
        return left;
    }

    static SyntaxNode Termino()
    {
        SyntaxNode left = Factor();
        while (Look.type is TokenType.MULTIPLICATION or TokenType.DIVISION)
        {
            SyntaxNode node = new("Termino");
            node.Add(left);
            node.Add(Leaf(Advance()));
            node.Add(Factor());
            left = node;
        }
        if (left.label != "Termino")
        {
            SyntaxNode wrapper = new("Termino");
            wrapper.Add(left);
            return wrapper;
        }
        return left;
    }

    static SyntaxNode Factor()
    {
        SyntaxNode node = new("Factor");
        if (Check(TokenType.OPENING_PARENTHESIS))
        {
            node.Add(Expect(TokenType.OPENING_PARENTHESIS));
            node.Add(Expresion());
            node.Add(Expect(TokenType.CLOSING_PARENTHESIS));
        }
        else if (Look.type is TokenType.IDENTIFIER or TokenType.INT or TokenType.REAL)
            node.Add(Leaf(Advance()));
        else
            throw Error("id, num_entero, num_real o '('");
        return node;
    }

    static SyntaxNode Leaf((Token token, int line) consumed)
    {
        return new SyntaxNode(consumed.token.type.ToString(), consumed.token, consumed.line);
    }

    static bool Check(TokenType type) => Look.type == type;

    static (Token token, int line) Advance()
    {
        (Token token, int line) current = tokens[position];
        if (position < tokens.Count - 1)
            position++;
        return current;
    }

    static SyntaxNode Expect(TokenType type)
    {
        if (Look.type != type)
            throw Error(Describe(type));
        return Leaf(Advance());
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
