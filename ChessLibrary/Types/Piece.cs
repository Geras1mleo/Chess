namespace Chess;

/// <summary>
/// Chess Piece
/// </summary>
public class Piece
{
    /// <summary>
    /// Color of piece White/Black
    /// </summary>
    public PieceColor Color { get; }

    /// <summary>
    /// Type of piece
    /// See: PieceType SmartEnum
    /// </summary>
    public PieceType Type { get; internal set; }

    /// <summary>
    /// Initializes new Piece by given color and type
    /// </summary>
    public Piece(PieceColor color, PieceType type)
    {
        Color = color;
        Type = type;
    }

    /// <summary>
    /// Initializes new Piece by given color and type as:<br/>
    /// "wp" => White Pawn<br/>
    /// "br" => Black Rook<br/>
    /// See: piece.ToString()<br/>
    /// </summary>
    public Piece(string piece)
    {
        if (!Regex.IsMatch(piece, "^[wb][bknpqr]$"))
            throw new ArgumentException("Piece should match pattern: ^[wb][bknpqr]$");

        Color = PieceColor.FromChar(piece[0]);
        Type = PieceType.FromChar(piece[1]);
    }

    /// <summary>
    /// Initializes new Piece by given color and type as FEN:<br/>
    /// 'Q' => White Queen<br/>
    /// 'q' => Black Queen<br/>
    /// See: piece.ToFENChar()<br/>
    /// </summary>
    public Piece(char fenChar)
    {
        if (!Regex.IsMatch(fenChar.ToString(), "^([bknpqr]|[BKNPQR])$"))
            throw new ArgumentException("FEN piece should match pattern: ^([bknpqr]|[BKNPQR])$");

        Type = PieceType.FromChar(fenChar);
        Color = char.IsLower(fenChar) ? PieceColor.Black : PieceColor.White;
    }

    /// <summary>
    /// Piece as:<br/>
    /// "wp" => White Pawn<br/>
    /// "br" => Black Rook<br/>
    /// See: new Piece(string piece)<br/>
    /// </summary>
    public override string ToString() => $"{Color.AsChar}{Type.AsChar}";

    /// <summary>
    /// Piece as FEN char<br/>
    /// Uppercase => White piece<br/>
    /// Lowercase => Black piece<br/>
    /// See: new Piece(char fenChar)<br/>
    /// </summary>
    public char ToFenChar()
    {
        return Color switch
        {
            var e when e == PieceColor.White => char.ToUpper(Type.AsChar),
            var e when e == PieceColor.Black => char.ToLower(Type.AsChar),
            var _ => throw new ArgumentException("GetAsFENChar"),
        };
    }
}
