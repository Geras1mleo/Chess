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
    }

    internal static (bool succeeded, ChessException? exception) TryLoad(string fen, out FenBoardBuilder? builder)
    {
        builder = null;

        var matches = Regexes.regexFen.Matches(fen);

        if (matches.Count == 0)
            return (false, new ChessArgumentException(null, "FEN board string should match pattern: " + Regexes.FenPattern));

        builder = new FenBoardBuilder();

        foreach (var group in matches[0].Groups.Values)
        {
            switch (group.Name)
            {
                case "1":
                    // Set pieces to given positions
                    int x = 0, y = 7;
                    for (int i = 0; i < group.Length; i++)
                    {
                        if (group.Value[i] == '/')
                        {
                            y--;
                            x = 0;
                            continue;
                        }
                        if (x < 8)
                            if (char.IsLetter(group.Value[i]))
                            {
                                builder.pieces[y, x] = new Piece(group.Value[i]);
                                x++;
                            }
                            else if (char.IsDigit(group.Value[i]))
                                x += int.Parse(group.Value[i].ToString());
                    }
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
                    if (group.Value == "-")
                        builder.EnPassant = new();
                    else
                        builder.EnPassant = new Position(group.Value);
                    break;
                case "6":
                    (builder.HalfMoves, builder.FullMoves) = group.Value.Split(' ').Select(s => int.Parse(s)).ToArray();
                    break;
            }
        }

        var wcap = new List<Piece>();
        var bcap = new List<Piece>();

        var fpieces = builder.pieces.Cast<Piece>().Where(p => p is not null);

        // Calculating missing pieces on according begin pieces in fen
        // Math.Clamp() for get max/min taken figures (2 queens possible)
        wcap.AddRange(Enumerable.Range(0, Math.Clamp(8 - fpieces.Where(p => p.Type == PieceType.Pawn && p.Color == PieceColor.White).Count(), 0, 8)).Select(_ => new Piece(PieceColor.White, PieceType.Pawn)));
        wcap.AddRange(Enumerable.Range(0, Math.Clamp(2 - fpieces.Where(p => p.Type == PieceType.Rook && p.Color == PieceColor.White).Count(), 0, 2)).Select(_ => new Piece(PieceColor.White, PieceType.Rook)));
        wcap.AddRange(Enumerable.Range(0, Math.Clamp(2 - fpieces.Where(p => p.Type == PieceType.Bishop && p.Color == PieceColor.White).Count(), 0, 2)).Select(_ => new Piece(PieceColor.White, PieceType.Bishop)));
        wcap.AddRange(Enumerable.Range(0, Math.Clamp(2 - fpieces.Where(p => p.Type == PieceType.Knight && p.Color == PieceColor.White).Count(), 0, 2)).Select(_ => new Piece(PieceColor.White, PieceType.Knight)));
        wcap.AddRange(Enumerable.Range(0, Math.Clamp(1 - fpieces.Where(p => p.Type == PieceType.Queen && p.Color == PieceColor.White).Count(), 0, 1)).Select(_ => new Piece(PieceColor.White, PieceType.Queen)));

        bcap.AddRange(Enumerable.Range(0, Math.Clamp(8 - fpieces.Where(p => p.Type == PieceType.Pawn && p.Color == PieceColor.Black).Count(), 0, 8)).Select(_ => new Piece(PieceColor.Black, PieceType.Pawn)));
        bcap.AddRange(Enumerable.Range(0, Math.Clamp(2 - fpieces.Where(p => p.Type == PieceType.Rook && p.Color == PieceColor.Black).Count(), 0, 2)).Select(_ => new Piece(PieceColor.Black, PieceType.Rook)));
        bcap.AddRange(Enumerable.Range(0, Math.Clamp(2 - fpieces.Where(p => p.Type == PieceType.Bishop && p.Color == PieceColor.Black).Count(), 0, 2)).Select(_ => new Piece(PieceColor.Black, PieceType.Bishop)));
        bcap.AddRange(Enumerable.Range(0, Math.Clamp(2 - fpieces.Where(p => p.Type == PieceType.Knight && p.Color == PieceColor.Black).Count(), 0, 2)).Select(_ => new Piece(PieceColor.Black, PieceType.Knight)));
        bcap.AddRange(Enumerable.Range(0, Math.Clamp(1 - fpieces.Where(p => p.Type == PieceType.Queen && p.Color == PieceColor.Black).Count(), 0, 1)).Select(_ => new Piece(PieceColor.Black, PieceType.Queen)));

        builder.WhiteCaptured = wcap.ToArray();
        builder.BlackCaptured = bcap.ToArray();

        return (true, null);
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
        StringBuilder piecesBuilder = new();

        for (int i = 7; i >= 0; i--)
        {
            int emptyCount = 0;
            for (int j = 0; j < 8; j++)
            {
                if (pieces[i, j] is null)
                    emptyCount++;
                else
                {
                    if (emptyCount > 0)
                    {
                        piecesBuilder.Append(emptyCount);
                        emptyCount = 0;
                    }
                    piecesBuilder.Append(pieces[i, j].ToFenChar());
                }
            }
            if (emptyCount > 0)
                piecesBuilder.Append(emptyCount);
            if (i - 1 >= 0)
                piecesBuilder.Append('/');
        }

        StringBuilder castlesBuilder = new();

        if (CastleWK)
            castlesBuilder.Append('K');
        if (CastleWQ)
            castlesBuilder.Append('Q');
        if (CastleBK)
            castlesBuilder.Append('k');
        if (CastleBQ)
            castlesBuilder.Append('q');

        if (castlesBuilder.Length == 0)
            castlesBuilder.Append('-');

        string enPasBuilder;

        if (EnPassant.HasValue)
            enPasBuilder = EnPassant.ToString();
        else
            enPasBuilder = "-";

        return string.Join(' ', piecesBuilder, Turn.AsChar, castlesBuilder, enPasBuilder, HalfMoves, FullMoves);
    }
}