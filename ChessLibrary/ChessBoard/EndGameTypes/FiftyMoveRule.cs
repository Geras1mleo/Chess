// *****************************************************
// *                                                   *
// * O Lord, Thank you for your goodness in our lives. *
// *     Please bless this code to our compilers.      *
// *                     Amen.                         *
// *                                                   *
// *****************************************************
//                                    Made by Geras1mleo

namespace Chess;

internal class FiftyMoveRule : EndGameRule
{
    public FiftyMoveRule(ChessBoard board) : base(board) { }

    internal override EndgameType Type => EndgameType.FiftyMoveRule;

    internal override bool IsEndGame()
    {
        return false;
    }
}

