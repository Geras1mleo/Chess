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
/// https://www.chessprogramming.org/Material#InsufficientMaterial
/// </summary>
internal class InsufficientMaterialRule : EndGameRule
{
    internal override EndgameType Type => EndgameType.InsufficientMaterial;

    public InsufficientMaterialRule(ChessBoard board) : base(board) { }

    internal override bool IsEndGame()
    {
        var pieces = board.pieces.PiecesList();

        return IsFirstLevelDraw(pieces)
            || IsSecondLevelDraw(pieces)
            || IsThirdLevelDraw(pieces);
    }

    private bool IsFirstLevelDraw(List<Piece> pieces)
    {
        return pieces.All(p => p.Type == PieceType.King);
    }

    private bool IsSecondLevelDraw(List<Piece> pieces)
    {
        var hasStrongPieces = pieces.Count(p => p.Type == PieceType.Pawn
                                                || p.Type == PieceType.Queen
                                                || p.Type == PieceType.Rook) > 0;

        // The only piece remaining will be Bishop or Knight, what results in draw
        return !hasStrongPieces && pieces.Count(p => p.Type != PieceType.King) == 1;;
    }

    private bool IsThirdLevelDraw(List<Piece> pieces)
    {
        var isDraw = false;

        if (pieces.Count == 4)
        {
            if (pieces.All(p => p.Type == PieceType.King || p.Type == PieceType.Bishop))
            {
                var firstPiece = pieces.First(p => p.Type == PieceType.Bishop);
                var lastPiece = pieces.Last(p => p.Type == PieceType.Bishop);

                isDraw = firstPiece.Color != lastPiece.Color && BishopsAreOnSameColor();
            }
            else if (pieces.All(p => p.Type == PieceType.King || p.Type == PieceType.Knight))
            {
                var firstPiece = pieces.First(p => p.Type == PieceType.Knight);
                var lastPiece = pieces.Last(p => p.Type == PieceType.Knight);

                isDraw = firstPiece.Color == lastPiece.Color;
            }
        }

        return isDraw;
    }

    private bool BishopsAreOnSameColor()
    {
        var bishopsCoords = new List<Position>();

        for (short i = 0; i < board.pieces.GetLength(0) && bishopsCoords.Count < 2; i++)
        {
            for (short j = 0; j < board.pieces.GetLength(1) && bishopsCoords.Count < 2; j++)
            {
                if (board.pieces[i, j]?.Type == PieceType.Bishop)
                    bishopsCoords.Add(new Position(i, j));
            }
        }

        return (bishopsCoords[0].X + bishopsCoords[1].X + bishopsCoords[0].Y + bishopsCoords[1].Y) % 2 == 0;
    }
}

