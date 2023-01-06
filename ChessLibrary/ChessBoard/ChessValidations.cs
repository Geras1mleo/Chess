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
    /// Checks if given move is valid for current pieces positions
    /// </summary>
    /// <param name="san">San move to be checked</param>
    /// <returns>Whether given move is valid</returns>
    public bool IsValidMove(string san)
    {
        return IsValidMove(ParseFromSan(san, false));
    }

    /// <summary>
    /// Checks if given move is valid for current pieces positions
    /// </summary>
    /// <param name="move">Move to be checked</param>
    /// <returns>Whether given move is valid</returns>
    public bool IsValidMove(Move move)
    {
        return IsValidMove(move, this, false, true);
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

    internal static bool IsValidMove(Move move, ChessBoard board, bool raise, bool checkTurn)
    {
        if (move is null || !move.HasValue)
            throw new ArgumentNullException(nameof(move));

        if (board.pieces[move.OriginalPosition.Y, move.OriginalPosition.X] is null)
            throw new ChessPieceNotFoundException(board, move.OriginalPosition);

        if (checkTurn && board.pieces[move.OriginalPosition.Y, move.OriginalPosition.X].Color != board.Turn) return false;

        if (move.OriginalPosition == move.NewPosition) return false;

        move.Piece = board.pieces[move.OriginalPosition.Y, move.OriginalPosition.X];
        move.IsCheck = false;
        move.IsMate = false;
        move.CapturedPiece = null;
        move.San = null;

        MovePromotion? promParams = null;
        // Promotion result can be already specified so we dont need to invoke event again to ask for prom result
        if (move.Parameter is MovePromotion p)
        {
            promParams = new MovePromotion(p.PromotionType);
        }

        move.Parameter = null;

        bool isValid = IsValidMove(move, board);
        // If is not valid => don't validate further
        bool isChecked = !isValid || IsKingCheckedValidation(move, move.Piece.Color, board);

        if (!isChecked)
        {
            // Capture
            if (board.pieces[move.NewPosition.Y, move.NewPosition.X] is not null
             && board.pieces[move.NewPosition.Y, move.NewPosition.X].Color != move.Piece.Color)
            {
                move.CapturedPiece = board.pieces[move.NewPosition.Y, move.NewPosition.X];
            }

            // Promote, Invoke event only when raises == true AND promotion parameters has been not specified yet
            if (move.Parameter is MovePromotion promotion)
            {
                if (promParams != null && promParams.PromotionType != PromotionType.Default)
                {
                    move.Parameter = promParams;
                }
                else if (raise)
                {
                    var args = new PromotionEventArgs(board);
                    board.OnPromotePawnEvent(args);
                    promotion.PromotionType = args.PromotionResult;
                }
            }

            // Check on opposite king
            move.IsCheck = IsKingCheckedValidation(move, move.Piece.Color.OppositeColor(), board);

            // Opposite king is in (stale)mate
            move.IsMate = !PlayerHasMovesValidation(move, move.Piece.Color.OppositeColor(), board);

            return true;
        }
        else
        {
            if (isValid && raise)
            {
                board.OnInvalidMoveKingCheckedEvent(new CheckEventArgs(board, move.Piece.Color == PieceColor.White ? board.WhiteKing : board.BlackKing, true));
            }
            return false;
        }
    }

    internal static bool IsValidMove(Move move, ChessBoard board)
    {
        return move.Piece.Type switch
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
    internal static bool IsKingCheckedValidation(Move move, PieceColor side, ChessBoard board)
    {
        var fboard = new ChessBoard(board.pieces, board.executedMoves);

        // If validating castle move
        if (move.Parameter is MoveCastle castle && move.Piece.Color == side
         && move.Piece is not null) // move.Piece is null only when calling recursively
        {
            var kingPos = GetKingPosition(side, board);
            short step = (short)(castle.CastleType == CastleType.King ? 1 : -1);

            for (short i = kingPos.X; i < 7 && i > 1; i += step)
                if (IsKingCheckedValidation(new(kingPos, new() { Y = kingPos.Y, X = i }), side, board))
                    return true;

            return false;
        }
        else if (move.OriginalPosition != move.NewPosition)
        {
            fboard.executedMoves.Add(move);
            fboard.DropPieceToNewPosition(new(move.ToString())); // todo optimize
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
                if (board.pieces[i, j] is not null && board.pieces[i, j].Color != side)
                {
                    if (kingPos.X == j && kingPos.Y == i) continue;

                    if (IsValidMove(new Move(new Position { Y = i, X = j }, kingPos) { Piece = board.pieces[i, j], }, board))
                        return true;
                }
            }
        }

        return false;
    }

    private static bool PlayerHasMovesValidation(Move move, PieceColor side, ChessBoard board)
    {
        var fboard = new ChessBoard(board.pieces, board.executedMoves);

        if (move.OriginalPosition != move.NewPosition)
        {
            fboard.executedMoves.Add(move);
            fboard.DropPieceToNewPosition(new(move.ToString()));
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
            if (stepH == 0 && stepV == 1
             && board.pieces[move.NewPosition.Y, move.NewPosition.X] is null)
            {
                ValidHandle();
                return true;
            }
            // 2 steps forward if in the beginning
            else if (stepH == 0 && stepV == 2
                && ((move.OriginalPosition.Y == 1 && board.pieces[2, move.NewPosition.X] is null && board.pieces[3, move.NewPosition.X] is null)
                 || (move.OriginalPosition.Y == 6 && board.pieces[5, move.NewPosition.X] is null && board.pieces[4, move.NewPosition.X] is null)))
            {
                return true;
            }
            // Second condition horizontal taking piece
            else if (stepV == 1 && stepH == 1
                  && board.pieces[move.NewPosition.Y, move.NewPosition.X] is not null
                  && move.Piece.Color != board.pieces[move.NewPosition.Y, move.NewPosition.X].Color)
            {
                ValidHandle();
                return true;
            }
            // If En Passant => pass to parameters
            else if (IsValidEnPassant(move, board, v, h))
            {
                move.Parameter = new MoveEnPassant()
                {
                    CapturedPawnPosition = new()
                    {
                        Y = (short)(move.NewPosition.Y - v),
                        X = move.NewPosition.X
                    }
                };

                move.CapturedPiece = new Piece(move.Piece.Color.OppositeColor(), PieceType.Pawn);

                return true;
            }
        }
        return false;

        void ValidHandle()
        {
            // If Promoting pawn
            if (move.NewPosition.Y == 7 || move.NewPosition.Y == 0)
            {
                move.Parameter = new MovePromotion(PromotionType.Default);
            }
        }
    }

    private static bool QueenValidation(Move move, Piece?[,] pieces)
    {
        // For queen just using validation of bishop OR rook
        return BishopValidation(move, pieces) || RookValidation(move, pieces);
    }

    private static bool RookValidation(Move move, Piece?[,] pieces)
    {
        var v = move.NewPosition.Y - move.OriginalPosition.Y; // Vertical difference
        var h = move.NewPosition.X - move.OriginalPosition.X; // Horizontal difference

        // if moving horizontally or vertically
        if (v == 0 || h == 0)
        {
            // These vars are always 1 or -1, one of them will stay 0
            var stepH = h != 0 ? Math.Abs(h) / h : 0;
            var stepV = v != 0 ? Math.Abs(v) / v : 0;

            // A bit too difficult for loop to explain
            for (int i = move.OriginalPosition.Y + stepV, j = move.OriginalPosition.X + stepH;
                Math.Abs(i - move.NewPosition.Y - (j - move.NewPosition.X)) >= 0;
                i += stepV, j += stepH)
            {
                if (pieces[i, j] is not null || (i == move.NewPosition.Y && j == move.NewPosition.X))
                {
                    return i == move.NewPosition.Y && j == move.NewPosition.X && move.Piece.Color != pieces[i, j]?.Color;
                }
            }
            // This return will never be reached (in theory)
            return false;
        }
        else return false;
    }

    private static bool KnightValidation(Move move, Piece?[,] pieces)
    {
        // New position must be with stepH = 1 and steV = 2 or vice versa
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
            // These vars are always 1 or -1
            var stepV = Math.Abs(v) / v;
            var stepH = Math.Abs(h) / h;

            // A bit too difficult for loop to explain
            for (int i = move.OriginalPosition.Y + stepV, j = move.OriginalPosition.X + stepH; Math.Abs(i - move.NewPosition.Y) >= 0; i += stepV, j += stepH)
            {
                if (pieces[i, j] is not null || (i == move.NewPosition.Y && j == move.NewPosition.X))
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
            // Piece(if exist) has different color than king
            return board.pieces[move.NewPosition.Y, move.NewPosition.X]?.Color != move.Piece.Color;
        }

        // Check if king is on begin pos
        if (move.OriginalPosition.X == 4 && move.OriginalPosition.Y % 7 == 0
         && move.OriginalPosition.Y == move.NewPosition.Y)
        {
            // if drop on rooks position to castle
            // OR drop on kings new position after castle
            if ((move.NewPosition.X % 7 == 0 && move.NewPosition.Y % 7 == 0)
             || (Math.Abs(move.NewPosition.X - move.OriginalPosition.X) == 2))
            {
                switch (move.Piece.Color)
                {
                    case var e when e.Equals(PieceColor.White):
                        // Queen Castle
                        if (move.NewPosition.X == 0 || move.NewPosition.X == 2)
                        {
                            if (!HasRightToCastle(PieceColor.White, CastleType.Queen, board))
                                return false;

                            if (board.pieces[0, 1] is null && board.pieces[0, 2] is null && board.pieces[0, 3] is null)
                            {
                                move.Parameter = new MoveCastle(CastleType.Queen);
                                return true;
                            }
                        }
                        // King Castle
                        else if (move.NewPosition.X == 7 || move.NewPosition.X == 6)
                        {
                            if (!HasRightToCastle(PieceColor.White, CastleType.King, board))
                                return false;

                            if (board.pieces[0, 5] is null && board.pieces[0, 6] is null)
                            {
                                move.Parameter = new MoveCastle(CastleType.King);
                                return true;
                            }
                        }
                        break;
                    case var e when e.Equals(PieceColor.Black):
                        // Queen Castle
                        if (move.NewPosition.X == 0 || move.NewPosition.X == 2)
                        {
                            if (!HasRightToCastle(PieceColor.Black, CastleType.Queen, board))
                                return false;

                            if (board.pieces[7, 1] is null && board.pieces[7, 2] is null && board.pieces[7, 3] is null)
                            {
                                move.Parameter = new MoveCastle(CastleType.Queen);
                                return true;
                            }
                        }
                        // King Castle
                        else if (move.NewPosition.X == 7 || move.NewPosition.X == 6)
                        {
                            if (!HasRightToCastle(PieceColor.Black, CastleType.King, board))
                                return false;

                            if (board.pieces[7, 5] is null && board.pieces[7, 6] is null)
                            {
                                move.Parameter = new MoveCastle(CastleType.King);
                                return true;
                            }
                        }
                        break;
                }
            }
        }
        return false;
    }

    internal static bool HasRightToCastle(PieceColor side, CastleType castleType, ChessBoard board)
    {
        var valid = false;

        if (board.LoadedFromFen)
        {
            if (side == PieceColor.White)
            {
                if (castleType == CastleType.King)
                    valid = board.FenBuilder.CastleWK;
                else if (castleType == CastleType.Queen)
                    valid = board.FenBuilder.CastleWQ;
            }
            else if (side == PieceColor.Black)
            {
                if (castleType == CastleType.King)
                    valid = board.FenBuilder.CastleBK;
                else if (castleType == CastleType.Queen)
                    valid = board.FenBuilder.CastleBQ;
            }

            if (board.moveIndex >= 0 && valid)
                valid = ValidByMoves();
        }
        else
            valid = ValidByMoves();

        return valid;

        bool ValidByMoves()
        {
            Position kingpos = new(side == PieceColor.White ? "e1" : "e8");

            var rookpos = castleType switch
            {
                CastleType.King => new Position(side == PieceColor.White ? "h1" : "h8"),
                CastleType.Queen => new Position(side == PieceColor.White ? "a1" : "a8"),
                _ => throw new ChessArgumentException(board, "Invalid Castle type parameter"),
            };

            return board.pieces[rookpos.Y, rookpos.X] is not null
                && board.pieces[rookpos.Y, rookpos.X].Type == PieceType.Rook
                && board.pieces[rookpos.Y, rookpos.X].Color == side
                && !PieceEverMoved(kingpos, board) && !PieceEverMoved(rookpos, board);
        }
    }

    private static bool PieceEverMoved(Position piecePos, ChessBoard board)
    {
        return board.DisplayedMoves.Any(p => p.OriginalPosition == piecePos);
    }

    private static bool IsValidEnPassant(Move move, ChessBoard board, short v, short h)
    {
        if (Math.Abs(v) == 1 && Math.Abs(h) == 1) // Capture attempt
        {
            var piece = board.pieces[move.NewPosition.Y - v, move.NewPosition.X];

            // if on given new (position.y => one back) is a pawn with opposite color
            if (piece is not null && piece.Color != move.Piece.Color && piece.Type == PieceType.Pawn)
            {
                return LastMoveEnPassantPosition(board) == move.NewPosition;
            }
        }
        return false;
    }

    /// <summary>
    /// Returns Valid(if not => X = -1,Y = -1) En passant move new position
    /// </summary>
    internal static Position LastMoveEnPassantPosition(ChessBoard board)
    {
        Position pos = new();

        if (board.moveIndex >= 0) // If there are moves made on board
        {
            var lastMove = board.DisplayedMoves.Last();

            if (lastMove.Piece.Type == PieceType.Pawn
             && Math.Abs(lastMove.NewPosition.Y - lastMove.OriginalPosition.Y) == 2) // If last move is a pawn moving 2 squares forward
            {
                pos = new() { X = lastMove.NewPosition.X, Y = (short)((lastMove.NewPosition.Y + lastMove.OriginalPosition.Y) / 2) };
            }
        }
        else if (board.LoadedFromFen)
        {
            pos = board.FenBuilder.EnPassant;
        }

        return pos;
    }
}