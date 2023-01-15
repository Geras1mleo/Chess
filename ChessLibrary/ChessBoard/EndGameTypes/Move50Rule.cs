// *****************************************************
// *                                                   *
// * O Lord, Thank you for your goodness in our lives. *
// *     Please bless this code to our compilers.      *
// *                     Amen.                         *
// *                                                   *
// *****************************************************
//                                    Made by Geras1mleo

namespace Chess;

internal class Move50Rule : EndGameRule
{
    public Move50Rule(ChessBoard board) : base(board) { }

    internal override EndgameType Type => EndgameType.Move50Rule;

    internal override bool IsEndGame()
    {
        return false;
    }
}

