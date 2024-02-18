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
    /// Generate all moves that the player can make
    /// </summary>
    /// <param name="allowAmbiguousCastle">Whether Castle move will be e1-g1 AND also e1-h1 which is in fact the same O-O</param>
    /// <param name="generateSan">San notation needs to be generated</param>
    /// <returns>All generated moves</returns>
    public Move[] Moves(bool allowAmbiguousCastle = false, bool generateSan = true)
    {
        var moves = new ConcurrentBag<Move>();
        var tasks = new List<Task>();

        for (short i = 0; i < MAX_ROWS; i++)
        for (short j = 0; j < MAX_COLS; j++)
        {
            if (pieces[i, j] != null)
            {
                tasks.Add(MoveGenerationTask(allowAmbiguousCastle, generateSan, j, i, moves));
            }
        }

        Task.WaitAll(tasks.ToArray());
        return moves.ToArray();
    }

    private Task MoveGenerationTask(bool allowAmbiguousCastle, bool generateSan, short x, short y, ConcurrentBag<Move> moves)
    {
        return Task.Run(() =>
        {
            var generatedMoves = Moves(new Position { Y = y, X = x }, allowAmbiguousCastle, generateSan);
            foreach (var move in generatedMoves)
            {
                moves.Add(move);
            }
        });
    }

    /// <summary>
    /// Returns all moves that the piece on given position can perform
    /// </summary>
    /// <param name="piecePosition">Position of piece</param>
    /// <param name="allowAmbiguousCastle">Whether Castle move will be e1-g1 AND also e1-h1 which is in fact the same O-O</param>
    /// <param name="generateSan">Whether SAN notation needs to be generated. For higher productivity => set to false</param>
    /// <returns>All available moves for given piece</returns>
    public Move[] Moves(Position piecePosition, bool allowAmbiguousCastle = false, bool generateSan = true)
    {
        if (pieces[piecePosition.Y, piecePosition.X] is null)
            throw new ChessPieceNotFoundException(this, piecePosition);

        var moves = new List<Move>();
        var positions = GeneratePositions(piecePosition, this);

        foreach (var position in positions)
        {
            Move move = new(piecePosition, position);

            if (!IsValidMove(move, this, false, true))
            {
                continue;
            }

            // Ambiguous castle
            if (!allowAmbiguousCastle && move.Parameter is MoveCastle)
            {
                if (move.NewPosition.X % 7 == 0) // Dropping king on position of rook 
                    continue;

                if (moves.Exists(m => m.OriginalPosition == move.OriginalPosition && m.NewPosition == move.NewPosition))
                    continue;
            }

            // If promotion => 4 different moves for each promotion type
            if (move.Parameter is MovePromotion promotion)
            {
                AddPromotionMoves(moves, move, generateSan, promotion.PromotionType);
            }
            else
            {
                moves.Add(move);

                if (generateSan)
                {
                    ParseToSan(move);
                }
            }
        }

        return moves.ToArray();
    }

    private void AddPromotionMoves(List<Move> moves, Move move, bool generateSan, PromotionType skipPromotion)
    {
        if (skipPromotion == PromotionType.Default)
            skipPromotion = PromotionType.ToQueen;

        moves.Add(new Move(move, skipPromotion));
        if (generateSan)
        {
            ParseToSan(moves[^1]);
        }

        // IsCheck and IsMate depends on promotion type so we have to reset those properties for each promotion type
        var promotions = new List<PromotionType>
        {
            PromotionType.ToQueen,
            PromotionType.ToRook,
            PromotionType.ToBishop,
            PromotionType.ToKnight
        };
        promotions.Remove(skipPromotion);

        foreach (var promotion in promotions)
        {
            var newMove = new Move(move, promotion);

            newMove.IsCheck = IsKingCheckedValidation(newMove, move.Piece.Color.OppositeColor(), this);
            newMove.IsMate = !PlayerHasMovesValidation(newMove, move.Piece.Color.OppositeColor(), this);

            moves.Add(newMove);

            if (generateSan)
            {
                ParseToSan(newMove);
            }
        }
    }

    /// <summary>
    /// Generating potential positions for given piece<br/>
    /// (!) Method doesn't takes in account validation for king (may be checked after making move with returned position)
    /// </summary>
    /// <param name="piecePosition">Position of piece</param>
    /// <returns>Potential positions</returns>
    public Position[] GeneratePositions(Position piecePosition)
    {
        if (pieces[piecePosition.Y, piecePosition.X] is null)
            throw new ChessPieceNotFoundException(this, piecePosition);

        return GeneratePositions(piecePosition, this);
    }

    private static Position[] GeneratePositions(Position piecePosition, ChessBoard board)
    {
        var positions = new List<Position>();

        switch (board[piecePosition]!.Type)
        {
            case var e when e == PieceType.Pawn:
                GeneratePawnPositions(piecePosition, board, positions);
                break;
            case var e when e == PieceType.Rook:
                GenerateRookPositions(piecePosition, board, positions);
                break;
            case var e when e == PieceType.Knight:
                GenerateKnightPositions(piecePosition, board, positions);
                break;
            case var e when e == PieceType.Bishop:
                GenerateBishopPositions(piecePosition, board, positions);
                break;
            case var e when e == PieceType.Queen:
                GenerateRookPositions(piecePosition, board, positions);
                GenerateBishopPositions(piecePosition, board, positions);
                break;
            case var e when e == PieceType.King:
                GenerateKingPositions(piecePosition, board, positions);
                break;
        }

        return positions.ToArray();
    }

    private static void GenerateKingPositions(Position piecePosition, ChessBoard board, List<Position> positions)
    {
        int fromX = Math.Max(0, piecePosition.X - 1);
        int toX = Math.Min(7, piecePosition.X + 1);
        int fromY = Math.Max(0, piecePosition.Y - 1);
        int toY = Math.Min(7, piecePosition.Y + 1);

        for (int x = fromX; x <= toX; x++)
        for (int y = fromY; y <= toY; y++)
            if (x != piecePosition.X || y != piecePosition.Y)
                if (board[x, y] == null || board[x, y]!.Color != board[piecePosition]!.Color)
                    positions.Add(new Position((short)x, (short)y));

        // Castle options
        if (piecePosition.Y % 7 == 0 && piecePosition.X == 4)
        {
            var rook = board[0, piecePosition.Y];

            if (board[1, piecePosition.Y] is null && board[2, piecePosition.Y] is null && board[3, piecePosition.Y] is null)
                if (rook?.Type == PieceType.Rook && rook.Color == board[piecePosition]!.Color)
                {
                    positions.Add(new Position() { X = 2, Y = piecePosition.Y });
                    if (!board.StandardiseCastlingPositions)
                        positions.Add(new Position() { X = 0, Y = piecePosition.Y });
                }

            rook = board[7, piecePosition.Y];

            if (board[5, piecePosition.Y] is null && board[6, piecePosition.Y] is null)
                if (rook?.Type == PieceType.Rook && rook.Color == board[piecePosition]!.Color)
                {
                    positions.Add(new Position() { X = 6, Y = piecePosition.Y });
                    if (!board.StandardiseCastlingPositions)
                        positions.Add(new Position() { X = 7, Y = piecePosition.Y });
                }
        }
    }

    private static void GenerateKnightPositions(Position piecePosition, ChessBoard board, List<Position> positions)
    {
        short x = piecePosition.X;
        short y = piecePosition.Y;
        Position[] possiblePositions =
        {
            new((short)(x + 2), (short)(y + 1)),
            new((short)(x + 2), (short)(y - 1)),
            new((short)(x - 2), (short)(y + 1)),
            new((short)(x - 2), (short)(y - 1)),
            new((short)(x + 1), (short)(y + 2)),
            new((short)(x + 1), (short)(y - 2)),
            new((short)(x - 1), (short)(y + 2)),
            new((short)(x - 1), (short)(y - 2))
        };

        foreach (var pos in possiblePositions)
        {
            if (pos.X >= 0 && pos.X < 8 && pos.Y >= 0 && pos.Y < 8)
            {
                if (board[pos] is null || board[pos]!.Color != board[piecePosition]!.Color)
                {
                    positions.Add(pos);
                }
            }
        }
    }


    private static void GeneratePawnPositions(Position piecePosition, ChessBoard board, List<Position> positions)
    {
        short step = (short)(board[piecePosition]!.Color == PieceColor.White ? 1 : -1);

        var x = piecePosition.X;
        var y = piecePosition.Y;

        short nextY = (short)(y + step);

        if (board[x, nextY] is null)
            positions.Add(new Position() { X = x, Y = nextY });

        short rightX = (short)(x + 1);
        if (rightX < 8)
        {
            if (board[rightX, nextY] is not null && board[rightX, nextY].Color != board[piecePosition].Color)
                positions.Add(new Position() { X = rightX, Y = nextY });

            else if (IsValidEnPassant(new Move(piecePosition, new Position() { Y = nextY, X = rightX }) { Piece = board[piecePosition] }, board, step, 1))
                positions.Add(new Position() { X = rightX, Y = nextY });
        }

        short leftX = (short)(x - 1);
        if (leftX > -1)
        {
            if (board[leftX, nextY] is not null && board[leftX, nextY].Color != board[piecePosition].Color)
                positions.Add(new Position() { X = leftX, Y = nextY });

            else if (IsValidEnPassant(new Move(piecePosition, new Position() { Y = nextY, X = leftX }) { Piece = board[piecePosition] }, board, step, -1))
                positions.Add(new Position() { X = leftX, Y = nextY });
        }

        // 2 forward
        if ((y == 1 && board[piecePosition].Color == PieceColor.White || y == 6 && board[piecePosition].Color == PieceColor.Black)
         && board[x, nextY] is null
         && board[x, (short)(y + step * 2)] is null)
            positions.Add(new Position() { X = x, Y = (short)(y + step * 2) });
    }

    // Directions for which the bishop can move
    private static readonly List<(short x, short y)> BishopDirections = new() { (1, 1), (1, -1), (-1, 1), (-1, -1) };

    private static void GenerateBishopPositions(Position piecePosition, ChessBoard board, List<Position> positions)
    {
        AddPositionsInDirections(BishopDirections, piecePosition, board, positions);
    }

    // Directions for which the rook can move
    private static readonly List<(short x, short y)> RookDirections = new() { (0, 1), (1, 0), (0, -1), (-1, 0) };

    private static void GenerateRookPositions(Position piecePosition, ChessBoard board, List<Position> positions)
    {
        AddPositionsInDirections(RookDirections, piecePosition, board, positions);
    }

    private static void AddPositionsInDirections(List<(short x, short y)> directions, Position piecePosition, ChessBoard board, List<Position> positions)
    {
        foreach (var direction in directions)
        {
            var currentPosition = (x: (short)(piecePosition.X + direction.x), y: (short)(piecePosition.Y + direction.y));

            // Loop until the end of the board is reached or a piece is encountered
            while (currentPosition.y >= 0 && currentPosition.y < MAX_ROWS && currentPosition.x >= 0 && currentPosition.x < MAX_COLS)
            {
                if (board.pieces[currentPosition.y, currentPosition.x] != null)
                {
                    // If the current piece is not of the same color as the original piece, add it to the list of positions
                    if (board.pieces[currentPosition.y, currentPosition.x]!.Color != board[piecePosition]!.Color)
                        positions.Add(new Position() { X = currentPosition.x, Y = currentPosition.y });

                    break;
                }

                positions.Add(new Position() { X = currentPosition.x, Y = currentPosition.y });

                currentPosition.y += direction.y;
                currentPosition.x += direction.x;
            }
        }
    }
}