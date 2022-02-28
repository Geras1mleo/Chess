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
    /// Raises when trying to make or validate move but after the move would have been made, king would have been checked
    /// </summary>
    public event ChessCheckedChangedEventHandler OnInvalidMoveKingChecked = delegate { };
    /// <summary>
    /// Raises when white king is (un)checked
    /// </summary>
    public event ChessCheckedChangedEventHandler OnWhiteKingCheckedChanged = delegate { };
    /// <summary>
    /// Raises when black king is (un)checked
    /// </summary>
    public event ChessCheckedChangedEventHandler OnBlackKingCheckedChanged = delegate { };
    /// <summary>
    /// Raises when user has to choose promotion action
    /// </summary>
    public event ChessPromotionResultEventHandler OnPromotePawn = delegate { };
    /// <summary>
    /// Raises when it's end of game
    /// </summary>
    public event ChessEndGameEventHandler OnEndGame = delegate { };
    /// <summary>
    /// Raises when any piece has been captured
    /// </summary>
    public event ChessCaptureEventHandler OnCaptured = delegate { };
    private readonly SynchronizationContext context = SynchronizationContext.Current;

    private void OnWhiteKingCheckedChangedEvent(CheckEventArgs e)
    {
        if (context is not null)
            context.Send(delegate { OnWhiteKingCheckedChanged(this, e); }, null);
        else
            OnWhiteKingCheckedChanged(this, e);
    }

    private void OnBlackKingCheckedChangedEvent(CheckEventArgs e)
    {
        if (context is not null)
            context.Send(delegate { OnBlackKingCheckedChanged(this, e); }, null);
        else
            OnBlackKingCheckedChanged(this, e);
    }

    private void OnInvalidMoveKingCheckedEvent(CheckEventArgs e)
    {
        if (context is not null)
            context.Send(delegate { OnInvalidMoveKingChecked(this, e); }, null);
        else
            OnInvalidMoveKingChecked(this, e);
    }

    private void OnPromotePawnEvent(PromotionEventArgs e)
    {
        if (context is not null)
            context.Send(delegate { OnPromotePawn(this, e); }, null);
        else
            OnPromotePawn(this, e);
    }

    private void OnEndGameEvent()
    {
        if (context is not null)
            context.Send(delegate { OnEndGame(this, new EndgameEventArgs(this, EndGame)); }, null);
        else
            OnEndGame(this, new EndgameEventArgs(this, EndGame));
    }

    private void OnCapturedEvent(Piece piece)
    {
        if (context is not null)
            context.Send(delegate { OnCaptured(this, new CaptureEventArgs(this, piece, CapturedWhite, CapturedBlack)); }, null);
        else
            OnCaptured(this, new CaptureEventArgs(this, piece, CapturedWhite, CapturedBlack));
    }
}
