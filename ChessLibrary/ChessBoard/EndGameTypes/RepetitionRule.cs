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
/// https://www.chessprogramming.org/Repetitions
/// </summary>
internal class RepetitionRule : EndGameRule
{
    private const int MINIMUM_MOVES_COUNT = 8; // at least 8 moves required to get threefold repetition

    public RepetitionRule(ChessBoard board) : base(board) { }

    internal override EndgameType Type => EndgameType.Repetition;

    internal override bool IsEndGame()
    {
        bool isRepetition = false;
        var movesCount = board.MoveIndex + 1;

        if (movesCount >= MINIMUM_MOVES_COUNT
        && board.LastIrreversibleMoveIndex <= board.MoveIndex - MINIMUM_MOVES_COUNT) // If last 8 moves were reversible
        {
            var currentIndex = board.MoveIndex;

            HashSet<ChessBoard> piecesPositions = new HashSet<ChessBoard>(new ChessBoardComparer());
            piecesPositions.Add(new ChessBoard(board.pieces, board.DisplayedMoves) { FenBuilder = board.FenBuilder, moveIndex = board.MoveIndex });

            board.MoveIndex -= MINIMUM_MOVES_COUNT;

            piecesPositions.Add(new ChessBoard(board.pieces, board.DisplayedMoves) { FenBuilder = board.FenBuilder, moveIndex = board.MoveIndex });

            if (piecesPositions.Count == 1)
            {
                board.MoveIndex += (MINIMUM_MOVES_COUNT / 2);

                piecesPositions.Add(new ChessBoard(board.pieces, board.DisplayedMoves) { FenBuilder = board.FenBuilder, moveIndex = board.MoveIndex });
            }

            board.MoveIndex = currentIndex; // Setting back to original positions

            isRepetition = piecesPositions.Count == 1;
        }

        return isRepetition;
    }
}

internal class ChessBoardComparer : IEqualityComparer<ChessBoard>
{
    public bool Equals(ChessBoard? x, ChessBoard? y)
    {
        bool isEqual = false;

        if (x is null && y is null)
        {
            isEqual = true;
        }
        else if (x is not null && y is not null)
        {
            isEqual = true;

            for (int i = 0; i < x.pieces.GetLength(0) && isEqual; i++)
            {
                for (int j = 0; j < x.pieces.GetLength(1) && isEqual; j++)
                {
                    if (x.pieces[i, j] is null != y.pieces[i, j] is null)
                        isEqual = false;

                    if (x.pieces[i, j] is not null && y.pieces[i, j] is not null)
                    {
                        isEqual = x.pieces[i, j].Color == y.pieces[i, j].Color && x.pieces[i, j].Type == y.pieces[i, j].Type;
                    }
                }
            }

            isEqual &= ChessBoard.HasRightToCastle(PieceColor.White, CastleType.King, x) == ChessBoard.HasRightToCastle(PieceColor.White, CastleType.King, y);
            isEqual &= ChessBoard.HasRightToCastle(PieceColor.White, CastleType.Queen, x) == ChessBoard.HasRightToCastle(PieceColor.White, CastleType.Queen, y);
            isEqual &= ChessBoard.HasRightToCastle(PieceColor.Black, CastleType.King, x) == ChessBoard.HasRightToCastle(PieceColor.Black, CastleType.King, y);
            isEqual &= ChessBoard.HasRightToCastle(PieceColor.Black, CastleType.Queen, x) == ChessBoard.HasRightToCastle(PieceColor.Black, CastleType.Queen, y);

            isEqual &= ChessBoard.LastMoveEnPassantPosition(x) == ChessBoard.LastMoveEnPassantPosition(y);
        }
        else
        {
            isEqual = false;
        }

        return isEqual;
    }

    public int GetHashCode([DisallowNull] ChessBoard obj)
    {
        return 0;
    }
}
