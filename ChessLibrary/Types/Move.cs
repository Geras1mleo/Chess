// *****************************************************
// *                                                   *
// * O Lord, Thank you for your goodness in our lives. *
// *     Please bless this code to our compilers.      *
// *                     Amen.                         *
// *                                                   *
// *****************************************************
//                                    Made by Geras1mleo

namespace Chess;

/// <summary>
/// Move on chess board
/// </summary>
public class Move
{
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
    /// Initializes new Move object by given positions
    /// </summary>
    public Move(Position originalPosition, Position newPosition)
    {
        OriginalPosition = originalPosition;
        NewPosition = newPosition;
    }

    /// <summary>
    /// Initializes new Move from long move notation
    /// </summary>
    /// <param name="move">
    /// Move as long string<br/>
    /// ex.:{wr - a1 - h8 - bq - e.p. - +}<br/>
    /// Or: {a1 - h8}<br/>
    /// See: move.ToString()
    /// </param>
    /// <exception cref="ChessArgumentException">Move didn't match regex pattern</exception>
    public Move(string move)
    {
        move = move.ToLower();

        var matches = Regexes.RegexMove.Matches(move.ToLower());

        if (matches.Count < 1)
            throw new ChessArgumentException(null, "Move should match pattern: " + Regexes.MovePattern);

        foreach (var group in matches[0].Groups.Values)
        {
            if (!group.Success) continue;

            switch (group.Name)
            {
                case "2":
                    Piece = new(group.Value);
                    break;
                case "3":
                    OriginalPosition = new(group.Value);
                    break;
                case "4":
                    NewPosition = new(group.Value);
                    break;
                case "6":
                    CapturedPiece = new(group.Value);
                    break;
                case "8":
                    Parameter = IMoveParameter.FromString(group.Value);
                    break;
                case "10":
                    if (group.Value == "+")
                        IsCheck = true;
                    else if (group.Value == "#")
                    {
                        IsCheck = true;
                        IsMate = true;
                    }
                    else if (group.Value == "$")
                        IsMate = true;
                    break;
            }
        }
    }

    /// <summary>
    /// Initializes new Move object by given positions
    /// </summary>
    public Move(string originalPos, string newPos)
    {
        OriginalPosition = new(originalPos);
        NewPosition = new(newPos);
    }

    internal Move(Move source, PromotionType promotion)
    {
        Piece = source.Piece;
        OriginalPosition = source.OriginalPosition;
        NewPosition = source.NewPosition;
        CapturedPiece = source.CapturedPiece;
        Parameter = new MovePromotion(promotion);
        IsCheck = source.IsCheck;
        IsMate = source.IsMate;
        San = source.San;
    }

    internal Move(Move source)
    {
        if (source.Piece is not null)
            Piece = new Piece(source.Piece);

        OriginalPosition = new Position(source.OriginalPosition.X, source.OriginalPosition.Y);
        NewPosition = new Position(source.NewPosition.X, source.NewPosition.Y);

        if (source.CapturedPiece is not null)
            CapturedPiece = new Piece(source.CapturedPiece);

        if (source.Parameter is not null)
            Parameter = IMoveParameter.FromString(source.Parameter.ShortStr);

        IsCheck = source.IsCheck;
        IsMate = source.IsMate;
        San = source.San;
    }

    /// <summary>
    /// Needed to Generate move from SAN in ChessConversions
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
        StringBuilder builder = new();

        builder.Append('{');

        if (Piece is not null)
            builder.Append(Piece + " - ");

        builder.Append(OriginalPosition + " - " + NewPosition);

        if (CapturedPiece is not null)
            builder.Append(" - " + CapturedPiece);

        if (Parameter is not null)
            builder.Append(" - " + Parameter.ShortStr);

        if (IsCheck)
        {
            if (IsMate)
                builder.Append(" - #");
            else
                builder.Append(" - +");
        }
        else if (IsMate)
            builder.Append(" - $");

        builder.Append('}');

        return builder.ToString();
    }
}