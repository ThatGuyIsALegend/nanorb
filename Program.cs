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

        while (code.Length > 0)
        {
            Token token = Lexer.getNextToken(code);
            Console.WriteLine(token);
            code = code.Substring(token.length);
        }
    }
}
