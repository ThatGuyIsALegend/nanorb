class Program
{
    static void Main(string[] args)
    {
        string code = """
              start
                whole edad1
                dec precio
                edad1 = 0
                precio = 3.14
                puts "Ingresa tu edad" # Esto es un comentario
                gets edad1
                contador = edad1 + 25
                if contador == 100
                  puts "Hola", contador
                elsif contador != 100
                  puts precio, contador
                else
                  x = 0
                end
                while contador < 1000
                  contador = contador + 1 * 2 - 10.5 / 0.25
                end
                if contador <= 25
                  x = contador
                end
                if contador > 0
                  nombre = contador
                end
                if contador >= 25
                  puts nombre
                end
                finish
              """;

        code = code.Replace("\r\n", "\n"); // CAMBIO: normaliza saltos de línea Windows (\r\n) a Unix (\n)
                                           // para que el \r no se clasifique como carácter inválido (ERROR)

        List<(Token token, int line)> tokens = Tokenize(code);

        Console.WriteLine("| # | Lexema | Tipo de token | Linea |");
        Console.WriteLine("|---|--------|---------------|-------|");
        int row = 1;
        foreach ((Token token, int line) in tokens)
        {
            if (token.type == TokenType.NEWLINE || token.type == TokenType.EOF)
                continue;
            Console.WriteLine($"| {row} | {token.lexeme} | {token.type} | {line} |");
            row++;
        }

        Console.WriteLine();
        Console.WriteLine(Parser.Parse(tokens)
            ? "Analisis sintactico: ACEPTADO"
            : "Analisis sintactico: RECHAZADO");
    }

    static List<(Token token, int line)> Tokenize(string code)
    {
        List<(Token token, int line)> tokens = [];
        while (code.Length > 0)
        {
            Token token = Lexer.getNextToken(code);
            int line = token.type == TokenType.NEWLINE ? Lexer.line - 1 : Lexer.line;
            tokens.Add((token, line));
            code = code.Substring(token.length);
        }
        tokens.Add((new Token("", TokenType.EOF, 0), Lexer.line));
        return tokens;
    }
}
