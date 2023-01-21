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
    private ChessBoard board;

    public EndGameProvider(ChessBoard board)
    {
        this.board = board;
    }

    public EndGameInfo? GetEndGameInfo()
    {
        EndGameInfo? endgameInfo = null;

        if (board.moveIndex >= 0
         && board.executedMoves[board.moveIndex].IsMate)
        {
            if (board.executedMoves[board.moveIndex].IsCheck)
                endgameInfo = new EndGameInfo(EndgameType.Checkmate, board.Turn.OppositeColor());
            else
                endgameInfo = new EndGameInfo(EndgameType.Stalemate, null);
        }
        else if (board.LoadedFromFen)
        {
            var whiteHasMoves = ChessBoard.PlayerHasMoves(PieceColor.White, board);
            var blackHasMoves = ChessBoard.PlayerHasMoves(PieceColor.Black, board);

            if (!whiteHasMoves && board.WhiteKingChecked)
                endgameInfo = new EndGameInfo(EndgameType.Checkmate, PieceColor.Black);

            else if (!blackHasMoves && board.BlackKingChecked)
                endgameInfo = new EndGameInfo(EndgameType.Checkmate, PieceColor.White);

            else if (!whiteHasMoves || !blackHasMoves)
                endgameInfo = new EndGameInfo(EndgameType.Stalemate, null);
        }

        if (endgameInfo is null)
        {
            endgameInfo = ResolveDrawRules(board.AutoEndgameRules);
        }

        return endgameInfo;
    }

    private EndGameInfo? ResolveDrawRules(AutoEndgameRules autoEndgameRules)
    {
        EndGameInfo? endgameInfo = null;

        var rules = new List<EndGameRule>();

        if ((autoEndgameRules & AutoEndgameRules.InsufficientMaterial) == AutoEndgameRules.InsufficientMaterial)
            rules.Add(new InsufficientMaterialRule(board));

        if ((autoEndgameRules & AutoEndgameRules.Repetition) == AutoEndgameRules.Repetition)
            rules.Add(new RepetitionRule(board));

        if ((autoEndgameRules & AutoEndgameRules.FiftyMoveRule) == AutoEndgameRules.FiftyMoveRule)
            rules.Add(new FiftyMoveRule(board));

        for (int i = 0; i < rules.Count && endgameInfo is null; i++)
        {
            if (rules[i].IsEndGame())
            {
                endgameInfo = new EndGameInfo(rules[i].Type, null);
            }
        }

        return endgameInfo;
    }
}

