namespace Chess;

public partial class ChessBoard
{
    /// <summary>
    /// Converts San move into Move object
    /// </summary>
    /// <param name="move">San move that will be converted</param>
    /// <returns>Move object according to given san</returns>
    /// <exception cref="ArgumentNullException">move was null</exception>
    /// <exception cref="ArgumentException">Given move didn't match the Regex pattern</exception>
    /// <exception cref="ChessSanNotFoundException">Given SAN move is not valid for current board positions</exception>
    /// <exception cref="ChessSanTooAmbiguousException">Given SAN move is too ambiguous between multiple moves</exception>
    public Move San(string move)
    {
        if (move == null)
            throw new ArgumentNullException(nameof(move));

        string pattern = @"(^([PNBRQK])?([a-h])?([1-8])?(x|X|-)?([a-h][1-8])(=[NBRQ]| ?e\.p\.)?|^O-O(-O)?)(\+|\#|\$)?$";

        if (!Regex.IsMatch(move, pattern))
            throw new ArgumentException("SAN Move should match pattern: " + pattern);

        var matches = Regex.Matches(move, pattern);

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
                        moveOut.Parameter = MoveParameter.FromString(group.Value);
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
                        if(!IsValidMove(new Move(originalPos, moveOut.NewPosition)))
                            throw new ChessSanNotFoundException(this, move);
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
                    moveOut.Parameter = MoveParameter.FromString(group.Value.Trim());
                    break;
                case "9":
                    switch (group.Value)
                    {
                        case "+":
                            moveOut.IsCheck = true;
                            break;
                        case "#":
                            moveOut.IsCheck = true; moveOut.IsMate = true;
                            break;
                        case "$":
                            moveOut.IsMate = true;
                            break;
                    }
                    break;
            }
        }

        moveOut.Piece ??= new Piece(Turn, PieceType.Pawn);

        if (isCapture && this[moveOut.NewPosition] != null)
            moveOut.CapturedPiece = this[moveOut.NewPosition];

        if (!originalPos.HasValue)
        {
            var amb = Moves(false).Where(m => m.NewPosition == moveOut.NewPosition && m.Piece.Type == moveOut.Piece.Type);

            if (originalPos.HasValueX)
            {
                amb = amb.Where(m => m.OriginalPosition.X == originalPos.X);
                var count = amb.Count();

                if (count == 1)
                    originalPos.Y = amb.ElementAt(0).OriginalPosition.Y;

                else ThrowException(count, amb);
            }
            else if (originalPos.HasValueY)
            {
                amb = amb.Where(m => m.OriginalPosition.Y == originalPos.Y);
                var count = amb.Count();

                if (count == 1)
                    originalPos.X = amb.ElementAt(0).OriginalPosition.X;

                else ThrowException(count, amb);
            }
            else
            {
                var count = amb.Count();
                if (count == 1)
                {
                    originalPos.X = amb.ElementAt(0).OriginalPosition.X;
                    originalPos.Y = amb.ElementAt(0).OriginalPosition.Y;
                }
                else ThrowException(count, amb);
            }
            void ThrowException(int count, IEnumerable<Move> moves)
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
    /// <exception cref="ArgumentNullException">Given move was null or didn't have valid value</exception>
    public string San(Move move)
    {
        if (move == null || !move.HasValue)
            throw new ArgumentNullException(nameof(move));

        string sMove = "";

        if (move.Parameter == MoveParameter.CastleKing || move.Parameter == MoveParameter.CastleQueen)
        {
            sMove = move.Parameter.AsShortString;
            goto CheckOrMateValidation;
        }

        if (move.Piece.Type != PieceType.Pawn)
            sMove += char.ToUpper(move.Piece.Type.AsChar) + HandleAmbiguousMoves(move);

        if (move.CapturedPiece != null)
        {
            if (move.Piece.Type == PieceType.Pawn)
                sMove += move.OriginalPosition.File();

            sMove += "x";
        }

        sMove += move.NewPosition;

        if (move.Parameter == MoveParameter.PawnPromotion
         || move.Parameter == MoveParameter.PromotionToQueen
         || move.Parameter == MoveParameter.PromotionToRook
         || move.Parameter == MoveParameter.PromotionToBishop
         || move.Parameter == MoveParameter.PromotionToKnight)
            sMove += move.Parameter.AsShortString;

        else if (move.Parameter == MoveParameter.EnPassant)
            sMove += " " + move.Parameter.AsShortString;

        CheckOrMateValidation:
        if (move.IsCheck && move.IsMate) sMove += "#";
        else if (move.IsCheck) sMove += "+";
        else if (move.IsMate) sMove += "$";

        move.San = sMove;
        return sMove;
    }

    /// <summary>
    /// Load Chess game from Forsyth-Edwards Notation<br/>
    /// ex.:<br/>
    /// rnbqkbnr/pppppppp/8/8/4P3/8/PPPP1PPP/RNBQKBNR b KQkq e3 0 1
    /// </summary>
    /// <param name="fen">FEN string</param>
    /// <exception cref="ArgumentException">Given FEN string didn't match the Regex pattern</exception>
    public void Load(string fen)
    {
        Fen = new FenBoard(fen);
        pieces = Fen.Pieces;
        PerformedMoves = new List<Move>();
        moveIndex = -1; prevMoveIndex = -1;
        endGame = null;
        Fen.IsCheck = WhiteKingChecked || BlackKingChecked;
    }

    /// <summary>
    /// Generates Fen string of current board
    /// </summary>
    /// <returns></returns>
    public string ToFen()
    {
        return new FenBoard(this).ToString();
    }

    private string HandleAmbiguousMoves(Move move)
    {
        var sOut = "";
        var amb = Moves(false).Where(m => m.NewPosition == move.NewPosition && m.Piece.Type == move.Piece.Type && m.OriginalPosition != move.OriginalPosition).ToList();

        if (amb.Count > 0)
            if (amb.Any(m => m.OriginalPosition.X == move.OriginalPosition.X))
                sOut += move.OriginalPosition.ToString()[1];
            else
                sOut += move.OriginalPosition.ToString()[0];

        return sOut;
    }

    private static int GetHalfMovesCount(ChessBoard board)
    {
        int index = board.PerformedMoves.FindLastIndex(m => m.CapturedPiece != null || m.Piece.Type == PieceType.Pawn);

        if (board.LoadedFromFEN && index < 0)
            return board.Fen.HalfMoves + board.PerformedMoves.Count;

        if (index >= 0)
            return board.PerformedMoves.Count - index - 1;
        else
            return board.PerformedMoves.Count;
    }

    private static int GetFullMovesCount(ChessBoard board)
    {
        return (board.PerformedMoves.Count / 2) + (board.LoadedFromFEN ? board.Fen.FullMoves : 0);
    }

    internal class FenBoard
    {
        private readonly Piece?[,] pieces;

        /// <summary>
        /// "Begin Situation"
        /// </summary>
        public Piece?[,] Pieces => (Piece?[,])pieces.Clone();
        public PieceColor Turn { get; }

        public bool CastleWK { get; }
        public bool CastleWQ { get; }
        public bool CastleBK { get; }
        public bool CastleBQ { get; }

        public Position EnPassant { get; }

        /// <summary>
        /// Count since the last pawn advance or piece capture
        /// </summary>
        public int HalfMoves { get; }
        /// <summary>
        /// Black moves Count
        /// </summary>
        public int FullMoves { get; }

        /// <summary>
        /// Whether given pieces positions check one of kings
        /// </summary>
        public bool IsCheck { get; internal set; }

        public FenBoard(ChessBoard board)
        {
            pieces = board.pieces;
            Turn = board.Turn;
            CastleWK = HasRightToCastle(PieceColor.White, MoveParameter.CastleKing, board);
            CastleWQ = HasRightToCastle(PieceColor.White, MoveParameter.CastleQueen, board);
            CastleBK = HasRightToCastle(PieceColor.Black, MoveParameter.CastleKing, board);
            CastleBQ = HasRightToCastle(PieceColor.Black, MoveParameter.CastleQueen, board);
            EnPassant = LastMoveEnPassantPosition(board);
            HalfMoves = GetHalfMovesCount(board);
            FullMoves = GetFullMovesCount(board);
        }

        public FenBoard(string fen)
        {
            string pattern = @"^(((?:[rnbqkpRNBQKP1-8]+\/){7})[rnbqkpRNBQKP1-8]+) ([b|w]) (-|[K|Q|k|q]{1,4}) (-|[a-h][36]) (\d+ \d+)$";

            if (!Regex.IsMatch(fen, pattern))
                throw new ArgumentException("FEN should match pattern: " + pattern);

            pieces = new Piece[8, 8];
            var matches = Regex.Matches(fen, pattern);

            foreach (var group in matches[0].Groups.Values)
            {
                switch (group.Name)
                {
                    case "1":
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
                                    pieces[y, x] = new Piece(group.Value[i]);
                                    x++;
                                }
                                else if (char.IsDigit(group.Value[i]))
                                    x += int.Parse(group.Value[i].ToString());
                        }
                        break;
                    case "3":
                        Turn = PieceColor.FromChar(group.Value[0]);
                        break;
                    case "4":
                        if (group.Value != "-")
                        {
                            if (group.Value.Contains('K'))
                                CastleWK = true;
                            if (group.Value.Contains('Q'))
                                CastleWQ = true;
                            if (group.Value.Contains('k'))
                                CastleBK = true;
                            if (group.Value.Contains('q'))
                                CastleBQ = true;
                        }
                        break;
                    case "5":
                        if (group.Value == "-")
                            EnPassant = new();
                        else
                            EnPassant = new Position(group.Value);
                        break;
                    case "6":
                        (HalfMoves, FullMoves) = group.Value.Split(' ').Select(s => int.Parse(s)).ToArray();
                        break;
                }
            }
        }

        public override string ToString()
        {
            string sPieces = "";

            for (int i = 7; i >= 0; i--)
            {
                int emptyCount = 0;
                for (int j = 0; j < 8; j++)
                {
                    if (pieces[i, j] == null)
                        emptyCount++;
                    else
                    {
                        if (emptyCount > 0)
                        {
                            sPieces += emptyCount;
                            emptyCount = 0;
                        }
                        sPieces += pieces[i, j].ToFenChar();
                    }
                }
                if (emptyCount > 0)
                    sPieces += emptyCount;
                if (i - 1 >= 0)
                    sPieces += "/";
            }

            string sCastles = "";

            if (CastleWK)
                sCastles += "K";
            if (CastleWQ)
                sCastles += "Q";
            if (CastleBK)
                sCastles += "k";
            if (CastleBQ)
                sCastles += "q";

            if (sCastles == "")
                sCastles = "-";

            string sEnPas;

            if (EnPassant.HasValue)
                sEnPas = EnPassant.ToString();
            else
                sEnPas = "-";

            return string.Join(' ', sPieces, Turn.AsChar, sCastles, sEnPas, HalfMoves, FullMoves);
        }
    }
}