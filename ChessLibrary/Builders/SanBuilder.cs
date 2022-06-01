// *****************************************************
// *                                                   *
// * O Lord, Thank you for your goodness in our lives. *
// *     Please bless this code to our compilers.      *
// *                     Amen.                         *
// *                                                   *
// *****************************************************
//                                    Made by Geras1mleo

namespace Chess;

internal static class SanBuilder
{
    public static (bool succeeded, ChessException? exception) TryParse(ChessBoard board, string san, out Move? move, bool resetSan = false)
    {
        move = null;

        var matches = Regexes.regexSanOneMove.Matches(san);

        if (matches.Count == 0)
            return (false, new ChessArgumentException(board, "SAN move string should match pattern: " + Regexes.SanMovesPattern));

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
                        if (board.Turn == PieceColor.White)
                        {
                            originalPos = new("e1");
                            if (group.Value == "O-O")
                                moveOut.NewPosition = new("h1");
                            else if (group.Value == "O-O-O")
                                moveOut.NewPosition = new("a1");
                        }
                        else if (board.Turn == PieceColor.Black)
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
                    moveOut.Piece = new Piece(board.Turn, PieceType.FromChar(group.Value[0]));
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
        moveOut.Piece ??= new Piece(board.Turn, PieceType.Pawn);

        if (isCapture && board[moveOut.NewPosition] is not null)
            moveOut.CapturedPiece = board[moveOut.NewPosition];

        if (!originalPos.HasValue)
        {
            var amb = GetMovesOfPieceOnPosition(moveOut.Piece, moveOut.NewPosition, board).ToList();

            if (originalPos.HasValueX)
            {
                amb = amb.Where(m => m.OriginalPosition.X == originalPos.X).ToList();

                if (amb.Count != 1)
                    return (false, ThrowException(amb.Count, amb));

                originalPos.Y = amb.ElementAt(0).OriginalPosition.Y;
            }
            else if (originalPos.HasValueY)
            {
                amb = amb.Where(m => m.OriginalPosition.Y == originalPos.Y).ToList();

                if (amb.Count != 1)
                    return (false, ThrowException(amb.Count, amb));

                originalPos.X = amb.ElementAt(0).OriginalPosition.X;
            }
            else
            {
                if (amb.Count != 1)
                    return (false, ThrowException(amb.Count, amb));

                originalPos.X = amb.ElementAt(0).OriginalPosition.X;
                originalPos.Y = amb.ElementAt(0).OriginalPosition.Y;
            }
            ChessException? ThrowException(int count, List<Move> moves)
            {
                if (count < 1)
                    return new ChessSanNotFoundException(board, san);

                else if (count > 1)
                    return new ChessSanTooAmbiguousException(board, san, moves.ToArray());

                return null;
            }
        }

        moveOut.OriginalPosition = originalPos;

        if (resetSan)
            TryParse(board, moveOut, out var _);

        move = moveOut;

        return (true, null);
    }

    public static (bool succeeded, ChessException? exception) TryParse(ChessBoard board, Move move, out string? san)
    {
        san = null;

        if (move is null || !move.HasValue)
            return (false, new ChessArgumentException(board, "Given move is null or doesn't have valid positions values"));

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
                builder.Append(HandleAmbiguousMovesNotation(move, board));
        }

        if (move.CapturedPiece is not null)
        {
            if (move.Piece.Type == PieceType.Pawn)
                builder.Append(move.OriginalPosition.File());

            builder.Append('x');
        }

        if (move.Parameter is MoveEnPassant enPassant)
        {
            builder.Append(move.OriginalPosition.File().ToString() + 'x');

            // Not required (LAN)
            // builder.Append(" " + enPassant.ShortStr);
        }

        builder.Append(move.NewPosition);

        if (move.Parameter is MovePromotion)
            builder.Append(move.Parameter.ShortStr);

        CheckOrMateValidation:

        if (move.IsCheck && move.IsMate) builder.Append('#');

        else if (move.IsCheck) builder.Append('+');

        else if (move.IsMate) builder.Append('$');

        move.San = builder.ToString();

        return (true, null);
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

                    if (ChessBoard.IsValidMove(move, board) && !ChessBoard.IsKingCheckedValidation(move, piece.Color, board))
                        moves.Add(move);
                }
            }
        }

        return moves.ToArray();
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
}
