// *****************************************************
// *                                                   *
// * O Lord, Thank you for your goodness in our lives. *
// *     Please bless this code to our compilers.      *
// *                     Amen.                         *
// *                                                   *
// *****************************************************
//                                    Made by Geras1mleo

namespace Chess;

internal class FenBoardBuilder
{
    private readonly Piece?[,] pieces;

    /// <summary>
    /// "Begin Situation"
    /// </summary>
    internal Piece?[,] Pieces => (Piece?[,])pieces.Clone();

    internal PieceColor Turn { get; private set; }

    internal bool CastleWK { get; private set; }
    internal bool CastleWQ { get; private set; }
    internal bool CastleBK { get; private set; }
    internal bool CastleBQ { get; private set; }

    internal Position EnPassant { get; private set; }

    /// <summary>
    /// Count since the last pawn advance or piece capture
    /// </summary>
    internal int HalfMoves { get; private set; }

    /// <summary>
    /// Black moves Count
    /// </summary>
    internal int FullMoves { get; private set; }

    internal Piece[] WhiteCaptured { get; private set; }
    internal Piece[] BlackCaptured { get; private set; }

    private FenBoardBuilder(Piece?[,] pieces)
    {
        this.pieces = pieces;
    }

    private FenBoardBuilder()
    {
        pieces = new Piece[8, 8];
        EnPassant = new Position();
    }

    internal static (bool succeeded, ChessException? exception) TryLoad(string fen, out FenBoardBuilder? builder)
    {
        builder = null;

        var matches = Regexes.RegexFen.Matches(fen);

        if (matches.Count == 0)
            return (false, new ChessArgumentException(null, "FEN board string should match pattern: " + Regexes.FenPattern));

        if (!Regexes.RegexFenContainsOneWhiteKing.IsMatch(fen) || !Regexes.RegexFenContainsOneBlackKing.IsMatch(fen))
            return (false, new ChessArgumentException(null, "Chess board should have exact 1 white king and exact 1 black king"));

        builder = new FenBoardBuilder();

        foreach (var group in matches[0].Groups.Values)
        {
            switch (group.Name)
            {
                case "1":
                    PlacePiecesOnBoard(builder, group.Value.AsSpan());
                    break;
                case "3":
                    builder.Turn = PieceColor.FromChar(group.Value[0]);
                    break;
                case "4":
                    if (group.Value != "-")
                    {
                        if (group.Value.Contains('K'))
                            builder.CastleWK = true;
                        if (group.Value.Contains('Q'))
                            builder.CastleWQ = true;
                        if (group.Value.Contains('k'))
                            builder.CastleBK = true;
                        if (group.Value.Contains('q'))
                            builder.CastleBQ = true;
                    }

                    break;
                case "5":
                    if (group.Value != "-")
                    {
                        builder.EnPassant = new Position(group.Value);
                    }

                    break;
                case "6":
                    (builder.HalfMoves, builder.FullMoves) = group.Value.Split(' ').Select(int.Parse).ToArray();
                    break;
            }
        }

        AddCapturedPieces(builder);

        return (true, null);
    }

    private static void PlacePiecesOnBoard(FenBoardBuilder builder, ReadOnlySpan<char> piecesSpan)
    {
        int x = 0, y = 7;
        foreach (var fenChar in piecesSpan)
        {
            if (fenChar == '/')
            {
                y--;
                x = 0;
            }
            else if (x < 8)
            {
                if (char.IsLetter(fenChar))
                {
                    builder.pieces[y, x] = new Piece(fenChar);
                    x++;
                }
                else if (char.IsDigit(fenChar))
                {
                    x += fenChar - '0';
                }
            }
        }
    }

    private static void AddCapturedPieces(FenBoardBuilder builder)
    {
        var whiteCaptured = new List<Piece>();
        var blackCaptured = new List<Piece>();
        var counts = (white: new Dictionary<PieceType, int>
                { { PieceType.Pawn, 0 }, { PieceType.Rook, 0 }, { PieceType.Bishop, 0 }, { PieceType.Knight, 0 }, { PieceType.Queen, 0 }, {PieceType.King , 0} },
            black: new Dictionary<PieceType, int>
                { { PieceType.Pawn, 0 }, { PieceType.Rook, 0 }, { PieceType.Bishop, 0 }, { PieceType.Knight, 0 }, { PieceType.Queen, 0 }, {PieceType.King , 0} });

        foreach (var piece in builder.pieces!.PiecesSpan())
        {
            if (piece.Color == PieceColor.White)
            {
                counts.white[piece.Type]++;
            }
            else
            {
                counts.black[piece.Type]++;
            }
        }

        AddPiecesToList(whiteCaptured, PieceColor.White, PieceType.Pawn, counts.white[PieceType.Pawn], 8);
        AddPiecesToList(whiteCaptured, PieceColor.White, PieceType.Rook, counts.white[PieceType.Rook], 2);
        AddPiecesToList(whiteCaptured, PieceColor.White, PieceType.Bishop, counts.white[PieceType.Bishop], 2);
        AddPiecesToList(whiteCaptured, PieceColor.White, PieceType.Knight, counts.white[PieceType.Knight], 2);
        AddPiecesToList(whiteCaptured, PieceColor.White, PieceType.Queen, counts.white[PieceType.Queen], 1);

        AddPiecesToList(blackCaptured, PieceColor.Black, PieceType.Pawn, counts.black[PieceType.Pawn], 8);
        AddPiecesToList(blackCaptured, PieceColor.Black, PieceType.Rook, counts.black[PieceType.Rook], 2);
        AddPiecesToList(blackCaptured, PieceColor.Black, PieceType.Bishop, counts.black[PieceType.Bishop], 2);
        AddPiecesToList(blackCaptured, PieceColor.Black, PieceType.Knight, counts.black[PieceType.Knight], 2);
        AddPiecesToList(blackCaptured, PieceColor.Black, PieceType.Queen, counts.black[PieceType.Queen], 1);

        builder.WhiteCaptured = whiteCaptured.ToArray();
        builder.BlackCaptured = blackCaptured.ToArray();
    }

    private static void AddPiecesToList(List<Piece> captured, PieceColor color, PieceType type, int actualCount, int targetCount)
    {
        for (int i = actualCount; i < targetCount; i++)
        {
            captured.Add(new Piece(color, type));
        }
    }

    internal static FenBoardBuilder Load(ChessBoard board)
    {
        return new FenBoardBuilder(board.pieces)
        {
            Turn = board.Turn,
            CastleWK = ChessBoard.HasRightToCastle(PieceColor.White, CastleType.King, board),
            CastleWQ = ChessBoard.HasRightToCastle(PieceColor.White, CastleType.Queen, board),
            CastleBK = ChessBoard.HasRightToCastle(PieceColor.Black, CastleType.King, board),
            CastleBQ = ChessBoard.HasRightToCastle(PieceColor.Black, CastleType.Queen, board),
            EnPassant = ChessBoard.LastMoveEnPassantPosition(board),
            HalfMoves = board.GetHalfMovesCount(),
            FullMoves = board.GetFullMovesCount()
        };
    }

    public override string ToString()
    {
        return string.Join(' ',
            GetPiecePlacement(),
            GetActiveColor(),
            GetCastlingAvailability(),
            GetEnPassantTargetSquare(),
            HalfMoves.ToString(),
            FullMoves.ToString());
    }

    private string GetPiecePlacement()
    {
        Span<char> span = stackalloc char[71]; // Max lenght is 71
        int offset = 0;

        int i = 7;
        while (i >= 0 && offset < span.Length)
        {
            int emptySquaresCount = 0;

            for (int j = 0; j < 8 && offset < span.Length; j++)
            {
                if (pieces[i, j] is null)
                    emptySquaresCount++;
                else
                {
                    if (emptySquaresCount > 0)
                    {
                        span[offset++] = (char)('0' + emptySquaresCount);
                        emptySquaresCount = 0;
                    }

                    span[offset++] = pieces[i, j]!.ToFenChar();
                }
            }

            if (emptySquaresCount > 0)
                span[offset++] = (char)('0' + emptySquaresCount);

            if (i - 1 >= 0)
                span[offset++] = '/';

            i--;
        }

        return new string(span.Slice(0, offset));
    }

    private string GetActiveColor()
    {
        return Turn.AsChar.ToString();
    }

    private string GetCastlingAvailability()
    {
        Span<char> span = stackalloc char[4]; // Max lenght is 4
        int offset = 0;

        if (CastleWK) span[offset++] = 'K';
        if (CastleWQ) span[offset++] = 'Q';
        if (CastleBK) span[offset++] = 'k';
        if (CastleBQ) span[offset++] = 'q';

        // Castling not available
        if (offset == 0) span[offset++] = '-';

        return new string(span.Slice(0, offset));
    }

    private string GetEnPassantTargetSquare()
    {
        if (EnPassant.HasValue)
            return EnPassant.ToString();

        return "-";
    }
}