// *****************************************************
// *                                                   *
// * O Lord, Thank you for your goodness in our lives. *
// *     Please bless this code to our compilers.      *
// *                     Amen.                         *
// *                                                   *
// *****************************************************
//                                    Made by Geras1mleo

namespace Chess;

public partial class ChessBoard
{
    /// <summary>
    /// Generates FEN string representing current board
    /// </summary>
    public string ToFen()
    {
        return FenBoardBuilder.Load(this).ToString();
    }

    /// <summary>
    /// Generates PGN string representing current board
    /// </summary>
    public string ToPgn()
    {
        return PgnBuilder.BoardToPgn(this);
    }

    /// <summary>
    /// Generates ASCII string representing current board
    /// </summary>
    public string ToAscii(bool displayFull = false)
    {
        StringBuilder builder = new("   ┌────────────────────────┐\n");

        for (int i = 8 - 1; i >= 0; i--)
        {
            builder.Append(" " + (i + 1) + " │");
            for (int j = 0; j < 8; j++)
            {
                builder.Append(' ');

                if (pieces[i, j] is not null)
                    builder.Append(pieces[i, j].ToFenChar());
                else
                    builder.Append('.');

                builder.Append(' ');
            }
            builder.Append("│\n");
        }

        builder.Append("   └────────────────────────┘\n");
        builder.Append("     a  b  c  d  e  f  g  h  \n");

        if (displayFull)
        {
            builder.Append('\n');

            builder.Append("  Turn: " + Turn + '\n');

            if (CapturedWhite.Length > 0)
                builder.Append("  White Captured: " + string.Join(", ", CapturedWhite.Select(p => p.ToFenChar())) + '\n');
            if (CapturedBlack.Length > 0)
                builder.Append("  Black Captured: " + string.Join(", ", CapturedBlack.Select(p => p.ToFenChar())) + '\n');
        }

        return builder.ToString();
    }

    internal int GetHalfMovesCount()
    {
        int index = LastIrreversibleMoveIndex;

        if (LoadedFromFen && index < 0)
            return FenBuilder!.HalfMoves + moveIndex + 1;

        if (index >= 0)
            return moveIndex - index;
        else
            return moveIndex + 1;
    }

    internal int GetFullMovesCount()
    {
        var count = 0;

        if (LoadedFromFen)
            count += (FenBuilder.FullMoves * 2) + (FenBuilder.Turn == PieceColor.Black ? 1 : 0) - 2;

        return (moveIndex + count + 3) / 2;
    }
}
