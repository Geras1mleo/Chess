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
        Move move;

        var positions = GeneratePositions(piecePosition, this);

        for (int i = 0; i < positions.Length; i++)
        {
            move = new(piecePosition, positions[i]) { Piece = pieces[piecePosition.Y, piecePosition.X] };

            if (IsValidMove(move, this, false, true))
            {
                // Ambiguous castle
                if (!allowAmbiguousCastle && move.Parameter is MoveCastle)
                {
                    if (move.NewPosition.X % 7 == 0) // Dropping king on position of rook
                        continue;
                }

                // If promotion => 4 different moves for each promotion type
                if (move.Parameter is MovePromotion promotion)
                {
                    moves.Add(new Move(move, PromotionType.ToQueen));
                    moves.Add(new Move(move, PromotionType.ToRook));
                    moves.Add(new Move(move, PromotionType.ToBishop));
                    moves.Add(new Move(move, PromotionType.ToKnight));
                }
                else
                    moves.Add(move);

                if (generateSan)
                    ParseToSan(move);
            }
        }

        return moves.ToArray();
    }

    /// <summary>
    /// Generates all moves that the player whose turn it is can make
    /// </summary>
    /// <param name="allowAmbiguousCastle">Whether Castle move will be e1-g1 AND also e1-h1 which is in fact the same O-O</param>
    /// <param name="generateSan">San notation needs to be generated</param>
    /// <returns>All generated moves</returns>
    public Move[] Moves(bool allowAmbiguousCastle = false, bool generateSan = true)
    {
        var moves = new ConcurrentBag<Move>();
        var tasks = new List<Task>();

        for (short i = 0; i < 8; i++)
        {
            for (short j = 0; j < 8; j++)
            {
                if (pieces[i, j] is null)
                    continue;

                short x = j;
                short y = i;

                tasks.Add(Task.Run(() =>
                {
                    foreach (var move in Moves(new Position { Y = y, X = x }, allowAmbiguousCastle, generateSan))
                    {
                        moves.Add(move);
                    }
                }));
            }
        }

        Task.WaitAll(tasks.ToArray());

        return moves.ToArray();
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

        switch (board[piecePosition].Type)
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
        if (piecePosition.Y + 1 < 8)
        {
            positions.Add(new Position() { X = piecePosition.X, Y = (short)(piecePosition.Y + 1) });

            if (piecePosition.X + 1 < 8)
                positions.Add(new Position() { X = (short)(piecePosition.X + 1), Y = (short)(piecePosition.Y + 1) });

            if (piecePosition.X - 1 > -1)
                positions.Add(new Position() { X = (short)(piecePosition.X - 1), Y = (short)(piecePosition.Y + 1) });
        }
        if (piecePosition.Y - 1 > -1)
        {
            positions.Add(new Position() { X = piecePosition.X, Y = (short)(piecePosition.Y - 1) });

            if (piecePosition.X + 1 < 8)
                positions.Add(new Position() { X = (short)(piecePosition.X + 1), Y = (short)(piecePosition.Y - 1) });

            if (piecePosition.X - 1 > -1)
                positions.Add(new Position() { X = (short)(piecePosition.X - 1), Y = (short)(piecePosition.Y - 1) });
        }
        if (piecePosition.X + 1 < 8)
        {
            positions.Add(new Position() { X = (short)(piecePosition.X + 1), Y = piecePosition.Y });
        }
        if (piecePosition.X - 1 > -1)
        {
            positions.Add(new Position() { X = (short)(piecePosition.X - 1), Y = piecePosition.Y });
        }

        positions.RemoveAll(p => !(board[p] is null || board[p].Color != board[piecePosition].Color));

        if (piecePosition.Y % 7 == 0 && piecePosition.X == 4)
        {
            // Castle options

            var piece = board[new Position() { X = 0, Y = piecePosition.Y }];

            if (board[1, piecePosition.Y] is null && board[2, piecePosition.Y] is null && board[3, piecePosition.Y] is null)
                if (piece?.Type == PieceType.Rook && piece.Color == board[piecePosition].Color)
                {
                    positions.Add(new Position() { X = 0, Y = piecePosition.Y });
                    positions.Add(new Position() { X = 2, Y = piecePosition.Y });
                }

            piece = board[new Position() { X = 7, Y = piecePosition.Y }];

            if (board[5, piecePosition.Y] is null && board[6, piecePosition.Y] is null)
                if (piece?.Type == PieceType.Rook && piece.Color == board[piecePosition].Color)
                {
                    positions.Add(new Position() { X = 6, Y = piecePosition.Y });
                    positions.Add(new Position() { X = 7, Y = piecePosition.Y });
                }
        }
    }

    private static void GenerateKnightPositions(Position piecePosition, ChessBoard board, List<Position> positions)
    {
        if (piecePosition.X + 2 < 8)
        {
            if (piecePosition.Y + 1 < 8)
                positions.Add(new Position() { X = (short)(piecePosition.X + 2), Y = (short)(piecePosition.Y + 1) });

            if (piecePosition.Y - 1 > -1)
                positions.Add(new Position() { X = (short)(piecePosition.X + 2), Y = (short)(piecePosition.Y - 1) });
        }
        if (piecePosition.X - 2 > -1)
        {
            if (piecePosition.Y + 1 < 8)
                positions.Add(new Position() { X = (short)(piecePosition.X - 2), Y = (short)(piecePosition.Y + 1) });

            if (piecePosition.Y - 1 > -1)
                positions.Add(new Position() { X = (short)(piecePosition.X - 2), Y = (short)(piecePosition.Y - 1) });
        }
        if (piecePosition.X + 1 < 8)
        {
            if (piecePosition.Y + 2 < 8)
                positions.Add(new Position() { X = (short)(piecePosition.X + 1), Y = (short)(piecePosition.Y + 2) });

            if (piecePosition.Y - 2 > -1)
                positions.Add(new Position() { X = (short)(piecePosition.X + 1), Y = (short)(piecePosition.Y - 2) });
        }
        if (piecePosition.X - 1 > -1)
        {
            if (piecePosition.Y + 2 < 8)
                positions.Add(new Position() { X = (short)(piecePosition.X - 1), Y = (short)(piecePosition.Y + 2) });

            if (piecePosition.Y - 2 > -1)
                positions.Add(new Position() { X = (short)(piecePosition.X - 1), Y = (short)(piecePosition.Y - 2) });
        }

        positions.RemoveAll(p => !(board[p] is null || board[p].Color != board[piecePosition].Color));
    }

    private static void GeneratePawnPositions(Position piecePosition, ChessBoard board, List<Position> positions)
    {
        short step = (short)(board[piecePosition].Color == PieceColor.White ? 1 : -1);

        if (board[piecePosition.X, (short)(piecePosition.Y + step)] is null)
            positions.Add(new Position() { X = piecePosition.X, Y = (short)(piecePosition.Y + step) });

        if (piecePosition.X + 1 < 8)
            if (board[(short)(piecePosition.X + 1), (short)(piecePosition.Y + step)] is not null
             && board[(short)(piecePosition.X + 1), (short)(piecePosition.Y + step)].Color != board[piecePosition].Color)
                positions.Add(new Position() { X = (short)(piecePosition.X + 1), Y = (short)(piecePosition.Y + step) });

            else if (IsValidEnPassant(new Move(piecePosition, new Position() { Y = (short)(piecePosition.Y + step), X = (short)(piecePosition.X + 1) }) { Piece = board[piecePosition] }, board, step, 1))
                positions.Add(new Position() { X = (short)(piecePosition.X + 1), Y = (short)(piecePosition.Y + step) });

        if (piecePosition.X - 1 > -1)
            if (board[(short)(piecePosition.X - 1), (short)(piecePosition.Y + step)] is not null
             && board[(short)(piecePosition.X - 1), (short)(piecePosition.Y + step)].Color != board[piecePosition].Color)
                positions.Add(new Position() { X = (short)(piecePosition.X - 1), Y = (short)(piecePosition.Y + step) });

            else if (IsValidEnPassant(new Move(piecePosition, new Position() { Y = (short)(piecePosition.Y + step), X = (short)(piecePosition.X - 1) }) { Piece = board[piecePosition] }, board, step, -1))
                positions.Add(new Position() { X = (short)(piecePosition.X - 1), Y = (short)(piecePosition.Y + step) });


        // 2 forward
        if (((piecePosition.Y == 1 && board[piecePosition].Color == PieceColor.White) || (piecePosition.Y == 6 && board[piecePosition].Color == PieceColor.Black))
            && board[piecePosition.X, (short)(piecePosition.Y + step)] is null
            && board[piecePosition.X, (short)(piecePosition.Y + (step * 2))] is null)
            positions.Add(new Position() { X = piecePosition.X, Y = (short)(piecePosition.Y + (step * 2)) });
    }

    private static void GenerateBishopPositions(Position piecePosition, ChessBoard board, List<Position> positions)
    {
        for (short i = (short)(piecePosition.Y + 1), j = (short)(piecePosition.X + 1); i < 8 && j < 8; i++, j++)
        {
            if (board[j, i] is not null)
            {
                if (board[j, i].Color != board[piecePosition].Color)
                    positions.Add(new Position() { X = j, Y = i });

                break;
            }

            positions.Add(new Position() { X = j, Y = i });
        }
        for (short i = (short)(piecePosition.Y - 1), j = (short)(piecePosition.X + 1); i > -1 && j < 8; i--, j++)
        {
            if (board[j, i] is not null)
            {
                if (board[j, i].Color != board[piecePosition].Color)
                    positions.Add(new Position() { X = j, Y = i });

                break;
            }

            positions.Add(new Position() { X = j, Y = i });
        }
        for (short i = (short)(piecePosition.Y + 1), j = (short)(piecePosition.X - 1); i < 8 && j > -1; i++, j--)
        {
            if (board[j, i] is not null)
            {
                if (board[j, i].Color != board[piecePosition].Color)
                    positions.Add(new Position() { X = j, Y = i });

                break;
            }

            positions.Add(new Position() { X = j, Y = i });
        }
        for (short i = (short)(piecePosition.Y - 1), j = (short)(piecePosition.X - 1); i > -1 && j > -1; i--, j--)
        {
            if (board[j, i] is not null)
            {
                if (board[j, i].Color != board[piecePosition].Color)
                    positions.Add(new Position() { X = j, Y = i });

                break;
            }

            positions.Add(new Position() { X = j, Y = i });
        }
    }

    private static void GenerateRookPositions(Position piecePosition, ChessBoard board, List<Position> positions)
    {
        for (short i = (short)(piecePosition.X + 1); i < 8; i++)
        {
            if (board[i, piecePosition.Y] is not null)
            {
                if (board[i, piecePosition.Y].Color != board[piecePosition].Color)
                    positions.Add(new Position() { X = i, Y = piecePosition.Y });

                break;
            }

            positions.Add(new Position() { X = i, Y = piecePosition.Y });
        }
        for (short i = (short)(piecePosition.Y + 1); i < 8; i++)
        {
            if (board[piecePosition.X, i] is not null)
            {
                if (board[piecePosition.X, i].Color != board[piecePosition].Color)
                    positions.Add(new Position() { X = piecePosition.X, Y = i });

                break;
            }

            positions.Add(new Position() { X = piecePosition.X, Y = i });
        }
        for (short i = (short)(piecePosition.X - 1); i > -1; i--)
        {
            if (board[i, piecePosition.Y] is not null)
            {
                if (board[i, piecePosition.Y].Color != board[piecePosition].Color)
                    positions.Add(new Position() { X = i, Y = piecePosition.Y });

                break;
            }

            positions.Add(new Position() { X = i, Y = piecePosition.Y });
        }
        for (short i = (short)(piecePosition.Y - 1); i > -1; i--)
        {
            if (board[piecePosition.X, i] is not null)
            {
                if (board[piecePosition.X, i].Color != board[piecePosition].Color)
                    positions.Add(new Position() { X = piecePosition.X, Y = i });

                break;
            }

            positions.Add(new Position() { X = piecePosition.X, Y = i });
        }
    }
}