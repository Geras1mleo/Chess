// *****************************************************
// *                                                   *
// * O Lord, Thank you for your goodness in our lives. *
// *     Please bless this code to our compilers.      *
// *                     Amen.                         *
// *                                                   *
// *****************************************************
//                                    Made by Geras1mleo

namespace Chess;

// https://en.wikipedia.org/wiki/Draw_(chess)
internal abstract class EndGameRule
{
    protected ChessBoard board;

    internal abstract EndgameType Type { get; }

    internal EndGameRule(ChessBoard board)
    {
        this.board = board;
    }

    internal abstract bool IsEndGame();
}

