namespace Chess;

/// <summary>
/// Chess board logic
/// </summary>
public partial class ChessBoard
{
    /// <param name="pos">Position on chess board</param>
    /// <returns>Piece on given position</returns>
    public Piece? this[Position pos]
    {
        get => pieces[pos.Y, pos.X];
        private set => pieces[pos.Y, pos.X] = value;
    }
    /// <param name="x">a->h</param>
    /// <param name="y">1->8</param>
    /// <returns>Piece on given position</returns>
    public Piece? this[char x, short y] => this[new Position(x.ToString() + y.ToString())];
    /// <param name="s">Position as string ex. e4/a1/h8...</param>
    /// <returns>Piece on given position</returns>
    public Piece? this[string s] => this[new Position(s)];
    /// <param name="x">0->8</param>
    /// <param name="y">0->8</param>
    /// <returns>Piece on given position</returns>
    public Piece? this[short x, short y] => pieces[y, x];

    private Piece?[,] pieces;

    private FenBoard? Fen;
    /// <summary>
    /// Whether board has been loaded from Forsyth-Edwards Notation
    /// </summary>
    public bool LoadedFromFEN => Fen != null;

    /// <summary>
    /// Determinize whose player turn is it now
    /// </summary>
    public PieceColor Turn
    {
        get
        {
            if (LoadedFromFEN)
                return PerformedMoves.Count % 2 == 0 ? Fen.Turn : Fen.Turn.OppositeColor();
            else
                return PerformedMoves.Count % 2 == 0 ? PieceColor.White : PieceColor.Black;
        }
    }

    private bool whiteKingChecked = false;
    /// <summary>
    /// Returns state of White king (Checked or not)
    /// </summary>
    public bool WhiteKingChecked
    {
        get => whiteKingChecked;
        private set
        {
            if (value != whiteKingChecked)
            {
                whiteKingChecked = value;
                OnKingCheckedChangedEvent(new CheckEventArgs(this, WhiteKing, value));
            }
        }
    }

    private bool blackKingChecked = false;
    /// <summary>
    /// Returns state of Black king (Checked or not)
    /// </summary>
    public bool BlackKingChecked
    {
        get => blackKingChecked;
        private set
        {
            if (value != blackKingChecked)
            {
                blackKingChecked = value;
                OnKingCheckedChangedEvent(new CheckEventArgs(this, BlackKing, value));
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
    public Piece[] WhiteCaptured
    {
        get
        {
            var cap = new List<Piece>();

            cap.AddRange(PerformedMoves.Where(m => m.CapturedPiece?.Color == PieceColor.White).Select(m => new Piece(m.CapturedPiece.Color, m.CapturedPiece.Type)));

            if (LoadedFromFEN)
            {
                var pieces = Fen.Pieces.Cast<Piece>().Where(p => p?.Color == PieceColor.White);

                // Calculating missing pieces on fen begin situation
                // Math.Clamp() for get max/min taken figures (2 queens possible)
                cap.AddRange(Enumerable.Range(0, Math.Clamp(8 - pieces.Where(p => p.Type == PieceType.Pawn).Count(), 0, 8)).Select(p => new Piece(PieceColor.White, PieceType.Pawn)));
                cap.AddRange(Enumerable.Range(0, Math.Clamp(2 - pieces.Where(p => p.Type == PieceType.Rook).Count(), 0, 2)).Select(p => new Piece(PieceColor.White, PieceType.Rook)));
                cap.AddRange(Enumerable.Range(0, Math.Clamp(2 - pieces.Where(p => p.Type == PieceType.Bishop).Count(), 0, 2)).Select(p => new Piece(PieceColor.White, PieceType.Bishop)));
                cap.AddRange(Enumerable.Range(0, Math.Clamp(2 - pieces.Where(p => p.Type == PieceType.Knight).Count(), 0, 2)).Select(p => new Piece(PieceColor.White, PieceType.Knight)));
                cap.AddRange(Enumerable.Range(0, Math.Clamp(1 - pieces.Where(p => p.Type == PieceType.Queen).Count(), 0, 1)).Select(p => new Piece(PieceColor.White, PieceType.Queen)));
            }
            return cap.ToArray();
        }
    }

    /// <summary>
    /// Black pieces that has been captured by white player
    /// </summary>
    public Piece[] BlackCaptured
    {
        get
        {
            var cap = new List<Piece>();

            cap.AddRange(PerformedMoves.Where(m => m.CapturedPiece?.Color == PieceColor.Black).Select(m => new Piece(m.CapturedPiece.Color, m.CapturedPiece.Type)));

            if (LoadedFromFEN)
            {
                var pieces = Fen.Pieces.Cast<Piece>().Where(p => p?.Color == PieceColor.Black);

                cap.AddRange(Enumerable.Range(0, Math.Clamp(8 - pieces.Where(p => p.Type == PieceType.Pawn).Count(), 0, 8)).Select(p => new Piece(PieceColor.Black, PieceType.Pawn)));
                cap.AddRange(Enumerable.Range(0, Math.Clamp(2 - pieces.Where(p => p.Type == PieceType.Rook).Count(), 0, 2)).Select(p => new Piece(PieceColor.Black, PieceType.Rook)));
                cap.AddRange(Enumerable.Range(0, Math.Clamp(2 - pieces.Where(p => p.Type == PieceType.Bishop).Count(), 0, 2)).Select(p => new Piece(PieceColor.Black, PieceType.Bishop)));
                cap.AddRange(Enumerable.Range(0, Math.Clamp(2 - pieces.Where(p => p.Type == PieceType.Knight).Count(), 0, 2)).Select(p => new Piece(PieceColor.Black, PieceType.Knight)));
                cap.AddRange(Enumerable.Range(0, Math.Clamp(1 - pieces.Where(p => p.Type == PieceType.Queen).Count(), 0, 1)).Select(p => new Piece(PieceColor.Black, PieceType.Queen)));
            }
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
            if (value != null) OnEndGameEvent();
        }
    }
    /// <summary>
    /// When true => use: EndGame. for more info on endgame(type, won side)
    /// </summary>
    public bool IsEndGame => EndGame != null;

    /// <summary>
    /// All performed moves on this chess board
    /// </summary>
    public List<Move> PerformedMoves { get; private set; }
    /// <summary>
    /// Performed moves in SAN
    /// </summary>
    public List<string> MovesInSan => PerformedMoves.Select(m => m.San).ToList();

    private int moveIndex = -1;
    /// <summary>
    /// Displayed move index in this chess board
    /// </summary>
    public int MoveIndex
    {
        get => moveIndex;
        set
        {
            if (value < PerformedMoves.Count && value >= 0)
                DisplayMoves(PerformedMoves.GetRange(0, value + 1));
        }
    }
    /// <summary>
    /// Is last move displayed on this chess board, false after DisplayPrevious()
    /// </summary>
    public bool IsLastMove => moveIndex == PerformedMoves.Count - 1;

    /// <summary>
    /// Creates new chess board with default pieces positions
    /// </summary>
    public ChessBoard()
    {
        PerformedMoves = new List<Move>();
        SetChessBeginSituation();
    }

    /// <summary>
    /// Creates new chess board and performs all given moves
    /// </summary>
    /// <param name="moves">Moves to be performed</param>
    public ChessBoard(List<Move> moves)
    {
        PerformedMoves = new List<Move>();
        SetChessBeginSituation();
        moves.ForEach(m => Move(m));
    }

    /// <summary>
    /// To perform operations and to not corrupt the main chess object
    /// </summary>
    private ChessBoard(Piece?[,] pieces, List<Move> moves)
    {
        PerformedMoves = new List<Move>(moves);
        this.pieces = (Piece[,])pieces.Clone();
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

        if (!IsLastMove)
            throw new ChessInvalidMoveException(this, "Please use board.DisplayLastMove(); to be able to perform new moves in this chess game", move);

        if (IsValidMove(move, true, true))
        {
            San(move);

            PerformedMoves.Add(move);

            DropPieceToNewPosition(move);

            moveIndex = PerformedMoves.Count - 1;

            HandleKingChecked();
            HandleEndGame();

            return true;
        }
        else return false;
    }

    private void DropPieceToNewPosition(Move move)
    {
        if (move.CapturedPiece != null)
            OnCapturedEventAsync(move.CapturedPiece);

        // Copying to the new position
        this[move.NewPosition] = this[move.OriginalPosition];
        // Clearing old position
        this[move.OriginalPosition] = null;

        if (move.Parameter != null)
            switch (move.Parameter)
            {
                case var e when e == MoveParameter.CastleKing:

                    this[new Position { Y = move.NewPosition.Y, X = 6 }] = new Piece(this[move.NewPosition].Color, PieceType.King);
                    this[new Position { Y = move.NewPosition.Y, X = 5 }] = new Piece(this[move.NewPosition].Color, PieceType.Rook);
                    this[move.NewPosition] = null;

                    break;
                case var e when e == MoveParameter.CastleQueen:

                    this[new Position { Y = move.NewPosition.Y, X = 2 }] = new Piece(this[move.NewPosition].Color, PieceType.King);
                    this[new Position { Y = move.NewPosition.Y, X = 3 }] = new Piece(this[move.NewPosition].Color, PieceType.Rook);
                    this[move.NewPosition] = null;

                    break;
                case var e when e == MoveParameter.EnPassant:

                    var pawnPos = PerformedMoves[^2].NewPosition;
                    this[pawnPos] = null;

                    break;
                case var e when e == MoveParameter.PromotionToQueen || e == MoveParameter.PawnPromotion:
                    move.Piece.Type = PieceType.Queen;
                    break;
                case var e when e == MoveParameter.PromotionToRook:
                    move.Piece.Type = PieceType.Rook;
                    break;
                case var e when e == MoveParameter.PromotionToBishop:
                    move.Piece.Type = PieceType.Bishop;
                    break;
                case var e when e == MoveParameter.PromotionToKnight:
                    move.Piece.Type = PieceType.Knight;
                    break;
            }
    }

    /// <summary>
    /// Puts given piece on given position
    /// </summary>
    public void Put(Piece piece, Position position)
    {
        this[position] = piece;
    }

    /// <summary>
    /// Removes a piece from position
    /// </summary>
    public void Remove(Position position)
    {
        this[position] = null;
    }

    /// <summary>
    /// Clears board<br/>
    /// Restore: board.Restore() => only when clearMoves = false<br/>
    /// When clearMoves = true and board has been loaded from fen => fen begin situation will be cleared
    /// </summary>
    /// <param name="setToBeginPositions">Set begin positions after the board was cleared</param>
    /// <param name="clearMoves">Clear performed moves, if true => cannot restore postions later with: board.Restore()</param>
    public void Clear(bool setToBeginPositions = false, bool clearMoves = false)
    {
        if (setToBeginPositions)
            if (LoadedFromFEN)
                pieces = Fen.Pieces;
            else
                SetChessBeginSituation();
        else
            pieces = new Piece[8, 8];

        if (clearMoves)
        {
            PerformedMoves.Clear();
            moveIndex = -1;
            endGame = null;
            Fen = null;
            HandleKingChecked();
        }
    }

    /// <summary>
    /// Restores board according to moves
    /// </summary>
    public void Restore()
    {
        DisplayMoves(PerformedMoves);
    }

    /// <summary>
    /// Cancel last move and display previous pieces positions
    /// </summary>
    public void Cancel()
    {
        if (PerformedMoves.Count > 0)
        {
            DisplayMoves(PerformedMoves);

            if (IsEndGame)
                EndGame = null;
        }
    }

    /// <summary>
    /// Displaying first move(if exist)
    /// </summary>
    public void First() => MoveIndex = 0;

    /// <summary>
    /// Displaying last move
    /// </summary>
    public void Last() => MoveIndex = PerformedMoves.Count - 1;

    /// <summary>
    /// Displaying next move(if exist)
    /// </summary>
    public void Next() => MoveIndex++;

    /// <summary>
    /// Displaying previous move(if exist)
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

        EndGame = new EndGameInfo(EndgameType.Draw, null);
    }

    private void DisplayMoves(List<Move> moves)
    {
        if (LoadedFromFEN)
            pieces = Fen.Pieces;
        else
            SetChessBeginSituation();

        for (int i = 0; i < moves.Count; i++)
            DropPieceToNewPosition(moves[i]);

        moveIndex = moves.Count - 1;

        HandleKingChecked();
    }

    private void HandleKingChecked()
    {
        if (moveIndex >= 0)
        {
            if (PerformedMoves[moveIndex].Piece.Color == PieceColor.White)
                BlackKingChecked = PerformedMoves[moveIndex].IsCheck;

            else if (PerformedMoves[moveIndex].Piece.Color == PieceColor.Black)
                WhiteKingChecked = PerformedMoves[moveIndex].IsCheck;
        }
        else
        {
            WhiteKingChecked = IsKingChecked(PieceColor.White, this);
            BlackKingChecked = IsKingChecked(PieceColor.Black, this);
        }
    }

    /// <summary>
    /// Checking if there is a checklame or stalemate
    /// and updating end game state
    /// </summary>
    private void HandleEndGame()
    {
        if (moveIndex >= 0)
        {
            if (PerformedMoves[moveIndex].IsMate)
                if (PerformedMoves[moveIndex].IsCheck)
                    EndGame = new EndGameInfo(EndgameType.Checkmate, Turn.OppositeColor());
                else
                    EndGame = new EndGameInfo(EndgameType.Stalemate, null);
        }
        else
        {
            var mw = !PlayerHasMoves(PieceColor.White, this);
            var mb = !PlayerHasMoves(PieceColor.Black, this);

            if (mw && IsKingChecked(PieceColor.White, this))
                EndGame = new EndGameInfo(EndgameType.Checkmate, PieceColor.White);

            else if (mb && IsKingChecked(PieceColor.Black, this))
                EndGame = new EndGameInfo(EndgameType.Checkmate, PieceColor.Black);

            else if (mw || mb)
                EndGame = new EndGameInfo(EndgameType.Stalemate, null);
        }
    }
}
