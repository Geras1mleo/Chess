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
        var (succeeded, exception) = SanBuilder.TryParse(this, san, out var move, false);

        if (!succeeded && exception is not null)
            return false;

        return IsValidMove(move!);
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

    // TODO: populateProperties = false/true
    internal static bool IsValidMove(Move move, ChessBoard board, bool raise, bool checkTurn)
    {
        if (move is null || !move.HasValue)
            throw new ArgumentNullException(nameof(move));

        var piece = board.pieces[move.OriginalPosition.Y, move.OriginalPosition.X];

        if (piece == null)
            throw new ChessPieceNotFoundException(board, move.OriginalPosition);

        if (checkTurn && piece.Color != board.Turn)
            return false;
        if (move.OriginalPosition == move.NewPosition)
            return false;

        MovePromotion? promParams = null;
        // Promotion result can be already specified so we dont need to invoke event again to ask for prom result
        if (move.Parameter is MovePromotion p)
            promParams = new MovePromotion(p.PromotionType);

        ResetMoveProperties(move, piece);

        bool isValid = IsValidMove(move, board);
        // If is not valid => don't validate further
        bool isChecked = !isValid || IsKingCheckedValidation(move, move.Piece.Color, board);

        if (!isChecked)
        {
            ValidMoveSetProperties(move, board, raise, promParams);

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

    private static void ResetMoveProperties(Move move, Piece piece)
    {
        move.Piece = piece;
        move.IsCheck = false;
        move.IsMate = false;
        move.CapturedPiece = null;
        move.San = null;
        move.Parameter = null;
    }

    private static void ValidMoveSetProperties(Move move, ChessBoard board, bool raise, MovePromotion? promParams)
    {
        // If capturing some pieces
        var capturedPiece = board.pieces[move.NewPosition.Y, move.NewPosition.X];
        if (capturedPiece != null && capturedPiece.Color != move.Piece.Color)
        {
            move.CapturedPiece = capturedPiece;
        }

        // Promotion: Invoke event only when raises == true AND promotion parameters has been not specified yet
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

        move.IsCheck = IsKingCheckedValidation(move, move.Piece.Color.OppositeColor(), board); // Whether there is a check on opposite king
        move.IsMate = !PlayerHasMovesValidation(move, move.Piece.Color.OppositeColor(), board); // Whether the opposite king is in (stale)mate
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
    /// Basically checking if after the move has been executed
    /// the next move onto position of king is valid for one of pieces of opponent
    /// </summary>
    internal static bool IsKingCheckedValidation(Move move, PieceColor side, ChessBoard board)
    {
        var newBoard = new ChessBoard(board.pieces, board.executedMoves);

        // If validating castle move
        if (move.Parameter is MoveCastle castle && move.Piece.Color == side && move.Piece is not null)
            return IsKingCheckedWhileCastling(side, board, castle);

        if (move.Parameter is MoveEnPassant enPassant)
            newBoard.Remove(enPassant.CapturedPawnPosition);

        if (move.OriginalPosition == move.NewPosition)
            return IsKingChecked(side, newBoard);

        newBoard.executedMoves.Add(move);
        newBoard.DropPieceToNewPosition(new Move(move));

        return IsKingChecked(side, newBoard);
    }

    private static bool IsKingCheckedWhileCastling(PieceColor side, ChessBoard board, MoveCastle castle)
    {
        bool isCheck = false;
        var kingPos = GetKingPosition(side, board);
        short step = (short)(castle.CastleType == CastleType.King ? 1 : -1);

        short i = kingPos.X;
        while (i < MAX_COLS - 1 && i > 1 && !isCheck)
        {
            isCheck = IsKingCheckedValidation(new Move(kingPos, new Position { Y = kingPos.Y, X = i }), side, board);
            i += step;
        }

        return isCheck;
    }

    private static bool IsKingChecked(PieceColor side, ChessBoard board)
    {
        var kingPos = GetKingPosition(side, board);

        // move in Validation => King is being captured!
        if (!kingPos.HasValue)
            return false;

        for (short i = 0; i < MAX_ROWS; i++)
        {
            for (short j = 0; j < MAX_COLS; j++)
            {
                var piece = board.pieces[i, j];
                if (piece == null || piece.Color == side)
                    continue;
                if (kingPos.X == j && kingPos.Y == i)
                    continue;

                if (IsValidMove(new Move(new Position { Y = i, X = j }, kingPos) { Piece = piece, }, board))
                    return true;
            }
        }

        return false;
    }

    private static bool PlayerHasMovesValidation(Move move, PieceColor side, ChessBoard board)
    {
        var newBoard = new ChessBoard(board.pieces, board.executedMoves);

        if (move.OriginalPosition != move.NewPosition)
        {
            newBoard.executedMoves.Add(move);
            newBoard.DropPieceToNewPosition(new Move(move));
        }
        return PlayerHasMoves(side, newBoard);
    }

    internal static bool PlayerHasMoves(PieceColor side, ChessBoard board)
    {
        for (short i = 0; i < 8; i++)
        {
            for (short j = 0; j < 8; j++)
            {
                var piece = board.pieces[i, j];
                if (piece == null || piece.Color != side)
                    continue;

                var fromPosition = new Position { Y = i, X = j };

                foreach (var position in GeneratePositions(fromPosition, board))
                {
                    var move = new Move(fromPosition, position) { Piece = piece };

                    if (piece.Type == PieceType.King)
                        KingValidation(move, board); // Needed to specify castling options that are required in the IsKingCheckedValidation

                    if (!IsKingCheckedValidation(move, side, board))
                        return true;
                }
            }
        }

        return false;
    }

    private static bool PawnValidation(Move move, ChessBoard board)
    {
        bool isValid = false;

        short verticalDifference = (short)(move.NewPosition.Y - move.OriginalPosition.Y);
        short horizontalDifference = (short)(move.NewPosition.X - move.OriginalPosition.X);

        short verticalStep = Math.Abs(verticalDifference);
        short horizontalStep = Math.Abs(horizontalDifference);

        PieceColor pieceColor = move.Piece.Color;

        // If the pawn is moving forward
        if ((pieceColor == PieceColor.White && verticalDifference > 0) || (pieceColor == PieceColor.Black && verticalDifference < 0))
        {
            // Check if moving 1 step forward
            if (horizontalStep == 0 && verticalStep == 1 && board.pieces[move.NewPosition.Y, move.NewPosition.X] == null)
            {
                HandlePotentialPromotion(move);
                isValid = true;
            }
            // Check if moving 2 steps forward at the beginning
            else if (horizontalStep == 0 && verticalStep == 2
                                         && ((move.OriginalPosition.Y == 1 && board.pieces[2, move.NewPosition.X] == null &&
                                              board.pieces[3, move.NewPosition.X] == null)
                                          || (move.OriginalPosition.Y == 6 && board.pieces[5, move.NewPosition.X] == null &&
                                              board.pieces[4, move.NewPosition.X] == null)))
            {
                isValid = true;
            }
            // Check if taking piece of opponent
            else if (verticalStep == 1 && horizontalStep == 1
                                       && board.pieces[move.NewPosition.Y, move.NewPosition.X] != null
                                       && pieceColor != board.pieces[move.NewPosition.Y, move.NewPosition.X].Color)
            {
                HandlePotentialPromotion(move);
                isValid = true;
            }
            // Check if the move is valid en passant
            else if (IsValidEnPassant(move, board, verticalDifference, horizontalDifference))
            {
                HandleEnPassant(move, verticalDifference, pieceColor);
                isValid = true;
            }
        }

        return isValid;
    }

    private static void HandlePotentialPromotion(Move move)
    {
        if (move.NewPosition.Y % (MAX_ROWS - 1) == 0)
        {
            move.Parameter = new MovePromotion(PromotionType.Default);
        }
    }

    private static void HandleEnPassant(Move move, short verticalDifference, PieceColor pieceColor)
    {
        move.Parameter = new MoveEnPassant()
        {
            CapturedPawnPosition = new Position()
            {
                Y = (short)(move.NewPosition.Y - verticalDifference),
                X = move.NewPosition.X
            }
        };
        move.CapturedPiece = new Piece(pieceColor.OppositeColor(), PieceType.Pawn);
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

    private static bool QueenValidation(Move move, Piece?[,] pieces)
    {
        // For queen just using validation of bishop OR rook
        return BishopValidation(move, pieces) || RookValidation(move, pieces);
    }

    private static bool RookValidation(Move move, Piece?[,] pieces)
    {
        int verticalDiff = move.NewPosition.Y - move.OriginalPosition.Y;
        int horizontalDiff = move.NewPosition.X - move.OriginalPosition.X;

        // If not moving straight
        if (verticalDiff != 0 && horizontalDiff != 0)
            return false;

        int stepVertical = Math.Sign(verticalDiff);
        int stepHorizontal = Math.Sign(horizontalDiff);

        int i = move.OriginalPosition.Y + stepVertical;
        int j = move.OriginalPosition.X + stepHorizontal;

        while (i != move.NewPosition.Y || j != move.NewPosition.X)
        {
            if (pieces[i, j] != null)
                return false;

            i += stepVertical;
            j += stepHorizontal;
        }

        return pieces[i, j]?.Color != move.Piece.Color;
    }

    private static bool KnightValidation(Move move, Piece?[,] pieces)
    {
        int verticalDiff = Math.Abs(move.NewPosition.X - move.OriginalPosition.X);
        int horizontalDiff = Math.Abs(move.NewPosition.Y - move.OriginalPosition.Y);

        // Check if the move is in a L-shape: 2 steps horizontally and 1 step vertically, or vice versa
        if ((verticalDiff == 2 && horizontalDiff == 1) || (verticalDiff == 1 && horizontalDiff == 2))
            return pieces[move.NewPosition.Y, move.NewPosition.X]?.Color != move.Piece.Color;

        return false;
    }

    private static bool BishopValidation(Move move, Piece?[,] pieces)
    {
        var verticalDiff = move.NewPosition.Y - move.OriginalPosition.Y;
        var horizontalDiff = move.NewPosition.X - move.OriginalPosition.X;

        // If not moving diagonal
        if (Math.Abs(verticalDiff) != Math.Abs(horizontalDiff))
            return false;

        var stepVertical = Math.Sign(verticalDiff);
        var stepHorizontal = Math.Sign(horizontalDiff);

        int i = move.OriginalPosition.Y + stepVertical;
        int j = move.OriginalPosition.X + stepHorizontal;

        while (i != move.NewPosition.Y && j != move.NewPosition.X)
        {
            if (pieces[i, j] != null)
                return false;

            i += stepVertical;
            j += stepHorizontal;
        }

        return pieces[i, j]?.Color != move.Piece.Color;
    }

    private static bool KingValidation(Move move, ChessBoard board)
    {
        if (Math.Abs(move.NewPosition.X - move.OriginalPosition.X) < 2 && Math.Abs(move.NewPosition.Y - move.OriginalPosition.Y) < 2)
        {
            // Piece(if exist) has different color than king
            return board.pieces[move.NewPosition.Y, move.NewPosition.X]?.Color != move.Piece.Color;
        }

        // Castle validation:

        bool kingMovesHorizontally = move.OriginalPosition.Y == move.NewPosition.Y;
        bool kingOnBeginPos = move.OriginalPosition.X == 4 && move.OriginalPosition.Y % 7 == 0;

        if (!kingOnBeginPos || !kingMovesHorizontally)
            return false;

        bool kingMoves2Tiles = Math.Abs(move.NewPosition.X - move.OriginalPosition.X) == 2;
        bool kingMovesOnRook = move.NewPosition.X % 7 == 0;

        if (!kingMovesOnRook && !kingMoves2Tiles)
            return false;

        // Standardise x-pos for checks
        int x = kingMovesOnRook ? (move.NewPosition.X == 0 ? 2 : 6) : move.NewPosition.X;

        bool isKingSideCastle = x == 6;
        bool isQueenSideCastle = x == 2;

        MoveCastle moveCastle = isKingSideCastle ? new MoveCastle(CastleType.King) : new MoveCastle(CastleType.Queen);
        move.Parameter = moveCastle;

        int y = move.NewPosition.Y;

        bool hasObstacles = true;

        if (isQueenSideCastle)
            hasObstacles = board.pieces[y, 1] != null || board.pieces[y, 2] != null || board.pieces[y, 3] != null;
        else if (isKingSideCastle)
            hasObstacles = board.pieces[y, 5] != null || board.pieces[y, 6] != null;

        bool isValid = !hasObstacles && HasRightToCastle(move.Piece.Color, moveCastle.CastleType, board);

        // Standardise castling position
        if (board.StandardiseCastlingPositions && isValid && kingMovesOnRook)
            move.NewPosition = new Position((short)(move.NewPosition.X == 0 ? 2 : 6), move.NewPosition.Y);

        return isValid;
    }

    internal static bool HasRightToCastle(PieceColor side, CastleType castleType, ChessBoard board)
    {
        var valid = false;

        if (board.LoadedFromFen)
        {
            if (side == PieceColor.White)
            {
                valid = castleType switch
                {
                    CastleType.King => board.FenBuilder!.CastleWK,
                    CastleType.Queen => board.FenBuilder!.CastleWQ,
                    _ => valid
                };
            }
            else if (side == PieceColor.Black)
            {
                valid = castleType switch
                {
                    CastleType.King => board.FenBuilder!.CastleBK,
                    CastleType.Queen => board.FenBuilder!.CastleBQ,
                    _ => valid
                };
            }

            if (valid && board.moveIndex >= 0)
                valid = ValidByMoves();
        }
        else
            valid = ValidByMoves();

        return valid;

        bool ValidByMoves()
        {
            Position kingpos = new(4, (short)(side == PieceColor.White ? 0 : 7)); // "e1" : "e8"

            var rookpos = castleType switch
            {
                CastleType.King => new Position(7, (short)(side == PieceColor.White ? 0 : 7)), // "h1" : "h8"
                CastleType.Queen => new Position(0, (short)(side == PieceColor.White ? 0 : 7)), // "a1" : "a8"
                _ => throw new ChessArgumentException(board, "Invalid Castle type parameter"),
            };

            var rook = board.pieces[rookpos.Y, rookpos.X];
            return rook != null
                && rook.Type == PieceType.Rook
                && rook.Color == side
                && !PieceEverMoved(kingpos, board) && !PieceEverMoved(rookpos, board);
        }
    }

    private static bool PieceEverMoved(Position piecePos, ChessBoard board)
    {
        return board.DisplayedMoves.Any(p => p.OriginalPosition == piecePos);
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

            bool isPawn = lastMove.Piece.Type == PieceType.Pawn;
            bool moving2Tiles = Math.Abs(lastMove.NewPosition.Y - lastMove.OriginalPosition.Y) == 2;

            if (isPawn && moving2Tiles)
            {
                pos = new Position
                {
                    X = lastMove.NewPosition.X,
                    Y = (short)((lastMove.NewPosition.Y + lastMove.OriginalPosition.Y) / 2)
                };
            }
        }
        else if (board.LoadedFromFen)
        {
            pos = board.FenBuilder!.EnPassant;
        }

        return pos;
    }
}