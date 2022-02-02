namespace Chess;

public partial class ChessBoard
{
    /// <summary>
    /// Invokes when trying to make or validate move but after the move would have been made, king would have been checked
    /// </summary>
    public event ChessCheckedChangedEventHandler OnInvalidMoveKingChecked = delegate { };
    /// <summary>
    /// Invokes when white king is (un)checked
    /// </summary>
    public event ChessCheckedChangedEventHandler OnWhiteKingCheckedChanged = delegate { };
    /// <summary>
    /// Invokes when black king is (un)checked
    /// </summary>
    public event ChessCheckedChangedEventHandler OnBlackKingCheckedChanged = delegate { };
    /// <summary>
    /// Invokes when user has to choose promote action
    /// </summary>
    public event ChessPromotionResultEventHandler OnPromotePawn = delegate { };
    /// <summary>
    /// Invokes when it's end of game
    /// </summary>
    public event ChessEndGameEventHandler OnEndGame = delegate { };
    /// <summary>
    /// Async! Invokes when any piece has been captured
    /// </summary>
    public event ChessCaptureEventHandler OnCaptured = delegate { };
    private readonly SynchronizationContext context = SynchronizationContext.Current;

    private void OnWhiteKingCheckedChangedEvent(CheckEventArgs e)
    {
        if (context != null)
            context.Send(delegate { OnWhiteKingCheckedChanged(this, e); }, null);
        else
            OnWhiteKingCheckedChanged(this, e);
    }

    private void OnBlackKingCheckedChangedEvent(CheckEventArgs e)
    {
        if (context != null)
            context.Send(delegate { OnBlackKingCheckedChanged(this, e); }, null);
        else
            OnBlackKingCheckedChanged(this, e);
    }

    private void OnInvalidMoveKingCheckedEvent(CheckEventArgs e)
    {
        if (context != null)
            context.Send(delegate { OnInvalidMoveKingChecked(this, e); }, null);
        else
            OnInvalidMoveKingChecked(this, e);
    }

    private void OnPromotePawnEvent(PromotionEventArgs e)
    {
        if (context != null)
            context.Send(delegate { OnPromotePawn(this, e); }, null);
        else
            OnPromotePawn(this, e);
    }

    private void OnEndGameEvent()
    {
        if (context != null)
            context.Send(delegate { OnEndGame(this, new EndgameEventArgs(this, EndGame)); }, null);
        else
            OnEndGame(this, new EndgameEventArgs(this, EndGame));
    }

    private void OnCapturedEvent(Piece piece)
    {
        if (context != null)
            context.Send(delegate { OnCaptured(this, new CaptureEventArgs(this, piece, WhiteCaptured, BlackCaptured)); }, null);
        else
            OnCaptured(this, new CaptureEventArgs(this, piece, WhiteCaptured, BlackCaptured));
    }
}
