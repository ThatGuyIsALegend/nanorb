class SyntaxNode
{
    public string label;
    public Token? token;
    public int line;
    public List<SyntaxNode> children = [];

    public SyntaxNode(string _label, Token? _token = null, int _line = 0)
    {
        this.label = _label;
        this.token = _token;
        this.line = _line;
    }

    public void Add(SyntaxNode child)
    {
        children.Add(child);
    }

    public string ToTreeString(string indent = "")
    {
        string self = indent + label;
        if (token != null && token.type != TokenType.NEWLINE && !string.IsNullOrEmpty(token.lexeme))
            self += $": {token.lexeme}";
        if (token != null)
            self += $" [linea {line}]";
        string result = self + "\n";
        foreach (SyntaxNode child in children)
            result += child.ToTreeString(indent + "  ");
        return result;
    }

    override public string ToString()
    {
        return ToTreeString();
    }
}
