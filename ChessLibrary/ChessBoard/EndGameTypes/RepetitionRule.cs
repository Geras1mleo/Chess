// *****************************************************
// *                                                   *
// * O Lord, Thank you for your goodness in our lives. *
// *     Please bless this code to our compilers.      *
// *                     Amen.                         *
// *                                                   *
// *****************************************************
//                                    Made by Geras1mleo

namespace Chess;

internal class RepetitionRule : EndGameRule
{
    public RepetitionRule(ChessBoard board) : base(board) { }

    internal override EndgameType Type => EndgameType.Repetition;

    internal override bool IsEndGame()
    {
        return false;
    }
}

