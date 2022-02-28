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
    /// Converts San move into Move object<br/>
    /// Long algebraic notation is also acceptable
    /// </summary>
    /// <param name="move">San move to be converted</param>
    /// <returns>Move object according to given san</returns>
    /// <exception cref="ArgumentNullException">move was null</exception>
    /// <exception cref="ArgumentException">Given move didn't match the Regex pattern</exception>
    /// <exception cref="ChessSanNotFoundException">Given SAN move is not valid for current board positions</exception>
    /// <exception cref="ChessSanTooAmbiguousException">Given SAN move is too ambiguous between multiple moves</exception>
    public Move San(string move)
    {
        if (move is null)
            throw new ArgumentNullException(nameof(move));

        var matches = Regexes.regexSanOneMove.Matches(move);

        if (matches.Count == 0)
            throw new ArgumentException("SAN Move should match pattern: " + Regexes.SanMovesPattern);

        Move moveOut = new();
        Position originalPos = new();
        bool isCapture = false;

        foreach (var group in matches[0].Groups.Values)
        {
            if (!group.Success) continue;

            switch (group.Name)
            {
                case "1":
                    if (group.Value == "O-O" || group.Value == "O-O-O")
                    {
                        moveOut.Parameter = IMoveParameter.FromString(group.Value);
                        if (Turn == PieceColor.White)
                        {
                            originalPos = new("e1");
                            if (group.Value == "O-O")
                                moveOut.NewPosition = new("h1");
                            else if (group.Value == "O-O-O")
                                moveOut.NewPosition = new("a1");
                        }
                        else if (Turn == PieceColor.Black)
                        {
                            originalPos = new("e8");
                            if (group.Value == "O-O")
                                moveOut.NewPosition = new("h8");
                            else if (group.Value == "O-O-O")
                                moveOut.NewPosition = new("a8");
                        }
                        // not realy needed
                        //if (!IsValidMove(new Move(originalPos, moveOut.NewPosition)))
                        //    throw new ChessSanNotFoundException(this, move);
                    }
                    break;
                case "2":
                    moveOut.Piece = new Piece(Turn, PieceType.FromChar(group.Value[0]));
                    break;
                case "3":
                    originalPos.X = Position.FromFile(group.Value[0]);
                    break;
                case "4":
                    originalPos.Y = Position.FromRank(group.Value[0]);
                    break;
                case "5":
                    if (group.Value == "x" || group.Value == "X")
                        isCapture = true;
                    break;
                case "6":
                    moveOut.NewPosition = new Position(group.Value);
                    break;
                case "7":
                    moveOut.Parameter = IMoveParameter.FromString(group.Value.Trim());
                    break;
                case "9":
                    switch (group.Value)
                    {
                        case "+":
                            moveOut.IsCheck = true;
                            break;
                        case "#":
                            moveOut.IsCheck = true;
                            moveOut.IsMate = true;
                            break;
                        case "$":
                            moveOut.IsMate = true;
                            break;
                    }
                    break;
            }
        }

        // If piece is not specified => Pawn
        moveOut.Piece ??= new Piece(Turn, PieceType.Pawn);

        if (isCapture && this[moveOut.NewPosition] is not null)
            moveOut.CapturedPiece = this[moveOut.NewPosition];

        if (!originalPos.HasValue)
        {
            var amb = GetMovesOfPieceOnPosition(moveOut.Piece, moveOut.NewPosition, this).ToList();

            if (originalPos.HasValueX)
            {
                amb = amb.Where(m => m.OriginalPosition.X == originalPos.X).ToList();

                if (amb.Count == 1)
                    originalPos.Y = amb.ElementAt(0).OriginalPosition.Y;

                else ThrowException(amb.Count, amb);
            }
            else if (originalPos.HasValueY)
            {
                amb = amb.Where(m => m.OriginalPosition.Y == originalPos.Y).ToList();

                if (amb.Count == 1)
                    originalPos.X = amb.ElementAt(0).OriginalPosition.X;

                else ThrowException(amb.Count, amb);
            }
            else
            {
                if (amb.Count == 1)
                {
                    originalPos.X = amb.ElementAt(0).OriginalPosition.X;
                    originalPos.Y = amb.ElementAt(0).OriginalPosition.Y;
                }
                else ThrowException(amb.Count, amb);
            }
            void ThrowException(int count, List<Move> moves)
            {
                if (count < 1)
                    throw new ChessSanNotFoundException(this, move);
                else if (count > 1)
                    throw new ChessSanTooAmbiguousException(this, move, moves.ToArray());
            }
        }

        moveOut.OriginalPosition = originalPos;
        San(moveOut);

        return moveOut;
    }

    /// <summary>
    /// Converts move object to string-move in Standard Algebraic notation
    /// </summary>
    /// <param name="move">Move to be converted</param>
    /// <returns>Move in SAN</returns>
    /// <exception cref="ArgumentNullException">Given move was null or didn't have valid positions values</exception>
    public string San(Move move)
    {
        if (move is null || !move.HasValue)
            throw new ArgumentNullException(nameof(move));

        StringBuilder builder = new();

        if (move.Parameter is MoveCastle)
        {
            builder.Append(move.Parameter.ShortStr);
            goto CheckOrMateValidation;
        }

        if (move.Piece.Type != PieceType.Pawn)
        {
            builder.Append(char.ToUpper(move.Piece.Type.AsChar));

            // Only rook, knight, bishop(second from promotion) and queen(second from promotion) can have ambiguous moves
            if (move.Piece.Type != PieceType.King)
                builder.Append(HandleAmbiguousMovesNotation(move, this));
        }

        if (move.CapturedPiece is not null)
        {
            if (move.Piece.Type == PieceType.Pawn)
                builder.Append(move.OriginalPosition.File());

            builder.Append('x');
        }

        builder.Append(move.NewPosition);

        if (move.Parameter is MovePromotion)
            builder.Append(move.Parameter.ShortStr);

        // Not required
        //else if (move.Parameter == MoveParameter.EnPassant)
        //    builder.Append(" " + move.Parameter.AsShortString);

        CheckOrMateValidation:

        if (move.IsCheck && move.IsMate) builder.Append('#');

        else if (move.IsCheck) builder.Append('+');

        else if (move.IsMate) builder.Append('$');


        return move.San = builder.ToString();
    }

    /// <summary>
    /// Loads Chess game from Forsyth-Edwards Notation<br/>
    /// ex.:<br/>
    /// rnbqkbnr/pppppppp/8/8/4P3/8/PPPP1PPP/RNBQKBNR b KQkq e3 0 1
    /// </summary>
    /// <param name="fen">FEN string</param>
    /// <exception cref="ArgumentException">Given FEN string didn't match the Regex pattern</exception>
    public void LoadFen(string fen)
    {
        FenObj = new FenBoard(fen);
        pieces = FenObj.Pieces;
        executedMoves.Clear();
        moveIndex = -1;
        endGame = null;

        headers.Clear();
        headers.Add("Variant", "From Position");
        headers.Add("FEN", fen);

        HandleKingChecked();
        HandleEndGame();
    }

    /// <summary>
    /// Loads Chess game from Portable Game Notation<br/>
    /// ex.:<br/>
    /// [White "Milan1905"]<br/>
    /// [Black "Geras1mleo"]<br/>
    /// [Result "1-0"]<br/>
    /// [WhiteElo "1006"]<br/>
    /// [BlackElo "626"]<br/>
    /// [Termination "Milan1905 won by resignation"]<br/>
    /// <br/>
    /// 1. e4 e5 2. Nf3 Nf6 3...
    /// </summary>
    /// <param name="pgn">PGN string</param>
    public void LoadPgn(string pgn)
    {
        SetChessBeginSituation();
        FenObj = null;
        executedMoves.Clear();
        headers.Clear();
        moveIndex = -1;
        endGame = null;

        var headersMatches = Regexes.regexHeaders.Matches(pgn);

        if (headersMatches.Count > 0)
        {
            // Extracting headers
            for (int i = 0; i < headersMatches.Count; i++)
            {
                // [Black "Geras1mleo"]
                // Groups[1] = Black
                // Groups[2] = Geras1mleo
                headers.Add(headersMatches[i].Groups[1].Value,
                            headersMatches[i].Groups[2].Value);
            }
        }

        // San move can occur in header ex. in nickname of player => remove headers from string
        pgn = Regexes.regexHeaders.Replace(pgn, "");

        // Loading fen if exist
        if (headers.TryGetValue("FEN", out var fen))
        {
            FenObj = new FenBoard(fen);
            pieces = FenObj.Pieces;
        }

        // Remove all alternatives
        pgn = Regexes.regexAlternatives.Replace(pgn, "");

        // Remove all comments
        pgn = Regexes.regexComments.Replace(pgn, "");

        // Todo Save Alternative moves(bracnhes) and Comments for moves

        var movesMatches = Regexes.regexSanMoves.Matches(pgn);

        // Execute all found moves
        for (int i = 0; i < movesMatches.Count; i++)
        {
            var move = San(movesMatches[i].Value);
            if (IsValidMove(move, this, false, true))
            {
                // Regenerate SAN after IsValidMove
                San(move);

                executedMoves.Add(move);
                DropPieceToNewPosition(move);

                moveIndex = executedMoves.Count - 1;
            }
        }

        HandleKingChecked();
        HandleEndGame();

        // If not actual end game but game is in fact ended => resign
        if (!IsEndGame)
        {
            if (pgn.Contains("1-0"))
                Resign(PieceColor.Black);

            else if (pgn.Contains("0-1"))
                Resign(PieceColor.White);

            else if (pgn.Contains("1/2-1/2"))
                Draw();
        }
    }

    /// <summary>
    /// Generates FEN string representing current board
    /// </summary>
    public string ToFen()
    {
        return new FenBoard(this).ToString();
    }

    /// <summary>
    /// Generates PGN string representing current board
    /// </summary>
    public string ToPgn()
    {
        StringBuilder builder = new();

        foreach (var header in headers)
            builder.Append('[' + header.Key + @" """ + header.Value + '"' + ']' + '\n');

        if (headers.Count > 0)
            builder.Append('\n');

        // Needed for moves count logic
        moveIndex = -1;

        for (int i = 0, count = 0; i < executedMoves.Count; i++)
        {
            // Adding moves count when needed
            if (count != GetFullMovesCount(this))
            {
                count = GetFullMovesCount(this);

                // Add space before move count if not first move
                if (i != 0) builder.Append(' ');

                builder.Append(count + ".");
            }

            if (moveIndex == -1)
            {
                // From position?
                if (LoadedFromFen && FenObj.Turn == PieceColor.Black)
                    builder.Append("..");
            }

            builder.Append(' ' + executedMoves[i].San);

            moveIndex++;
        }

        if (IsEndGame)
        {
            if (EndGame.WonSide == PieceColor.White)
                builder.Append(" 1-0");
            else if (EndGame.WonSide == PieceColor.Black)
                builder.Append(" 0-1");
            else
                builder.Append(" 1/2-1/2");
        }

        // Back to positions
        Last();

        return builder.ToString();
    }

    /// <summary>
    /// Generates ASCII string representing current board
    /// </summary>
    public string ToAscii(bool displayFull = false)
    {
        StringBuilder builder = new("   ┌────────────────────────┐\n");

        for (int i = 8 - 1; i >= 0; i--)
        {
            builder.Append(" " + (i + 1) + " │");
            for (int j = 0; j < 8; j++)
            {
                builder.Append(' ');

                if (pieces[i, j] is not null)
                    builder.Append(pieces[i, j].ToFenChar());
                else
                    builder.Append('.');

                builder.Append(' ');
            }
            builder.Append("│\n");
        }

        builder.Append("   └────────────────────────┘\n");
        builder.Append("     a  b  c  d  e  f  g  h  \n");

        if (displayFull)
        {
            builder.Append('\n');

            builder.Append("  Turn: " + Turn + '\n');

            if (CapturedWhite.Length > 0)
                builder.Append("  White Captured: " + string.Join(", ", CapturedWhite.Select(p => p.ToFenChar())) + '\n');
            if (CapturedBlack.Length > 0)
                builder.Append("  Black Captured: " + string.Join(", ", CapturedBlack.Select(p => p.ToFenChar())) + '\n');
        }

        return builder.ToString();
    }

    private static string HandleAmbiguousMovesNotation(Move move, ChessBoard board)
    {
        var amb = GetMovesOfPieceOnPosition(move.Piece, move.NewPosition, board).Where(m => m.OriginalPosition != move.OriginalPosition).ToList();
        var origPos = move.OriginalPosition.ToString();

        if (amb.Count == 0)
            return "";

        else if (amb.Count == 1)
        {
            if (amb[0].OriginalPosition.X == move.OriginalPosition.X)
                return origPos[1].ToString();
            else
                return origPos[0].ToString();
        }
        else
        {
            StringBuilder builder = new();

            if (amb.Any(m => m.OriginalPosition.X == move.OriginalPosition.X))
                builder.Append(origPos[1]);

            if (amb.Any(m => m.OriginalPosition.Y == move.OriginalPosition.Y))
                builder.Append(origPos[0]);

            return builder.ToString();
        }
    }

    private static Move[] GetMovesOfPieceOnPosition(Piece piece, Position newPosition, ChessBoard board)
    {
        var moves = new List<Move>();
        Move move;

        for (short i = 0; i < 8; i++)
        {
            for (short j = 0; j < 8; j++)
            {
                if (board.pieces[i, j] is not null
                 && board.pieces[i, j].Color == piece.Color
                 && board.pieces[i, j].Type == piece.Type)
                {
                    // if original pos == new pos
                    if (newPosition.Y == i && newPosition.X == j) continue;

                    move = new Move(new() { Y = i, X = j }, newPosition) { Piece = piece };

                    if (IsValidMove(move, board) && !IsKingCheckedValidation(move, piece.Color, board))
                        moves.Add(move);
                }
            }
        }

        return moves.ToArray();
    }

    internal static int GetHalfMovesCount(ChessBoard board)
    {
        int index = board.DisplayedMoves.FindLastIndex(m => m.CapturedPiece is not null || m.Piece.Type == PieceType.Pawn);

        if (board.LoadedFromFen && index < 0)
            return board.FenObj.HalfMoves + board.moveIndex + 1;

        if (index >= 0)
            return board.moveIndex - index;
        else
            return board.moveIndex + 1;
    }

    internal static int GetFullMovesCount(ChessBoard board)
    {
        var count = 0;

        if (board.LoadedFromFen)
            count += (board.FenObj.FullMoves * 2) + (board.FenObj.Turn == PieceColor.Black ? 1 : 0) - 2;

        return (board.moveIndex + count + 3) / 2;
    }

    /// <summary>
    /// Creates new object of board and loads given FEN string<br/>
    /// Return value indicates whether load succeeded
    /// </summary>
    public static bool TryLoadFen(string fen, out ChessBoard? board)
    {
        try
        {
            board = new();
            board.LoadFen(fen);
            return true;
        }
        catch (Exception)
        {
            board = null;
            return false;
        }
    }

    /// <summary>
    /// Creates new object of board and loads given PGN string<br/>
    /// Return value indicates whether load succeeded
    /// </summary>
    public static bool TryLoadPgn(string pgn, out ChessBoard? board)
    {
        try
        {
            board = new();
            board.LoadPgn(pgn);
            return true;
        }
        catch (Exception)
        {
            board = null;
            return false;
        }
    }
}
