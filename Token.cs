
class Token
{
    public string lexeme;
    public TokenType type;

    public Token(string _lexeme, TokenType _type)
    {
        this.lexeme = _lexeme;
        this.type = _type;
    }

    override public string ToString()
    {
        return "Lexeme: " + lexeme + " Type: " + type;
    }
}
