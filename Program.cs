class Program
{
    static void Main(string[] args)
    {
        string code = """
              start
                whole edad1 = 0
                dec precio = 3.14
                puts("Ingresa tu edad") # Esto es un comentario
                gets(edad1)
                contador = edad1 + 25
                if(contador == 100)
                  puts("Hola", contador)
                elsif(contador != 100)
                  puts(precio, contador)
                else
                  x = 0
                end
                while(contador < 1000)
                  contador = contador + 1 * 2 - 10.5 / 0.25
                end
                if(contador <= 25)
                  x = contador
                end
                if(contador > 0)
                  nombre = "Mundo"
                end
                if(contador >= 25)
                  puts(nombre)
                end
              finish
              """;

        List<(string lexeme, TokenType type, int line)> symbolTable = [];

        while (code.Length > 0)
        {
            Token token = Lexer.getNextToken(code);
            if (token.type != TokenType.NEWLINE)
                symbolTable.Add((token.lexeme, token.type, Lexer.line));
            code = code.Substring(token.length);
        }

        Console.WriteLine("| # | Lexema | Tipo de token | Linea |");
        Console.WriteLine("|---|--------|---------------|-------|");
        int row = 1;
        foreach ((string lexeme, TokenType type, int line) in symbolTable)
        {
            Console.WriteLine($"| {row} | {lexeme} | {type} | {line} |");
            row++;
        }
    }
}
