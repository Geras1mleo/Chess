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
        var matches = Regexes.RegexSanOneMove.Matches(san);

        if (matches.Count == 0)
            return (false, new ChessArgumentException(board, "SAN move string should match pattern: " + Regexes.SanMovesPattern));

        var moveOut = new Move();
        var originalPos = new Position();
        var isCapture = false;

        foreach (var group in matches[0].Groups.Values)
        {
            if (!group.Success) continue;

            switch (group.Name)
            {
                case "1":
                    if (group.Value == "O-O" || group.Value == "O-O-O")
                        ParseCastling(board, group, moveOut, ref originalPos);
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
                    ParseEndgameGroup(group, moveOut);
                    break;
            }
        }

        // If piece is not specified => Pawn
        moveOut.Piece ??= new Piece(board.Turn, PieceType.Pawn);

        if (isCapture && board[moveOut.NewPosition] is not null)
            moveOut.CapturedPiece = board[moveOut.NewPosition];

        moveOut.OriginalPosition = originalPos;

        var (succeeded, exception) = ParseOriginalPosition(board, san, moveOut);
        if (!succeeded)
            return (false, exception);

        if (resetSan)
        {
            TryParse(board, moveOut, out _);
        }

        move = moveOut;
        return (true, null);
    }

    private static (bool succeeded, ChessException? exception) ParseOriginalPosition(ChessBoard board, string san, Move move)
    {
        ChessException? GetException(int count, List<Move> moves)
        {
            return count switch
            {
                < 1 => new ChessSanNotFoundException(board, san),
                > 1 => new ChessSanTooAmbiguousException(board, san, moves.ToArray()),
                _ => null
            };
        }

        var ambiguousMoves = GetMovesOfPieceOnPosition(move.Piece, move.NewPosition, board).ToList();

        if (move.OriginalPosition.HasValueX)
            ambiguousMoves.RemoveAll(m => m.OriginalPosition.X != move.OriginalPosition.X);
        if (move.OriginalPosition.HasValueY)
            ambiguousMoves.RemoveAll(m => m.OriginalPosition.Y != move.OriginalPosition.Y);

        if (ambiguousMoves.Count != 1)
            return (false, GetException(ambiguousMoves.Count, ambiguousMoves));

        move.OriginalPosition = ambiguousMoves[0].OriginalPosition;

        // EnPassant
        if (ambiguousMoves[0].Parameter is MoveEnPassant enPassant)
        {
            move.Parameter = enPassant;
            move.CapturedPiece = ambiguousMoves[0].CapturedPiece;
        }

        return (true, null);
    }

    private static void ParseEndgameGroup(Group group, Move moveOut)
    {
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
    }

    private static void ParseCastling(ChessBoard board, Group group, Move move, ref Position originalPos)
    {
        move.Parameter = IMoveParameter.FromString(group.Value);
        if (board.Turn == PieceColor.White)
        {
            originalPos = new Position("e1");
            if (group.Value == "O-O")
                move.NewPosition = new Position("g1");
            else if (group.Value == "O-O-O")
                move.NewPosition = new Position("c1");
        }
        else if (board.Turn == PieceColor.Black)
        {
            originalPos = new Position("e8");
            if (group.Value == "O-O")
                move.NewPosition = new Position("g8");
            else if (group.Value == "O-O-O")
                move.NewPosition = new Position("c8");
        }
        move.Piece = board[originalPos] ?? new Piece(board.Turn, PieceType.King);
    }

    public static (bool succeeded, ChessException? exception) TryParse(ChessBoard board, Move move, out string? san)
    {
        san = null;

        if (move is not { HasValue: true })
            return (false, new ChessArgumentException(board, "Given move is null or doesn't have valid positions values"));

        Span<char> span = stackalloc char[10];
        int offset = 0;

        if (move.Parameter is MoveCastle)
        {
            offset = span.InsertSpan(offset, move.Parameter.ShortStr.AsSpan());
        }
        else
        {
            if (move.Piece.Type != PieceType.Pawn)
            {
                span[offset++] = char.ToUpper(move.Piece.Type.AsChar);

                // Only rooks, knights, bishops(second from promotion) and queens(second from promotion) can have ambiguous moves
                if (move.Piece.Type != PieceType.King)
                    offset = span.InsertSpan(offset, HandleAmbiguousMovesNotation(move, board));
            }

            if (move.CapturedPiece is not null)
            {
                if (move.Piece.Type == PieceType.Pawn)
                    span[offset++] = move.OriginalPosition.File();

                span[offset++] = 'x';
            }

            // Destination position
            offset = span.InsertSpan(offset, move.NewPosition.ToString().AsSpan());

            if (move.Parameter is MovePromotion)
                offset = span.InsertSpan(offset, move.Parameter.ShortStr.AsSpan());
        }

        if (move.IsCheck && move.IsMate) span[offset++] = '#';
        else if (move.IsCheck) span[offset++] = '+';
        else if (move.IsMate) span[offset++] = '$';

        san = new string(span.Slice(0, offset));
        move.San = san;

        return (true, null);
    }

    private static ReadOnlySpan<char> HandleAmbiguousMovesNotation(Move move, ChessBoard board)
    {
        var ambiguousMoves = GetMovesOfPieceOnPosition(move.Piece, move.NewPosition, board).Where(m => m.OriginalPosition != move.OriginalPosition).ToList();

        Span<char> span = stackalloc char[2];

        if (ambiguousMoves.Any())
        {
            if (ambiguousMoves.Any(m => m.OriginalPosition.Y == move.OriginalPosition.Y))
                span[0] = move.OriginalPosition.File();

            if (ambiguousMoves.Any(m => m.OriginalPosition.X == move.OriginalPosition.X))
                span[1] = move.OriginalPosition.Rank();

            if (!char.IsLetterOrDigit(span[0]) && !char.IsLetterOrDigit(span[1]))
                span[0] = move.OriginalPosition.File();
        }

        return new ReadOnlySpan<char>(span.Trim(stackalloc char[] { '\0' }).ToArray());
    }

    private static IEnumerable<Move> GetMovesOfPieceOnPosition(Piece piece, Position newPosition, ChessBoard board)
    {
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

                    var move = new Move(new Position { Y = i, X = j }, newPosition) { Piece = piece };

                    if (ChessBoard.IsValidMove(move, board) && !ChessBoard.IsKingCheckedValidation(move, piece.Color, board))
                        yield return move;
                }
            }
        }
    }
}