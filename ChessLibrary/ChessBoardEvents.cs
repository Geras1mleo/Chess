namespace Chess;

public partial class ChessBoard
{
    /// <summary>
    /// Invokes when trying to make or validate move but after the move would have been made, king would have been checked
    /// </summary>
    public event ChessCheckedChangedEventHandler OnInvalidMoveKingChecked = delegate { };
    /// <summary>
    /// Invokes when one of kings is checked
    /// </summary>
    public event ChessCheckedChangedEventHandler OnKingCheckedChanged = delegate { };
    /// <summary>
    /// Invokes when user has to choose promote action
    /// </summary>
    public event ChessPromotionResultEventHandler OnPromotePawn = delegate { };
    /// <summary>
    /// Invokes when it's end game
    /// </summary>
    public event ChessEndGameEventHandler OnEndGame = delegate { };
    /// <summary>
    /// Invokes when any piece has been captured
    /// </summary>
    public event ChessCaptureEventHandler OnCaptured = delegate { };
    private readonly SynchronizationContext context = SynchronizationContext.Current;

    private void OnKingCheckedChangedEvent(CheckEventArgs e)
    {
        if (context != null)
            context.Post(delegate { OnKingCheckedChanged(this, e); }, null);
        else
            OnKingCheckedChanged(this, e);
    }

    private void OnInvalidMoveKingCheckedEvent(CheckEventArgs e)
    {
        if (context != null)
            context.Post(delegate { OnInvalidMoveKingChecked(this, e); }, null);
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
            context.Post(delegate { OnEndGame(this, new EndgameEventArgs(this, EndGame)); }, null);
        else
            OnEndGame(this, new EndgameEventArgs(this, EndGame));
    }

    private void OnCapturedEvent(CaptureEventArgs e)
    {
        if (context != null)
            context.Post(delegate { OnCaptured(this, e); }, null);
        else
            OnCaptured(this, e);
    }
}
