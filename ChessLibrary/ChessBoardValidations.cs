// *****************************************************
// *                                                   *
// * O Lord, Thank you for your goodness in our lives. *
// *     Please bless this code to our compilers.      *
// *                     Amen.                         *
// *                                                   *
// *****************************************************

namespace Chess;

public partial class ChessBoard
{
    /// <summary>
    /// Checks if given move is valid for current pieces positions
    /// </summary>
    /// <param name="move">Move to be checked</param>
    /// <param name="invokeEvents">To invoke event handlers such as OnInvalidMoveKingChecked and OnPromotePawn</param>
    /// <param name="checkPlayerMove">Check if given piece in move object is able to move according to board.PlayerTurn</param>
    /// <returns>Whether given move is valid</returns>
    public bool IsValidMove(Move move, bool invokeEvents = true, bool checkPlayerMove = true)
    {
        return IsValidMove(move, this, invokeEvents, checkPlayerMove);
    }

    /// <summary>
    /// Returns all moves that the piece on given position can perform
    /// </summary>
    /// <param name="position">Position of piece</param>
    /// <param name="generateSan">Whether SAN notation needs to be generated. For higher productivity => set to false</param>
    /// <returns>All available moves for given piece</returns>
    public Move[] Moves(Position position, bool generateSan = true)
    {
        if (pieces[position.Y, position.X] == null)
            throw new ChessPieceNotFoundException(this, new Move(position, position));

        var moves = new List<Move>();
        Move move;

        for (short i = 0; i < 8; i++)
        {
            for (short j = 0; j < 8; j++)
            {
                // if original pos == new pos
                if (position.Y == i && position.X == j) continue;

                move = new Move(position, new Position { Y = i, X = j }) { Piece = pieces[position.Y, position.X] };

                if (IsValidMove(move, this, false, true))
                {
                    moves.Add(move);
                    if (generateSan) San(move);
                }
            }
        }

        return moves.ToArray();
    }

    /// <summary>
    /// Generates all moves that the player whose turn it is can make
    /// </summary>
    /// <param name="generateSan">San notation needs to be generated</param>
    /// <returns>All generated moves</returns>
    public Move[] Moves(bool generateSan = true)
    {
        var moves = new List<Move>();

        for (short i = 0; i < 8; i++)
        {
            for (short j = 0; j < 8; j++)
            {
                if (pieces[i, j] != null)
                    moves.AddRange(Moves(new Position { Y = i, X = j }, generateSan));
            }
        }

        return moves.ToArray();
    }

    private static bool IsCheckmate(PieceColor side, ChessBoard board)
    {
        return IsKingChecked(side, board) && !PlayerHasMoves(side, board);
    }

    private static bool IsStalemate(PieceColor side, ChessBoard board)
    {
        return !IsKingChecked(side, board) && !PlayerHasMoves(side, board);
    }

    private static Position GetKingPosition(PieceColor side, ChessBoard board)
    {
        var kingPos = new Position();
        for (short i = 0; i < 8; i++)
        {
            for (short j = 0; j < 8; j++)
            {
                if (board.pieces[i, j]?.Color == side && board.pieces[i, j]?.Type == PieceType.King)
                {
                    kingPos = new Position() { Y = i, X = j, };
                    return kingPos;
                }
            }
        }
        return kingPos;
    }

    private static bool IsValidMove(Move move, ChessBoard board, bool invokeEvents, bool checkTurn)
    {
        if (move == null || !move.HasValue)
            throw new ArgumentNullException(nameof(move));

        if (board.pieces[move.OriginalPosition.Y, move.OriginalPosition.X] == null)
            throw new ChessPieceNotFoundException(board, move);

        if (checkTurn && board.pieces[move.OriginalPosition.Y, move.OriginalPosition.X].Color != board.Turn) return false;

        if (move.OriginalPosition == move.NewPosition) return false;

        move.Piece = board.pieces[move.OriginalPosition.Y, move.OriginalPosition.X];
        move.IsCheck = false;
        move.IsMate = false;
        move.CapturedPiece = null;
        move.Parameter = null;
        move.San = null;

        bool isValid = IsValidMove(move, board);
        bool isChecked = !isValid || IsKingCheckedValidation(move, move.Piece.Color, board);

        if (!isChecked)
        {
            // Capture
            if (board.pieces[move.NewPosition.Y, move.NewPosition.X] != null
             && board.pieces[move.NewPosition.Y, move.NewPosition.X].Color != move.Piece.Color)
                move.CapturedPiece = board.pieces[move.NewPosition.Y, move.NewPosition.X];

            // Promote
            if (invokeEvents && move.Parameter == MoveParameter.PawnPromotion)
            {
                var e = new PromotionEventArgs(board);
                board.OnPromotePawnEvent(e);
                if (e.Handled)
                    move.Parameter = e.PromotionResult;
            }

            // Check on opposite king
            move.IsCheck = IsKingCheckedValidation(move, move.Piece.Color.OppositeColor(), board);

            // Opposite king is in (stale)mate
            move.IsMate = !PlayerHasMovesValidation(move, move.Piece.Color.OppositeColor(), board);

            return true;
        }
        else
        {
            if (isValid && invokeEvents)
                board.OnInvalidMoveKingCheckedEvent(new CheckEventArgs(board, move.Piece.Color == PieceColor.White ? board.WhiteKing : board.BlackKing, true));
            return false;
        }
    }

    private static bool IsValidMove(Move move, ChessBoard board)
    {
        return (int)move.Piece.Type switch
        {
            var e when e == PieceType.Pawn => PawnValidation(move, board),
            var e when e == PieceType.Rook => RookValidation(move, board.pieces),
            var e when e == PieceType.Knight => KnightValidation(move, board.pieces),
            var e when e == PieceType.Bishop => BishopValidation(move, board.pieces),
            var e when e == PieceType.Queen => QueenValidation(move, board.pieces),
            var e when e == PieceType.King => KingValidation(move, board),
            _ => false
        };
    }

    /// <summary>
    /// Basically checking if after the move will been performed 
    /// the next move onto position of king is valid for one of pieces of opponent
    /// </summary>
    private static bool IsKingCheckedValidation(Move move, PieceColor side, ChessBoard board)
    {
        var fboard = new ChessBoard(board.pieces, board.PerformedMoves);

        // If checking for valid castle move
        // move.Piece == null only when calling recursively
        if (move.Piece != null && move.Piece.Color == side && (move.Parameter == MoveParameter.CastleKing || move.Parameter == MoveParameter.CastleQueen))
        {
            var kingPos = GetKingPosition(side, board);
            short step = (short)(move.Parameter == MoveParameter.CastleKing ? 1 : -1);

            for (short i = kingPos.X; i < 7 && i > 0; i += step)
                if (IsKingCheckedValidation(new(kingPos, new() { Y = kingPos.Y, X = i }), side, board))
                    return true;

            return false;
        }
        else if (move.OriginalPosition != move.NewPosition)
        {
            fboard.PerformedMoves.Add(move);
            fboard.DropPieceToNewPosition(move);
        }

        return IsKingChecked(side, fboard);
    }

    private static bool IsKingChecked(PieceColor side, ChessBoard board)
    {
        var kingPos = GetKingPosition(side, board);

        // move in Validation => King is being captured!
        if (!kingPos.HasValue)
            return false;

        for (short i = 0; i < 8; i++)
        {
            for (short j = 0; j < 8; j++)
            {
                if (board.pieces[i, j] != null && board.pieces[i, j].Color != side && !(kingPos.X == j && kingPos.Y == i))
                {
                    if (IsValidMove(new Move(new Position { Y = i, X = j }, kingPos) { Piece = board.pieces[i, j], }, board))
                        return true;
                }
            }
        }

        return false;
    }

    private static bool PlayerHasMovesValidation(Move move, PieceColor side, ChessBoard board)
    {
        var fboard = new ChessBoard(board.pieces, board.PerformedMoves);

        if (move.OriginalPosition != move.NewPosition)
        {
            fboard.PerformedMoves.Add(move);
            fboard.DropPieceToNewPosition(move);
        }
        return PlayerHasMoves(side, fboard);
    }

    private static bool PlayerHasMoves(PieceColor side, ChessBoard board)
    {
        for (short i = 0; i < 8; i++)
        {
            for (short j = 0; j < 8; j++)
            {
                if (board.pieces[i, j]?.Color == side)
                {
                    var position = new Position { Y = i, X = j };

                    for (short k = 0; k < 8; k++)
                    {
                        for (short l = 0; l < 8; l++)
                        {
                            if (position.X == l && position.Y == k) continue;

                            var move = new Move(position, new Position { Y = k, X = l }) { Piece = board.pieces[i, j] };

                            if (IsValidMove(move, board) && !IsKingCheckedValidation(move, side, board))
                                return true;
                        }
                    }
                }
            }
        }
        return false;
    }

    private static bool PawnValidation(Move move, ChessBoard board)
    {
        short v = (short)(move.NewPosition.Y - move.OriginalPosition.Y); // Vertical difference
        short h = (short)(move.NewPosition.X - move.OriginalPosition.X); // Horizontal difference

        short stepV = Math.Abs(v);
        short stepH = Math.Abs(h);

        // If moving forwards
        if ((move.Piece.Color == PieceColor.White && v > 0) || (move.Piece.Color == PieceColor.Black && v < 0))
        {
            // 1 step forward
            if (stepH == 0 && stepV == 1 && board.pieces[move.NewPosition.Y, move.NewPosition.X] == null)
            {
                ValidHandle();
                return true;
            }
            // 2 steps forward if in the beginning
            else if (stepH == 0 && stepV == 2
                && ((move.OriginalPosition.Y == 1 && board.pieces[2, move.NewPosition.X] == null && board.pieces[3, move.NewPosition.X] == null)
                 || (move.OriginalPosition.Y == 6 && board.pieces[5, move.NewPosition.X] == null && board.pieces[4, move.NewPosition.X] == null)))
            {
                return true;
            }
            // Second condition horizontal taking piece
            else if (stepV == 1 && stepH == 1
                  && board.pieces[move.NewPosition.Y, move.NewPosition.X] != null
                  && move.Piece.Color != board.pieces[move.NewPosition.Y, move.NewPosition.X].Color)
            {
                ValidHandle();
                return true;
            }
            // If En Passant => give it in parameters to replace later
            else if (IsValidEnPassant(move, board, v, h))
            {
                move.Parameter = MoveParameter.EnPassant;
                move.CapturedPiece = new Piece(move.Piece.Color.OppositeColor(), PieceType.Pawn);
                return true;
            }
        }
        return false;

        void ValidHandle()
        {
            // If Promoting pawn
            if (move.NewPosition.Y == 7 || move.NewPosition.Y == 0)
                move.Parameter = MoveParameter.PawnPromotion;
        }
    }

    private static bool QueenValidation(Move move, Piece?[,] pieces)
    {   // For queen just using validation of bishop OR rook
        return BishopValidation(move, pieces) || RookValidation(move, pieces);
    }

    private static bool RookValidation(Move move, Piece?[,] pieces)
    {
        var v = move.NewPosition.Y - move.OriginalPosition.Y; // Vertical difference
        var h = move.NewPosition.X - move.OriginalPosition.X; // Horizontal difference

        // if moving horizontally or vertically
        if (v == 0 || h == 0)
        {
            // Result is always 1 or -1
            var stepH = h != 0 ? Math.Abs(h) / h : 0;
            var stepV = v != 0 ? Math.Abs(v) / v : 0;

            for (int i = move.OriginalPosition.Y + stepV, j = move.OriginalPosition.X + stepH; Math.Abs(i - move.NewPosition.Y - (j - move.NewPosition.X)) >= 0; i += stepV, j += stepH)
            {
                if (pieces[i, j] != null || (i == move.NewPosition.Y && j == move.NewPosition.X))
                {
                    return i == move.NewPosition.Y && j == move.NewPosition.X && move.Piece.Color != pieces[i, j]?.Color;
                }
            }
            return false;
        }
        else return false;
    }

    private static bool KnightValidation(Move move, Piece?[,] pieces)
    {
        if ((Math.Abs(move.NewPosition.X - move.OriginalPosition.X) == 2 && Math.Abs(move.NewPosition.Y - move.OriginalPosition.Y) == 1)
         || (Math.Abs(move.NewPosition.X - move.OriginalPosition.X) == 1 && Math.Abs(move.NewPosition.Y - move.OriginalPosition.Y) == 2))
        {
            return pieces[move.NewPosition.Y, move.NewPosition.X]?.Color != move.Piece.Color;
        }
        else return false;
    }

    private static bool BishopValidation(Move move, Piece?[,] pieces)
    {
        var v = move.NewPosition.Y - move.OriginalPosition.Y; // Vertical difference
        var h = move.NewPosition.X - move.OriginalPosition.X; // Horizontal difference

        // If moving diagonal
        if (Math.Abs(v) == Math.Abs(h))
        {
            // Result is always 1 or -1
            var stepV = Math.Abs(v) / v;
            var stepH = Math.Abs(h) / h;

            for (int i = move.OriginalPosition.Y + stepV, j = move.OriginalPosition.X + stepH; Math.Abs(i - move.NewPosition.Y) >= 0; i += stepV, j += stepH)
            {
                if (pieces[i, j] != null || (i == move.NewPosition.Y && j == move.NewPosition.X))
                {
                    return i == move.NewPosition.Y && j == move.NewPosition.X && move.Piece.Color != pieces[i, j]?.Color;
                }
            }
            // This return will never be reached (in theory)
            return false;
        }
        else return false;
    }

    private static bool KingValidation(Move move, ChessBoard board)
    {
        if (Math.Abs(move.NewPosition.X - move.OriginalPosition.X) < 2 && Math.Abs(move.NewPosition.Y - move.OriginalPosition.Y) < 2)
        {
            return board.pieces[move.NewPosition.Y, move.NewPosition.X]?.Color != move.Piece.Color;
        }
        // Castle
        else if (move.NewPosition.X % 7 == 0 && board.pieces[move.NewPosition.Y, move.NewPosition.X] != null)
        {
            if (board.pieces[move.NewPosition.Y, move.NewPosition.X].Type == PieceType.Rook && board.pieces[move.NewPosition.Y, move.NewPosition.X].Color == move.Piece.Color)
            {
                switch (move.Piece.Color)
                {
                    // White King
                    case var e when e.Equals(PieceColor.White):
                        // Queen Castle
                        if (move.NewPosition.X == 0 && HasRightToCastle(PieceColor.White, MoveParameter.CastleQueen, board))
                        {
                            if (board.pieces[0, 1] == null && board.pieces[0, 2] == null && board.pieces[0, 3] == null)
                            {
                                move.Parameter = MoveParameter.CastleQueen;
                                return true;
                            }
                        }
                        // King Castle
                        else if (move.NewPosition.X == 7 && HasRightToCastle(PieceColor.White, MoveParameter.CastleKing, board))
                        {
                            if (board.pieces[0, 5] == null && board.pieces[0, 6] == null)
                            {
                                move.Parameter = MoveParameter.CastleKing;
                                return true;
                            }
                        }
                        break;
                    // Black King
                    case var e when e.Equals(PieceColor.Black):
                        // Queen Castle
                        if (move.NewPosition.X == 0 && HasRightToCastle(PieceColor.Black, MoveParameter.CastleQueen, board))
                        {
                            if (board.pieces[7, 1] == null && board.pieces[7, 2] == null && board.pieces[7, 3] == null)
                            {
                                move.Parameter = MoveParameter.CastleQueen;
                                return true;
                            }
                        }
                        // King Castle
                        else if (move.NewPosition.X == 7 && HasRightToCastle(PieceColor.Black, MoveParameter.CastleKing, board))
                        {
                            if (board.pieces[7, 5] == null && board.pieces[7, 6] == null)
                            {
                                move.Parameter = MoveParameter.CastleKing;
                                return true;
                            }
                        }
                        break;
                }
            }
        }
        return false;
    }

    private static bool HasRightToCastle(PieceColor side, MoveParameter castleType, ChessBoard board)
    {
        var valid = false;

        if (board.LoadedFromFEN)
        {
            if (side == PieceColor.White)
            {
                if (castleType == MoveParameter.CastleKing)
                    valid = board.Fen.CastleWK;
                else if (castleType == MoveParameter.CastleQueen)
                    valid = board.Fen.CastleWQ;
            }
            else if (side == PieceColor.Black)
            {
                if (castleType == MoveParameter.CastleKing)
                    valid = board.Fen.CastleBK;
                else if (castleType == MoveParameter.CastleQueen)
                    valid = board.Fen.CastleBQ;
            }

            if (board.PerformedMoves.Count > 0 && valid)
                valid = ValidByMoves();
        }
        else
            valid = ValidByMoves();

        return valid;

        bool ValidByMoves()
        {
            Position kingpos = new(side == PieceColor.White ? "e1" : "e8");
            Position rookpos;

            if (castleType == MoveParameter.CastleKing)
                rookpos = new Position(side == PieceColor.White ? "h1" : "h8");
            else if (castleType == MoveParameter.CastleQueen)
                rookpos = new Position(side == PieceColor.White ? "a1" : "a8");
            else
                throw new ArgumentException("Invalid move parameter", nameof(castleType));

            return !PieceEverMoved(kingpos, board) && !PieceEverMoved(rookpos, board);
        }
    }

    private static bool PieceEverMoved(Position piecePos, ChessBoard board)
    {
        return board.PerformedMoves.Any(p => p.OriginalPosition == piecePos);
    }

    private static bool IsValidEnPassant(Move move, ChessBoard board, short v, short h)
    {
        if (Math.Abs(v) == 1 && Math.Abs(h) == 1)
        {
            var piece = board.pieces[move.NewPosition.Y - v, move.NewPosition.X];

            // if on given new (position.y => one back) is a pawn with opposite color
            if (piece != null && piece.Color != move.Piece.Color && piece.Type == PieceType.Pawn)
            {
                bool valid = false;

                if (board.LoadedFromFEN)
                    valid = board.Fen.EnPassant == move.NewPosition;

                if (board.PerformedMoves.Count > 0)
                {
                    var position = LastMoveEnPassantPosition(board);
                    valid = position == move.NewPosition;
                }

                return valid;
            }
        }
        return false;
    }

    private static Position LastMoveEnPassantPosition(ChessBoard board)
    {
        Position pos = new();

        var lastMove = board.PerformedMoves.LastOrDefault();

        if (lastMove != null && lastMove.Piece.Type == PieceType.Pawn && Math.Abs(lastMove.NewPosition.Y - lastMove.OriginalPosition.Y) == 2)
        {
            pos = new() { X = lastMove.NewPosition.X, Y = (short)((lastMove.NewPosition.Y + lastMove.OriginalPosition.Y) / 2) };
        }
        return pos;
    }
}