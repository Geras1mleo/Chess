// *****************************************************
// *                                                   *
// * O Lord, Thank you for your goodness in our lives. *
// *     Please bless this code to our compilers.      *
// *                     Amen.                         *
// *                                                   *
// *****************************************************
//                                    Made by Geras1mleo

namespace Chess;

internal class InsufficientMaterialRule : EndGameRule
{
    internal override EndgameType Type => EndgameType.InsufficientMaterial;

    public InsufficientMaterialRule(ChessBoard board) : base(board) { }

    internal override bool IsEndGame()
    {
        var pieces = board.pieces.PiecesList();

        bool isEndGame = IsFirstLevelDraw(pieces)
                      || IsSecondLevelDraw(pieces)
                      || IsThirdLevelDraw(pieces);

        return isEndGame;
    }

    private bool IsFirstLevelDraw(List<Piece> pieces)
    {
        return pieces.Where(p => p.Type != PieceType.King).Count() < 1;
    }

    private bool IsSecondLevelDraw(List<Piece> pieces)
    {
        var isDraw = false;
        var hasStrongPieces = pieces.Where(p => p.Type == PieceType.Pawn
                                             || p.Type == PieceType.Queen
                                             || p.Type == PieceType.Rook).Count() > 0;

        // The only piece remaining will be Bishop or Knight, what results in draw
        if (!hasStrongPieces && pieces.Where(p => p.Type != PieceType.King).Count() == 1)
            isDraw = true;

        return isDraw;
    }

    private bool IsThirdLevelDraw(List<Piece> pieces)
    {
        var isDraw = false;

        if (pieces.Count == 4
         && pieces.Where(p => p.Type == PieceType.Bishop).Count() == 2
         && pieces.Where(p => p.Type == PieceType.King).Count() == 2)
        {
            var firstBishop = pieces.First(p => p.Type == PieceType.Bishop);
            var lastBishop = pieces.Last(p => p.Type == PieceType.Bishop);

            isDraw = firstBishop.Color != lastBishop.Color && BishopsAreOnSameColor();
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

