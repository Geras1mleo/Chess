namespace Chess;

/// <summary>
/// Move on chess board
/// </summary>
public class Move
{
    const string ParametersSeparator = " - ";

    /// <summary>
    /// Whether Positions are initialized
    /// </summary>
    public bool HasValue => OriginalPosition.HasValue && NewPosition.HasValue;

    /// <summary>
    /// Moved Piece
    /// </summary>
    public Piece Piece { get; internal set; }

    /// <summary>
    /// Original position of moved piece
    /// </summary>
    public Position OriginalPosition { get; internal set; }

    /// <summary>
    /// New Position of moved piece
    /// </summary>
    public Position NewPosition { get; internal set; }

    /// <summary>
    /// Captured piece (if exist) or null
    /// </summary>
    public Piece? CapturedPiece { get; internal set; }

    /// <summary>
    /// Move additional parameter   
    /// </summary>
    public IMoveParameter? Parameter { get; internal set; }

    /// <summary>
    /// Move places opponent's king in check? => true
    /// </summary>
    public bool IsCheck { get; internal set; }

    /// <summary>
    /// Move places opponent's king in checkmate => true
    /// </summary>
    public bool IsMate { get; internal set; }

    /// <summary>
    /// Move in SAN Notation<br/>
    /// -> Use board.MoveToSan() to get san string for this move according to your board positions
    /// </summary>
    public string? San { get; internal set; }

    /// <summary>
    /// Initializes new Move that has to be validated
    /// </summary>
    public Move(Position originalPosition, Position newPosition)
    {
        OriginalPosition = originalPosition;
        NewPosition = newPosition;
    }

    /// <summary>
    /// Initalizes new Move from long move notation
    /// </summary>
    /// <param name="move">
    /// Move as long string<br/>
    /// ex.:{wr - a1 - h8 - bq - e.p. - +}<br/>
    /// Or: {a1 - h8}<br/>
    /// See: move.ToString()
    /// </param>
    /// <exception cref="ArgumentException">Move </exception>
    public Move(string move)
    {
        move = move.ToLower();
        var pattern = "^{(((w|b)(b|k|n|p|q|r))" + ParametersSeparator + "|)" +
                    "[a-h][1-8]" + ParametersSeparator + "[a-h][1-8]" +
                    "(" + ParametersSeparator + "(w|b)(b|k|n|p|q|r)|)" +
                    "(" + ParametersSeparator + @"((o-o)|(o-o-o)|(e\.p\.)|(=)|(=q)|(=r)|(=b)|(=n))|)" +
                    "(" + ParametersSeparator + @"(\+|#)|)}$";

        if (!Regex.IsMatch(move, pattern))
            throw new ArgumentException("Move should match pattern: " + pattern);

        var args = move[1..^1].Split(new string[] { ParametersSeparator }, StringSplitOptions.None);


        if (Regex.IsMatch(args[0], "(w|b)(b|k|n|p|q|r)") && args.Length > 2)
        {
            Piece = new Piece(args[0]);
            OriginalPosition = new Position(args[1]);
            NewPosition = new Position(args[2]);
            SetArgsFrom(3);
        }
        else
        {
            OriginalPosition = new Position(args[0]);
            NewPosition = new Position(args[1]);
            SetArgsFrom(2);
        }

        void SetArgsFrom(int index)
        {
            for (int i = index; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case var e when Regex.IsMatch(e, "(w|b)(b|k|n|p|q|r)"):
                        CapturedPiece = new Piece(e);
                        break;
                    case var e when Regex.IsMatch(e, @"(0-0)|(0-0-0)|(e\.p\.)|(=)|(=q)|(=r)|(=b)|(=n)"):
                        Parameter = IMoveParameter.FromString(e);
                        break;
                    case var e when Regex.IsMatch(e, @"(\+)"):
                        IsCheck = true;
                        break;
                    case var e when Regex.IsMatch(e, @"(#)"):
                        IsCheck = true; IsMate = true;
                        break;
                }
            }
        }
    }

    /// <summary>
    /// Needed to Generate move from SAN in ChessConvertions
    /// </summary>
    internal Move()
    {
        OriginalPosition = new();
        NewPosition = new();
    }

    /// <summary>
    /// Long move notation as: <br/>
    /// {wr - a1 - h8 - bq - e.p. - +}<br/>
    /// Or: {a1 - h8}
    /// </summary>
    public override string ToString()
    {
        return "{" +
                    (Piece is null ? "" : Piece + ParametersSeparator) +
                    OriginalPosition + ParametersSeparator + NewPosition + // Permanent
                    (CapturedPiece is null ? "" : ParametersSeparator + CapturedPiece) +
                    (Parameter is null ? "" : ParametersSeparator + Parameter.ShortStr) +
                    (!IsMate ? (IsCheck ? ParametersSeparator + "+" : "") : ParametersSeparator + "#")
                + "}";
    }
}