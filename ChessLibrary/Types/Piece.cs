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
        if (!Regexes.RegexPiece.IsMatch(piece))
            throw new ChessArgumentException(null, "Piece should match pattern: " + Regexes.PiecePattern);

        Color = PieceColor.FromChar(piece[0]);
        Type = PieceType.FromChar(piece[1]);
    }

    internal Piece(Piece piece)
    {
        Color = PieceColor.FromValue(piece.Color.Value);
        Type = PieceType.FromValue(piece.Type.Value);
    }

    /// <summary>
    /// Initializes new Piece by given color and type as FEN:<br/>
    /// 'Q' => White Queen<br/>
    /// 'q' => Black Queen<br/>
    /// See: piece.ToFenChar()<br/>
    /// </summary>
    public Piece(char fenChar)
    {
        if (!Regexes.RegexFenPiece.IsMatch(fenChar.ToString()))
            throw new ChessArgumentException(null, "FEN piece character should match pattern: " + Regexes.FenPiecePattern);

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
            var _ => throw new ChessArgumentException(null, nameof(Color), nameof(Piece.ToFenChar)),
        };
    }
}
