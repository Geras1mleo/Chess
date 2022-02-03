#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
namespace Chess;

/// <summary>
/// Smart enum for Piece color in chess game
/// </summary>
public class PieceColor : SmartEnum<PieceColor>
{
    public static readonly PieceColor White = new("White", 1, 'w');
    public static readonly PieceColor Black = new("Black", 2, 'b');

    /// <summary>
    /// White => 'w'<br/>
    /// Black => 'b'<br/>
    /// </summary>
    public char AsChar { get; }

    /// <returns>
    /// Opposite color <br/>
    /// White => Black;<br/>
    /// Black => White;<br/>
    /// </returns>
    public PieceColor OppositeColor()
    {
        return Value switch
        {
            1 => Black,
            2 => White,
            _ => throw new ArgumentException("OppositeColor"),
        };
    }

    private PieceColor(string name, int value, char asChar) : base(name, value)
    {
        AsChar = asChar;
    }

    /// <returns>
    /// PieceColor object from char<br/>
    /// 'w' => White<br/>
    /// 'b' => Black<br/>
    /// </returns>
    public static PieceColor FromChar(char color)
    {
        return char.ToLower(color) switch
        {
            'w' => White,
            'b' => Black,
            _ => throw new ArgumentException("PieceColor.FromChar"),
        };
    }
}

/// <summary>
/// Smart enum for Piece type in chess game
/// </summary>
public class PieceType : SmartEnum<PieceType>
{
    public static readonly PieceType Pawn = new("Pawn", 1, 'p');
    public static readonly PieceType Rook = new("Rook", 2, 'r');
    public static readonly PieceType Knight = new("Knight", 3, 'n');
    public static readonly PieceType Bishop = new("Bishop", 4, 'b');
    public static readonly PieceType Queen = new("Queen", 5, 'q');
    public static readonly PieceType King = new("King", 6, 'k');

    /// <summary>
    /// Pawn => 'p'<br/>
    /// Rook => 'r'<br/>
    /// Knight => 'n'<br/>
    /// Bishop => 'b'<br/>
    /// Queen => 'q'<br/>
    /// King => 'k'<br/>
    /// </summary>
    public char AsChar { get; }

    private PieceType(string name, int value, char asChar) : base(name, value)
    {
        AsChar = asChar;
    }

    /// <returns>
    /// PieceType object from char<br/>
    /// 'p' => Pawn<br/>
    /// 'r' => Rook<br/>
    /// 'n' => Knight<br/>
    /// 'b' => Bishop<br/>
    /// 'q' => Queen<br/>
    /// 'k' => King<br/>
    /// </returns>
    public static PieceType FromChar(char type)
    {
        return char.ToLower(type) switch
        {
            'p' => Pawn,
            'r' => Rook,
            'n' => Knight,
            'b' => Bishop,
            'q' => Queen,
            'k' => King,
            _ => throw new ArgumentException("PieceType.FromChar"),
        };
    }
}

/// <summary>
/// Smart enum for Move parameter in chess game
/// </summary>
public class MoveParameter : SmartEnum<MoveParameter>
{
    public static readonly MoveParameter CastleKing = new("CastleKing", 2, "O-O");
    public static readonly MoveParameter CastleQueen = new("CastleQueen", 3, "O-O-O");
    public static readonly MoveParameter EnPassant = new("EnPassant", 4, "e.p.");
    public static readonly MoveParameter PawnPromotion = new("PawnPromotion", 5, "=");
    public static readonly MoveParameter PromotionToQueen = new("PromotionToQueen", 6, "=q");
    public static readonly MoveParameter PromotionToRook = new("PromotionToRook", 6, "=r");
    public static readonly MoveParameter PromotionToBishop = new("PromotionToBishop", 6, "=b");
    public static readonly MoveParameter PromotionToKnight = new("PromotionToKnight", 6, "=n");

    /// <summary>
    /// CastleKing => "O-O"<br/>
    /// CastleQueen => "O-O-O"<br/>
    /// EnPassant => "e.p."<br/>
    /// PawnPromotion => "="<br/>
    /// PromotionToQueen => "=q"<br/>
    /// PromotionToRook => "=r"<br/>
    /// PromotionToBishop => "=b"<br/>
    /// PromotionToKnight => "=n"<br/>
    /// </summary>
    public string AsShortString { get; }

    private MoveParameter(string name, int value, string asShortString) : base(name, value)
    {
        AsShortString = asShortString;
    }

    /// <returns>
    /// MoveParameter object from short string<br/>
    /// "0-0" => CastleKing<br/>
    /// "0-0-0" => CastleQueen<br/>
    /// "e.p." => EnPassant<br/>
    /// "=" => PawnPromotion<br/>
    /// "=q" => PromotionToQueen<br/>
    /// "=r" => PromotionToRook<br/>
    /// "=b" => PromotionToBishop<br/>
    /// "=n" => PromotionToKnight<br/>
    /// </returns>
    public static MoveParameter FromString(string parameter)
    {
        return parameter.ToLower() switch
        {
            "o-o" => CastleKing,
            "o-o-o" => CastleQueen,
            "e.p." => EnPassant,
            "=" => PawnPromotion,
            "=q" => PromotionToQueen,
            "=r" => PromotionToRook,
            "=b" => PromotionToBishop,
            "=n" => PromotionToKnight,
            _ => throw new ArgumentException("Parameter.FromString"),
        };
    }
}

/// <summary>
/// Endgame type enum for chess game
/// </summary>
public enum EndgameType : byte
{
    Checkmate,
    Stalemate,
    Resigned,
    Draw,
}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member