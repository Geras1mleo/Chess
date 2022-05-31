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
    /// Tries to load 
    /// ChessBoard from Forsyth-Edwards Notation<br/>
    /// ex.:<br/>
    /// rnbqkbnr/pppppppp/8/8/4P3/8/PPPP1PPP/RNBQKBNR b KQkq e3 0 1
    /// </summary>
    /// <param name="fen">FEN string to load</param>
    /// <param name="board">Result with loaded board</param>
    /// <returns>Whether load is succeeded</returns>
    public static bool TryLoadFromFen(string fen, [NotNullWhen(true)] out ChessBoard? board)
    {
        var (succeeded, _) = FenBoardBuilder.TryLoad(fen, out var builder);

        if (!succeeded)
        {
            board = null;
            return false;
        }

        board = BuildBoardFromFen(builder);

        return true;
    }

    /// <summary>
    /// Loads ChessBoard from Forsyth-Edwards Notation<br/>
    /// ex.:<br/>
    /// rnbqkbnr/pppppppp/8/8/4P3/8/PPPP1PPP/RNBQKBNR b KQkq e3 0 1
    /// </summary>
    /// <param name="fen">FEN string to load</param>
    /// <returns>ChessBoard with according positions</returns>
    /// <exception cref="ChessArgumentException">Given FEN string didn't match the Regex pattern</exception>
    public static ChessBoard LoadFromFen(string fen)
    {
        var (succeeded, exception) = FenBoardBuilder.TryLoad(fen, out var builder);

        if (!succeeded && exception is not null)
            throw exception;

        return BuildBoardFromFen(builder);
    }

    private static ChessBoard BuildBoardFromFen(FenBoardBuilder builder)
    {
        var board = new ChessBoard
        {
            FenBuilder = builder,
            pieces = builder.Pieces
        };

        board.headers.Add("Variant", "From Position");
        board.headers.Add("FEN", builder.ToString());

        board.HandleKingChecked();
        board.HandleEndGame();

        return board;
    }
}
