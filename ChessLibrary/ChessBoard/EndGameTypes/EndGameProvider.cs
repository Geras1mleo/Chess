// *****************************************************
// *                                                   *
// * O Lord, Thank you for your goodness in our lives. *
// *     Please bless this code to our compilers.      *
// *                     Amen.                         *
// *                                                   *
// *****************************************************
//                                    Made by Geras1mleo

namespace Chess;

internal class EndGameProvider
{
    private readonly ChessBoard board;
    private readonly List<EndGameRule> rules = new();

    public EndGameProvider(ChessBoard board)
    {
        this.board = board;
    }

    public EndGameInfo? GetEndGameInfo()
    {
        EndGameInfo? endgameInfo = null;

        if (board.moveIndex >= 0 && board.executedMoves[board.moveIndex].IsMate)
        {
            if (board.executedMoves[board.moveIndex].IsCheck)
                endgameInfo = new EndGameInfo(EndgameType.Checkmate, board.Turn.OppositeColor());
            else
                endgameInfo = new EndGameInfo(EndgameType.Stalemate, null);
        }
        else if (board.LoadedFromFen)
        {
            // TODO need to check both???
            var whiteHasMoves = ChessBoard.PlayerHasMoves(PieceColor.White, board);
            var blackHasMoves = ChessBoard.PlayerHasMoves(PieceColor.Black, board);

            if (!whiteHasMoves && board.WhiteKingChecked)
                endgameInfo = new EndGameInfo(EndgameType.Checkmate, PieceColor.Black);

            else if (!blackHasMoves && board.BlackKingChecked)
                endgameInfo = new EndGameInfo(EndgameType.Checkmate, PieceColor.White);

            else if ((!whiteHasMoves && board.Turn == PieceColor.White) || (!blackHasMoves && board.Turn == PieceColor.Black))
                endgameInfo = new EndGameInfo(EndgameType.Stalemate, null);
        }

        if (endgameInfo is null)
        {
            endgameInfo = ResolveDrawRules();
        }

        return endgameInfo;
    }

    private EndGameInfo? ResolveDrawRules()
    {
        EndGameInfo? endgameInfo = null;

        for (int i = 0; i < rules.Count && endgameInfo is null; i++)
        {
            if (rules[i].IsEndGame())
            {
                endgameInfo = new EndGameInfo(rules[i].Type, null);
            }
        }

        return endgameInfo;
    }

    public void UpdateRules()
    {
        rules.Clear();

        if ((board.AutoEndgameRules & AutoEndgameRules.InsufficientMaterial) == AutoEndgameRules.InsufficientMaterial)
            rules.Add(new InsufficientMaterialRule(board));

        if ((board.AutoEndgameRules & AutoEndgameRules.Repetition) == AutoEndgameRules.Repetition)
            rules.Add(new RepetitionRule(board));

        if ((board.AutoEndgameRules & AutoEndgameRules.FiftyMoveRule) == AutoEndgameRules.FiftyMoveRule)
            rules.Add(new FiftyMoveRule(board));
    }
}