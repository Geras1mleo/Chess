#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
namespace Chess;

public delegate void ChessEventHandler(object sender, ChessEventArgs e);
public delegate void ChessCheckedChangedEventHandler(object sender, CheckEventArgs e);
public delegate void ChessEndGameEventHandler(object sender, EndgameEventArgs e);
public delegate void ChessCaptureEventHandler(object sender, CaptureEventArgs e);
public delegate void ChessPromotionResultEventHandler(object sender, PromotionEventArgs e);

public abstract class ChessEventArgs : EventArgs
{
    public ChessBoard ChessBoard { get; }

    protected ChessEventArgs(ChessBoard chessBoard)
    {
        ChessBoard = chessBoard;
    }
}
public class CaptureEventArgs : ChessEventArgs
{
    /// <summary>
    /// Piece that has been captured
    /// </summary>
    public Piece CapturedPiece { get; }
    /// <summary>
    /// List of captured pieces where color == White
    /// </summary>
    public Piece[] WhiteCapturedPieces { get; set; }
    /// <summary>
    /// List of captured pieces where color == Black
    /// </summary>
    public Piece[] BlackCapturedPieces { get; set; }

    public CaptureEventArgs(ChessBoard chessBoard, Piece capturedPiece, Piece[] whiteCapturedPieces, Piece[] blackCapturedPieces) : base(chessBoard)
    {
        CapturedPiece = capturedPiece;
        WhiteCapturedPieces = whiteCapturedPieces;
        BlackCapturedPieces = blackCapturedPieces;
    }
}
public class EndgameEventArgs : ChessEventArgs
{
    /// <summary>
    /// End game additional info
    /// </summary>
    public EndGameInfo EndgameInfo { get; }

    public EndgameEventArgs(ChessBoard chessBoard, EndGameInfo endgameInfo) : base(chessBoard)
    {
        EndgameInfo = endgameInfo;
    }
}
public class CheckEventArgs : ChessEventArgs
{
    /// <summary>
    /// Position of checked king
    /// </summary>
    public Position KingPosition { get; }
    /// <summary>
    /// Checked state
    /// </summary>
    public bool IsChecked { get; }

    public CheckEventArgs(ChessBoard chessBoard, Position kingPosition, bool isChecked) : base(chessBoard)
    {
        KingPosition = kingPosition;
        IsChecked = isChecked;
    }
}

public class PromotionEventArgs : ChessEventArgs
{
    private MoveParameter propmotionResult = MoveParameter.PawnPromotion;

    /// <summary>
    /// Set to true when Promotion has been handled by user
    /// </summary>
    public bool Handled { get; set; }
    /// <summary>
    /// Allowed:<br/>
    /// MoveParameter.PawnPromotion<br/>
    /// MoveParameter.PromotionToQueen<br/>
    /// MoveParameter.PromotionToRook<br/>
    /// MoveParameter.PromotionToBishop<br/>
    /// MoveParameter.PromotionToKnight<br/>
    /// </summary>
    public MoveParameter PromotionResult
    {
        get => propmotionResult;
        set
        {
            if (value == MoveParameter.PawnPromotion
             || value == MoveParameter.PromotionToQueen
             || value == MoveParameter.PromotionToRook
             || value == MoveParameter.PromotionToBishop
             || value == MoveParameter.PromotionToKnight)
            {
                Handled = true;
                propmotionResult = value;
            }
            else throw new InvalidOperationException("MoveParameter for promotion result must be promotion type.");
        }
    }

    public PromotionEventArgs(ChessBoard chessBoard) : base(chessBoard) { }
}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member