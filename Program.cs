class Program
{
    static void Main(string[] args)
    {
        string code = """
              start
                while(5 < 4)
                  puts("*+-!=")
                  puts(10.67)
                  a = 6 + 7
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
