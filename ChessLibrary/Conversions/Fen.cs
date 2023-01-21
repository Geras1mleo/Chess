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
    /// <param name="autoEndgameRules">Automatic draw/endgame rules that will be used to check for endgame</param>
    /// <returns>Whether load is succeeded</returns>
    public static bool TryLoadFromFen(string fen, [NotNullWhen(true)] out ChessBoard? board, AutoEndgameRules autoEndgameRules = AutoEndgameRules.None)
    {
        var (succeeded, _) = FenBoardBuilder.TryLoad(fen, out var builder);

        if (!succeeded)
        {
            board = null;
            return false;
        }

        board = BuildBoardFromFen(builder, autoEndgameRules);

        return true;
    }

    /// <summary>
    /// Loads ChessBoard from Forsyth-Edwards Notation<br/>
    /// ex.:<br/>
    /// rnbqkbnr/pppppppp/8/8/4P3/8/PPPP1PPP/RNBQKBNR b KQkq e3 0 1
    /// </summary>
    /// <param name="fen">FEN string to load</param>
    /// <param name="autoEndgameRules">Automatic draw/endgame rules that will be used to check for endgame</param>
    /// <returns>ChessBoard with according positions</returns>
    /// <exception cref="ChessArgumentException">Given FEN string didn't match the Regex pattern</exception>
    public static ChessBoard LoadFromFen(string fen, AutoEndgameRules autoEndgameRules = AutoEndgameRules.None)
    {
        var (succeeded, exception) = FenBoardBuilder.TryLoad(fen, out var builder);

        if (!succeeded && exception is not null)
            throw exception;

        return BuildBoardFromFen(builder, autoEndgameRules);
    }

    private static ChessBoard BuildBoardFromFen(FenBoardBuilder builder, AutoEndgameRules autoEndgameRules)
    {
        var board = new ChessBoard
        {
            FenBuilder = builder,
            pieces = builder.Pieces,
            AutoEndgameRules = autoEndgameRules
        };

        board.headers.Add("Variant", "From Position");
        board.headers.Add("FEN", builder.ToString());

        board.HandleKingChecked();
        board.HandleEndGame();

        return board;
    }
}
