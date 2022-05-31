// *****************************************************
// *                                                   *
// * O Lord, Thank you for your goodness in our lives. *
// *     Please bless this code to our compilers.      *
// *                     Amen.                         *
// *                                                   *
// *****************************************************
//                                    Made by Geras1mleo

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Chess;

/// <summary>
/// Special move Parameter<br/>
/// Castle<br/>
/// EnPassant<br/>
/// Pawn Promotion
/// </summary>
public interface IMoveParameter
{
    /// <summary>
    /// Parameter as short(SAN/LAN) string
    /// </summary>
    public string ShortStr { get; }

    /// <summary>
    /// Parameter as long(Comment) string
    /// </summary>
    public string LongStr { get; }

    /// <summary>
    /// Special Move dropping implementation
    /// </summary>
    internal void Execute(Move move, ChessBoard board);

    internal void Undo(Move move, ChessBoard board);

    internal static IMoveParameter FromString(string parameter)
    {
        return parameter.ToLower() switch
        {
            "o-o" => new MoveCastle(CastleType.King),
            "o-o-o" => new MoveCastle(CastleType.Queen),
            "e.p." => new MoveEnPassant(),
            "=" => new MovePromotion(PromotionType.Default),
            "=q" => new MovePromotion(PromotionType.ToQueen),
            "=r" => new MovePromotion(PromotionType.ToRook),
            "=b" => new MovePromotion(PromotionType.ToBishop),
            "=n" => new MovePromotion(PromotionType.ToKnight),
            _ => throw new ChessArgumentException(null, "Parameter not recognized..."),
        };
    }
}

public class MoveCastle : IMoveParameter
{
    public CastleType CastleType { get; private set; }

    public string ShortStr
    {
        get
        {
            return CastleType switch
            {
                CastleType.King => "O-O",
                CastleType.Queen => "O-O-O",
                _ => throw new ChessArgumentException(null, nameof(CastleType), nameof(MoveCastle.ShortStr))
            };
        }
    }

    public string LongStr
    {
        get
        {
            return CastleType switch
            {
                CastleType.King => "King Side Castle",
                CastleType.Queen => "Queen Side Castle",
                _ => throw new ChessArgumentException(null, nameof(CastleType), nameof(MoveCastle.LongStr))
            };
        }
    }

    void IMoveParameter.Execute(Move move, ChessBoard board)
    {
        var y = move.NewPosition.Y;
        switch (CastleType)
        {
            case CastleType.King:
                board.pieces[y, 6] = new Piece(move.Piece.Color, PieceType.King);
                board.pieces[y, 5] = new Piece(move.Piece.Color, PieceType.Rook);
                board.pieces[y, 4] = null;
                board.pieces[y, 7] = null;
                break;
            case CastleType.Queen:
                board.pieces[y, 2] = new Piece(move.Piece.Color, PieceType.King);
                board.pieces[y, 3] = new Piece(move.Piece.Color, PieceType.Rook);
                board.pieces[y, 4] = null;
                board.pieces[y, 0] = null;
                break;
            default:
                throw new ChessArgumentException(board, nameof(CastleType), nameof(IMoveParameter.Execute));
        }
    }

    void IMoveParameter.Undo(Move move, ChessBoard board)
    {
        var y = move.NewPosition.Y;
        switch (CastleType)
        {
            case CastleType.King:
                board.pieces[y, 4] = new Piece(move.Piece.Color, PieceType.King);
                board.pieces[y, 7] = new Piece(move.Piece.Color, PieceType.Rook);
                board.pieces[y, 6] = null;
                board.pieces[y, 5] = null;
                break;
            case CastleType.Queen:
                board.pieces[y, 4] = new Piece(move.Piece.Color, PieceType.King);
                board.pieces[y, 0] = new Piece(move.Piece.Color, PieceType.Rook);
                board.pieces[y, 2] = null;
                board.pieces[y, 3] = null;
                break;
            default:
                throw new ChessArgumentException(board, nameof(CastleType), nameof(IMoveParameter.Undo));
        }
    }

    internal MoveCastle(CastleType castleType)
    {
        CastleType = castleType;
    }
}

public class MoveEnPassant : IMoveParameter
{
    public Position CapturedPawnPosition { get; internal set; }

    public string ShortStr => "e.p.";

    public string LongStr => "En Passant";

    void IMoveParameter.Execute(Move move, ChessBoard board)
    {
        ChessBoard.DropPiece(move, board);

        if (CapturedPawnPosition.HasValue)
            board.pieces[CapturedPawnPosition.Y, CapturedPawnPosition.X] = null;
        else
            throw new ChessArgumentException(board, nameof(CapturedPawnPosition), nameof(IMoveParameter.Execute));
    }

    void IMoveParameter.Undo(Move move, ChessBoard board)
    {
        ChessBoard.RestorePiece(move, board);

        board.pieces[move.NewPosition.Y, move.NewPosition.X] = null;
        board.pieces[CapturedPawnPosition.Y, CapturedPawnPosition.X] = move.CapturedPiece;
    }
}

public class MovePromotion : IMoveParameter
{
    public PromotionType PromotionType { get; internal set; } = PromotionType.Default;

    public string ShortStr
    {
        get
        {
            return PromotionType switch
            {
                PromotionType.Default => "=Q",
                PromotionType.ToQueen => "=Q",
                PromotionType.ToRook => "=R",
                PromotionType.ToBishop => "=B",
                PromotionType.ToKnight => "=N",
                _ => throw new ChessArgumentException(null, nameof(PromotionType), nameof(MovePromotion.ShortStr))
            };
        }
    }

    public string LongStr
    {
        get
        {
            return PromotionType switch
            {
                PromotionType.Default => "Default Promotion",
                PromotionType.ToQueen => "Promotion To Queen",
                PromotionType.ToRook => "Promotion To Rook",
                PromotionType.ToBishop => "Promotion To Bishop",
                PromotionType.ToKnight => "Promotion To Knight",
                _ => throw new ChessArgumentException(null, nameof(PromotionType), nameof(MovePromotion.LongStr))
            };
        }
    }

    void IMoveParameter.Execute(Move move, ChessBoard board)
    {
        ChessBoard.DropPiece(move, board);

        // Making sure original type(pawn) is saved

        board.pieces[move.NewPosition.Y, move.NewPosition.X].Type = PromotionType switch
        {
            PromotionType.ToQueen or PromotionType.Default => PieceType.Queen,
            PromotionType.ToRook => PieceType.Rook,
            PromotionType.ToBishop => PieceType.Bishop,
            PromotionType.ToKnight => PieceType.Knight,
            _ => throw new ChessArgumentException(board, nameof(PromotionType), nameof(IMoveParameter.Execute)),
        };
    }

    void IMoveParameter.Undo(Move move, ChessBoard board)
    {
        ChessBoard.RestorePiece(move, board);

        board.pieces[move.OriginalPosition.Y, move.OriginalPosition.X].Type = PieceType.Pawn;
    }

    internal MovePromotion(PromotionType promotionType)
    {
        PromotionType = promotionType;
    }
}

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member