// *****************************************************
// *                                                   *
// * O Lord, Thank you for your goodness in our lives. *
// *     Please bless this code to our compilers.      *
// *                     Amen.                         *
// *                                                   *
// *****************************************************
//                                    Made by Geras1mleo

namespace Chess;

/// <summary>
/// Chess board logic
/// </summary>
public partial class ChessBoard
{
    internal Piece?[,] pieces;

    /// <summary>
    /// Returns Piece on given position
    /// </summary>
    /// <param name="pos">Position on chess board</param>
    public Piece? this[Position pos] => pieces[pos.Y, pos.X];

    /// <summary>
    /// Returns Piece on given position
    /// </summary>
    /// <param name="x">a->h</param>
    /// <param name="y">1->8</param>
    public Piece? this[char x, short y] => this[new Position(x.ToString() + y.ToString())];

    /// <summary>
    /// Returns Piece on given position
    /// </summary>
    /// <param name="s">
    /// Position as string<br/>
    /// ex.: e4 / a1 / h8 etc.
    /// </param>
    public Piece? this[string s] => this[new Position(s)];

    /// <summary>
    /// Returns Piece on given position
    /// </summary>
    /// <param name="x">0->8</param>
    /// <param name="y">0->8</param>
    public Piece? this[short x, short y] => pieces[y, x];

    private readonly Dictionary<string, string> headers;

    /// <summary>
    /// Headers of current chess board
    /// </summary>
    public IReadOnlyDictionary<string, string> Headers => new Dictionary<string, string>(headers);

    private FenBoard? FenObj;
    /// <summary>
    /// Whether board has been loaded from Forsyth-Edwards Notation
    /// </summary>
    public bool LoadedFromFEN => FenObj is not null;

    /// <summary>
    /// Determinize whose player turn is it now
    /// </summary>
    public PieceColor Turn
    {
        get
        {
            if (LoadedFromFEN)
                return DisplayedMoves.Count % 2 == 0 ? FenObj.Turn : FenObj.Turn.OppositeColor();
            else
                return DisplayedMoves.Count % 2 == 0 ? PieceColor.White : PieceColor.Black;

        }
    }

    private bool whiteKingChecked = false;
    /// <summary>
    /// Returns the state of White king (Checked or not)
    /// </summary>
    public bool WhiteKingChecked
    {
        get => whiteKingChecked;
        private set
        {
            if (value != whiteKingChecked)
            {
                whiteKingChecked = value;
                OnWhiteKingCheckedChangedEvent(new CheckEventArgs(this, WhiteKing, value));
            }
        }
    }

    private bool blackKingChecked = false;
    /// <summary>
    /// Returns the state of Black king (Checked or not)
    /// </summary>
    public bool BlackKingChecked
    {
        get => blackKingChecked;
        private set
        {
            if (value != blackKingChecked)
            {
                blackKingChecked = value;
                OnBlackKingCheckedChangedEvent(new CheckEventArgs(this, BlackKing, value));
            }
        }
    }

    /// <summary>
    /// Returns White king position on chess board
    /// </summary>
    public Position WhiteKing => GetKingPosition(PieceColor.White, this);
    /// <summary>
    /// Returns White king position on chess board
    /// </summary>
    public Position BlackKing => GetKingPosition(PieceColor.Black, this);

    /// <summary>
    /// White pieces that has been captured by black player
    /// </summary>
    public Piece[] CapturedWhite
    {
        get
        {
            var cap = new List<Piece>();

            if (LoadedFromFEN) cap.AddRange(FenObj.WhiteCaptured);

            cap.AddRange(DisplayedMoves.Where(m => m.CapturedPiece?.Color == PieceColor.White).Select(m => new Piece(m.CapturedPiece.Color, m.CapturedPiece.Type)));

            return cap.ToArray();
        }
    }

    /// <summary>
    /// Black pieces that has been captured by white player
    /// </summary>
    public Piece[] CapturedBlack
    {
        get
        {
            var cap = new List<Piece>();

            if (LoadedFromFEN) cap.AddRange(FenObj.BlackCaptured);

            cap.AddRange(DisplayedMoves.Where(m => m.CapturedPiece?.Color == PieceColor.Black).Select(m => new Piece(m.CapturedPiece.Color, m.CapturedPiece.Type)));

            return cap.ToArray();
        }
    }

    private EndGameInfo? endGame;
    /// <summary>
    /// Represents End of game state(or null), won side(or draw) and type of end game
    /// </summary>
    public EndGameInfo? EndGame
    {
        get => endGame;
        private set
        {
            endGame = value;
            if (value is not null) OnEndGameEvent();
        }
    }
    /// <summary>
    /// When true => use: EndGame. for more info on endgame(type, won side)
    /// </summary>
    public bool IsEndGame => EndGame is not null;

    private readonly List<Move> executedMoves;
    /// <summary>
    /// Executed moves on this chess board<br/>
    /// </summary>
    public IReadOnlyList<Move> ExecutedMoves => new List<Move>(executedMoves);

    /// <summary>
    /// Executed moves in SAN
    /// </summary>
    public List<string> MovesInSan => new List<Move>(executedMoves).Select(m => m.San).ToList();

    private int moveIndex = -1;
    /// <summary>
    /// Displayed move index in this chess board
    /// </summary>
    public int MoveIndex
    {
        get => moveIndex;
        set
        {
            if (value < executedMoves.Count && value >= -1)
                DisplayMoves(executedMoves.GetRange(0, value + 1));
            else
                throw new IndexOutOfRangeException("Move not found");
        }
    }
    /// <summary>
    /// Is last move displayed on this chess board, false after DisplayPrevious()
    /// </summary>
    public bool IsLastMoveDisplayed => moveIndex == executedMoves.Count - 1;

    private List<Move> DisplayedMoves => executedMoves.GetRange(0, moveIndex + 1);   

    /// <summary>
    /// Creates new chess board with default pieces positions
    /// </summary>
    public ChessBoard()
    {
        executedMoves = new List<Move>();
        headers = new Dictionary<string, string>();
        SetChessBeginSituation();
    }

    /// <summary>
    /// To execute operations and to not corrupt the main chess object
    /// </summary>
    private ChessBoard(Piece?[,] pieces, List<Move> moves)
    {
        executedMoves = new List<Move>(moves);
        this.pieces = (Piece[,])pieces.Clone();
    }

    /// <summary>
    /// Converts san move into Move object and performs it on chess board
    /// </summary>
    /// <param name="sanMove">Chess move in SAN</param>
    public bool Move(string sanMove)
    {
        return Move(San(sanMove));
    }

    /// <summary>
    /// Validates the move and performs it on chess board
    /// </summary>
    /// <param name="move">Move that will be validated and performed</param>
    /// <returns>Validation succeed and move is performed => true</returns>
    /// <exception cref="ArgumentNullException">Move is null</exception>
    /// <exception cref="ChessGameEndedException">Game Ended</exception>
    /// <exception cref="ChessInvalidMoveException">Currently displaying one of previous moves, use first board.DisplayLastMove();</exception>
    /// <exception cref="ChessPieceNotFoundException">Piece on given position not found</exception>
    public bool Move(Move move)
    {
        if (IsEndGame)
            throw new ChessGameEndedException(this, EndGame);

        if (!IsLastMoveDisplayed)
            throw new ChessInvalidMoveException(this, "Please use board.DisplayLastMove(); to be able to perform new moves in this chess game", move);

        if (IsValidMove(move, this, true, true))
        {
            San(move);

            executedMoves.Add(move);

            if (move.CapturedPiece is not null)
                OnCapturedEvent(move.CapturedPiece);

            DropPieceToNewPosition(move);

            moveIndex = executedMoves.Count - 1;

            HandleKingChecked();
            HandleEndGame();

            return true;
        }
        else return false;
    }

    /// <summary>
    /// Adding header to this chess game<br/>
    /// ex.:<br/>
    /// name => Black; value => Geras1mleo<br/>
    /// Pgn Output: [Black "Geras1mleo"]
    /// </summary>
    /// <param name="name">Header name</param>
    /// <param name="value">Header value</param>
    public void AddHeader(string name, string value)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentNullException(nameof(name));
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentNullException(nameof(value));

        if (name.ToLower() == "fen")
            throw new ArgumentException("To load game from fen please use: board.LoadFen();");

        headers.Add(name, value);
    }

    /// <summary>
    /// Removing header from this chess game
    /// </summary>
    /// <param name="name">Header name</param>
    public void RemoveHeader(string name)
    {
        if (name.ToLower() == "fen")
            throw new ArgumentException("Could not remove FEN header from current game: FEN header required if loaded from it");

        headers.Remove(name);
    }

    /// <summary>
    /// Puts given piece on given position<br/>
    /// Warning! Checked state and end game state is not being updated
    /// </summary>
    public void Put(Piece piece, Position position)
    {
        pieces[position.Y, position.X] = piece;
    }

    /// <summary>
    /// Removes a piece on given position from board<br/>
    /// Warning! Checked state and end game state is not being updated
    /// </summary>
    public void Remove(Position position)
    {
        pieces[position.Y, position.X] = null;
    }

    /// <summary>
    /// Clears board and restores begin positions
    /// </summary>
    public void Clear()
    {
        SetChessBeginSituation();
        executedMoves.Clear();
        headers.Clear();
        moveIndex = -1;
        endGame = null;
        FenObj = null;
        WhiteKingChecked = false;
        BlackKingChecked = false;
    }

    /// <summary>
    /// Restores board according to moves and/or positions loaded from fen<br/>
    /// Similar to Last()
    /// </summary>
    public void Restore()
    {
        DisplayMoves(executedMoves);
    }

    /// <summary>
    /// Cancel last move and display previous pieces positions
    /// </summary>
    public void Cancel()
    {
        if (IsLastMoveDisplayed && executedMoves.Count > 0)
        {
            var move = executedMoves[^1];

            if (move.Parameter is not null)
                move.Parameter.Undo(move, this);
            else
                RestorePiece(move, this);

            executedMoves.RemoveAt(executedMoves.Count - 1);
            moveIndex = executedMoves.Count - 1;

            HandleKingChecked();
            EndGame = null;
        }
    }

    /// <summary>
    /// Displaying first move(if possible)
    /// </summary>
    public void First() => MoveIndex = 0;

    /// <summary>
    /// Displaying last move(if possible)
    /// </summary>
    public void Last() => MoveIndex = executedMoves.Count - 1;

    /// <summary>
    /// Displaying next move(if possible)
    /// </summary>
    public void Next() => MoveIndex++;

    /// <summary>
    /// Displaying previous move(if possible)
    /// </summary>
    public void Previous() => MoveIndex--;

    /// <summary>
    /// Declares resign of one of sides
    /// </summary>
    /// <param name="resignedSide">Resigned side</param>
    /// <exception cref="ChessGameEndedException"></exception>
    public void Resign(PieceColor resignedSide)
    {
        if (IsEndGame)
            throw new ChessGameEndedException(this, EndGame);

        EndGame = new EndGameInfo(EndgameType.Resigned, resignedSide.OppositeColor());
    }

    /// <summary>
    /// Declares draw in current chess game
    /// </summary>
    /// <exception cref="ChessGameEndedException"></exception>
    public void Draw()
    {
        if (IsEndGame)
            throw new ChessGameEndedException(this, EndGame);

        EndGame = new EndGameInfo(EndgameType.DrawDeclared, null);
    }

    private void DropPieceToNewPosition(Move move)
    {
        if (move.Parameter is not null)
        {
            move.Parameter.Execute(move, this);
            return;
        }

        DropPiece(move, this);
    }

    /// <summary>
    /// Default dropping piece implementation<br/>
    /// Clearing old, copy to new...
    /// </summary>
    internal static void DropPiece(Move move, ChessBoard board)
    {
        // Moving piece to its new position
        board.pieces[move.NewPosition.Y, move.NewPosition.X] = new(board.pieces[move.OriginalPosition.Y, move.OriginalPosition.X].Color, board.pieces[move.OriginalPosition.Y, move.OriginalPosition.X].Type);

        // Clearing old position
        board.pieces[move.OriginalPosition.Y, move.OriginalPosition.X] = null;
    }

    internal static void RestorePiece(Move move, ChessBoard board)
    {
        // Moving piece to its original position
        board.pieces[move.OriginalPosition.Y, move.OriginalPosition.X] = new(board.pieces[move.NewPosition.Y, move.NewPosition.X].Color, board.pieces[move.NewPosition.Y, move.NewPosition.X].Type);

        // Clearing new position / or setting captured piece back
        board.pieces[move.NewPosition.Y, move.NewPosition.X] = move.CapturedPiece;
    }

    private void DisplayMoves(List<Move> moves)
    {
        if (LoadedFromFEN)
            pieces = FenObj.Pieces;
        else
            SetChessBeginSituation();

        for (int i = 0; i < moves.Count; i++)
            DropPieceToNewPosition(moves[i]);

        moveIndex = moves.Count - 1;

        HandleKingChecked();
    }

    /// <summary>
    /// Checking if there one of kings are checked
    /// and updating checked states
    /// </summary>
    private void HandleKingChecked()
    {
        WhiteKingChecked = false;
        BlackKingChecked = false;

        if (moveIndex >= 0)
        {
            if (executedMoves[moveIndex].IsCheck)
            {
                if (executedMoves[moveIndex].Piece.Color == PieceColor.White)
                    BlackKingChecked = true;

                else if (executedMoves[moveIndex].Piece.Color == PieceColor.Black)
                    WhiteKingChecked = true;
            }
        }
        else
        {
            if (LoadedFromFEN)
            {
                WhiteKingChecked = IsKingChecked(PieceColor.White, this);
                BlackKingChecked = IsKingChecked(PieceColor.Black, this);
            }
        }
    }

    /// <summary>
    /// Checking if there is a checkmate or stalemate
    /// and updating end game state
    /// </summary>
    private void HandleEndGame()
    {
        if (moveIndex >= 0)
        {
            if (executedMoves[moveIndex].IsMate)
                if (executedMoves[moveIndex].IsCheck)
                    EndGame = new EndGameInfo(EndgameType.Checkmate, Turn.OppositeColor());
                else
                    EndGame = new EndGameInfo(EndgameType.Stalemate, null);
        }
        else
        {
            if (LoadedFromFEN)
            {
                var mw = !PlayerHasMoves(PieceColor.White, this);
                var mb = !PlayerHasMoves(PieceColor.Black, this);

                if (mw && WhiteKingChecked)
                    EndGame = new EndGameInfo(EndgameType.Checkmate, PieceColor.Black);

                else if (mb && BlackKingChecked)
                    EndGame = new EndGameInfo(EndgameType.Checkmate, PieceColor.White);

                else if (mw || mb)
                    EndGame = new EndGameInfo(EndgameType.Stalemate, null);
            }
        }

        // Todo:
        // InsufficientMaterial
        // Move50Rule
        // Repetition
    }

    private void SetChessBeginSituation()
    {
        pieces = new Piece[8, 8];

        pieces[0, 0] = new Piece(PieceColor.White, PieceType.Rook);
        pieces[0, 1] = new Piece(PieceColor.White, PieceType.Knight);
        pieces[0, 2] = new Piece(PieceColor.White, PieceType.Bishop);
        pieces[0, 3] = new Piece(PieceColor.White, PieceType.Queen);
        pieces[0, 4] = new Piece(PieceColor.White, PieceType.King);
        pieces[0, 5] = new Piece(PieceColor.White, PieceType.Bishop);
        pieces[0, 6] = new Piece(PieceColor.White, PieceType.Knight);
        pieces[0, 7] = new Piece(PieceColor.White, PieceType.Rook);

        pieces[1, 0] = new Piece(PieceColor.White, PieceType.Pawn);
        pieces[1, 1] = new Piece(PieceColor.White, PieceType.Pawn);
        pieces[1, 2] = new Piece(PieceColor.White, PieceType.Pawn);
        pieces[1, 3] = new Piece(PieceColor.White, PieceType.Pawn);
        pieces[1, 4] = new Piece(PieceColor.White, PieceType.Pawn);
        pieces[1, 5] = new Piece(PieceColor.White, PieceType.Pawn);
        pieces[1, 6] = new Piece(PieceColor.White, PieceType.Pawn);
        pieces[1, 7] = new Piece(PieceColor.White, PieceType.Pawn);

        pieces[6, 0] = new Piece(PieceColor.Black, PieceType.Pawn);
        pieces[6, 1] = new Piece(PieceColor.Black, PieceType.Pawn);
        pieces[6, 2] = new Piece(PieceColor.Black, PieceType.Pawn);
        pieces[6, 3] = new Piece(PieceColor.Black, PieceType.Pawn);
        pieces[6, 4] = new Piece(PieceColor.Black, PieceType.Pawn);
        pieces[6, 5] = new Piece(PieceColor.Black, PieceType.Pawn);
        pieces[6, 6] = new Piece(PieceColor.Black, PieceType.Pawn);
        pieces[6, 7] = new Piece(PieceColor.Black, PieceType.Pawn);

        pieces[7, 0] = new Piece(PieceColor.Black, PieceType.Rook);
        pieces[7, 1] = new Piece(PieceColor.Black, PieceType.Knight);
        pieces[7, 2] = new Piece(PieceColor.Black, PieceType.Bishop);
        pieces[7, 3] = new Piece(PieceColor.Black, PieceType.Queen);
        pieces[7, 4] = new Piece(PieceColor.Black, PieceType.King);
        pieces[7, 5] = new Piece(PieceColor.Black, PieceType.Bishop);
        pieces[7, 6] = new Piece(PieceColor.Black, PieceType.Knight);
        pieces[7, 7] = new Piece(PieceColor.Black, PieceType.Rook);
    }
}
