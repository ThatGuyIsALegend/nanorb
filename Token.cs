
class Token
{
    public string lexeme;
    public TokenType type;
    public int length;

    public Token(string _lexeme, TokenType _type)
    {
        this.lexeme = _lexeme;
        this.type = _type;
    }

    public Token(string _lexeme, TokenType _type, int _length)
    {
        this.lexeme = _lexeme;
        this.type = _type;
        this.length = _length;
    }

    override public string ToString()
    {
        return "Lexeme: " + lexeme + " Type: " + type;
    }
}
