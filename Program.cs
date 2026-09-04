class Program
{
    static void Main(string[] args)
    {
        string code = """
              start
                while(5 < 4)
                  puts("10")
                end
              finish
              """;

        while (code.Length > 0)
        {
            Token token = Lexer.getNextToken(code);
            Console.WriteLine(token);
            code = code.Substring(token.lexeme.Length);
        }
    }
}
