#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Chess;

public interface IMoveParameter
{
    string ShortStr { get; }
    string LongStr { get; }

    /// <summary>
    /// Special Move dropping implementation
    /// </summary>
    internal void ExecuteWithParameter(Move move, ChessBoard board);

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
            _ => throw new ArgumentException("Parameter not found!", nameof(parameter)),
        };
    }
}

internal class MoveCastle : IMoveParameter
{
    public CastleType CastleType { get; }

    public string ShortStr
    {
        get
        {
            return CastleType switch
            {
                CastleType.King => "O-O",
                CastleType.Queen => "O-O-O",
                _ => throw new ArgumentException(nameof(ShortStr))
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
                _ => throw new ArgumentException(nameof(LongStr))
            };
        }
    }

    public void ExecuteWithParameter(Move move, ChessBoard board)
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
                throw new ArgumentException(nameof(CastleType));
        }

    }

    public MoveCastle(CastleType castleType)
    {
        CastleType = castleType;
    }
}

internal class MoveEnPassant : IMoveParameter
{
    public Position CapturedPawnPosition { get; set; }

    public string ShortStr => "e.p.";

    public string LongStr => "En Passant";

    public void ExecuteWithParameter(Move move, ChessBoard board)
    {
        ChessBoard.DropPiece(move, board);

        if (CapturedPawnPosition.HasValue)
            board.pieces[CapturedPawnPosition.Y, CapturedPawnPosition.X] = null;
    }
}

internal class MovePromotion : IMoveParameter
{
    public PromotionType PromotionType { get; set; } = PromotionType.Default;

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
                _ => throw new ArgumentException(nameof(ShortStr))
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
                _ => throw new ArgumentException(nameof(LongStr))
            };
        }
    }

    public void ExecuteWithParameter(Move move, ChessBoard board)
    {
        ChessBoard.DropPiece(move, board);

        move.Piece.Type = PromotionType switch
        {
            PromotionType.ToQueen or PromotionType.Default => PieceType.Queen,
            PromotionType.ToRook => PieceType.Rook,
            PromotionType.ToBishop => PieceType.Bishop,
            PromotionType.ToKnight => PieceType.Knight,
            _ => throw new ArgumentException(nameof(PromotionType)),
        };
    }

    public MovePromotion(PromotionType promotionType)
    {
        PromotionType = promotionType;
    }
}

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member