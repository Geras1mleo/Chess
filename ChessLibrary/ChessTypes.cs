namespace Chess;

/// <summary>
/// Chess Piece
/// </summary>
public class Piece
{
    /// <summary>
    /// Color of piece White/Black
    /// </summary>
    public PieceColor Color { get; }
    /// <summary>
    /// Type of piece
    /// See: PieceType
    /// </summary>
    public PieceType Type { get; internal set; }

    /// <summary>
    /// Initializes new Piece by given color and type
    /// </summary>
    public Piece(PieceColor color, PieceType type)
    {
        Color = color;
        Type = type;
    }

    /// <summary>
    /// Initializes new Piece by given color and type as:
    /// "wp" => White Pawn
    /// "br" => Black Rook
    /// etc...
    /// See: piece.ToString()
    /// </summary>
    public Piece(string piece)
    {
        if (!Regex.IsMatch(piece, "^[wb][bknpqr]$"))
            throw new ArgumentException("Piece should match pattern: ^[wb][bknpqr]$");

        Color = PieceColor.FromChar(piece[0]);
        Type = PieceType.FromChar(piece[1]);
    }

    /// <summary>
    /// Initializes new Piece by given color and type as FEN:
    /// 'Q' => White Queen
    /// 'q' => Black Queen
    /// etc...
    /// See: piece.ToFENChar()
    /// </summary>
    public Piece(char fenChar)
    {
        if (!Regex.IsMatch(fenChar.ToString(), "^([bknpqr]|[BKNPQR])$"))
            throw new ArgumentException("FEN piece should match pattern: ^([bknpqr]|[BKNPQR])$");

        Type = PieceType.FromChar(fenChar);
        Color = char.IsLower(fenChar) ? PieceColor.Black : PieceColor.White;
    }

    /// <returns>
    /// Piece as:
    /// "wp" => White Pawn
    /// "br" => Black Rook
    /// etc...
    /// See: new Piece(string piece)
    /// </returns>
    public override string ToString() => $"{Color.AsChar}{Type.AsChar}";

    /// <returns>
    /// Piece as FEN char
    /// Uppercase => White piece
    /// Lowercase => Black piece
    /// See: new Piece(char fenChar)
    /// </returns>
    public char ToFenChar()
    {
        return Color switch
        {
            var e when e == PieceColor.White => char.ToUpper(Type.AsChar),
            var e when e == PieceColor.Black => char.ToLower(Type.AsChar),
            var _ => throw new ArgumentException("GetAsFENChar"),
        };
    }
}

/// <summary>
/// Position on Chess table counting from 0
/// </summary>
public struct Position
{
    /// <summary>
    /// Whether X is between 0 and 7<br/>
    /// And Y is between 0 and 7
    /// </summary>
    public bool HasValue => HasValueX & HasValueY;

    /// <summary>
    /// Whether X is between 0 and 7
    /// </summary>
    public bool HasValueX => X >= 0 && X <= 7;
    /// <summary>
    /// Whether Y is between 0 and 7
    /// </summary>
    public bool HasValueY => Y >= 0 && Y <= 7;

    /// <summary>
    /// Horizontal position (File) on chess board
    /// </summary>
    public short X { get; internal set; } = -1;
    /// <summary>
    /// Vertical position (Rank) on chess board
    /// </summary>
    public short Y { get; internal set; } = -1;

    /// <summary>
    /// Initializes a new Position ex.:<br/>
    /// "a1" - notation => {X = 0, Y = 0}<br/>
    /// "h8" - notation => {X = 7, Y = 7}<br/>
    /// </summary>
    /// <param name="position">Position as string</param>
    public Position(string position)
    {
        position = position.ToLower();

        if (!Regex.IsMatch(position, "^[a-h][1-8]$"))
            throw new ArgumentException("Table position should match pattern: ^[a-h][1-8]$");

        X = FromFile(position[0]);
        Y = FromRank(position[1]);
    }

    /// <summary>
    /// Initializes an empty Position:<br/>
    /// {X = -1, Y = -1}
    /// </summary>
    public Position() { }

    /// <returns>
    /// Short horizontal position from file char<br/>
    /// 'a' => 0<br/>
    /// 'b' => 1<br/>
    /// 'c' => 2<br/>
    /// 'd' => 3<br/>
    /// 'e' => 4<br/>
    /// 'f' => 5<br/>
    /// 'g' => 6<br/>
    /// 'h' => 7<br/>
    /// </returns>
    public static short FromFile(char file)
    {
        return char.ToLower(file) switch
        {
            'a' => 0,
            'b' => 1,
            'c' => 2,
            'd' => 3,
            'e' => 4,
            'f' => 5,
            'g' => 6,
            'h' => 7,
            _ => throw new ArgumentException("Position.FromFile"),
        };
    }

    /// <returns>
    /// Short vertical position from rank char<br/>
    /// '1' => 0<br/>
    /// '2' => 1<br/>
    /// '3' => 2<br/>
    /// '4' => 3<br/>
    /// '5' => 4<br/>
    /// '6' => 5<br/>
    /// '7' => 6<br/>
    /// '8' => 7<br/>
    /// </returns>
    public static short FromRank(char rank)
    {   // This code is faster than convertion to short with short.TryParse(...)
        return rank switch
        {
            '1' => 0,
            '2' => 1,
            '3' => 2,
            '4' => 3,
            '5' => 4,
            '6' => 5,
            '7' => 6,
            '8' => 7,
            _ => throw new ArgumentException("Position.FromRank"),
        };
    }

    /// <returns>
    /// Char from X<br/>
    /// 0 => 'a'<br/>
    /// 1 => 'b'<br/>
    /// 2 => 'c'<br/>
    /// 3 => 'd'<br/>
    /// 4 => 'e'<br/>
    /// 5 => 'f'<br/>
    /// 6 => 'g'<br/>
    /// 7 => 'h'<br/>
    /// </returns>
    public char File()
    {
        return X switch
        {
            0 => 'a',
            1 => 'b',
            2 => 'c',
            3 => 'd',
            4 => 'e',
            5 => 'f',
            6 => 'g',
            7 => 'h',
            _ => throw new ArgumentException("Position.File")
        };
    }

    /// <returns>
    /// Char from Y
    /// 0 => '1'<br/>
    /// 1 => '2'<br/>
    /// 2 => '3'<br/>
    /// 3 => '4'<br/>
    /// 4 => '5'<br/>
    /// 5 => '6'<br/>
    /// 6 => '7'<br/>
    /// 7 => '8'<br/>
    /// </returns>
    public char Rank()
    {   // This code is faster than convertion to char
        return Y switch
        {
            0 => '1',
            1 => '2',
            2 => '3',
            3 => '4',
            4 => '5',
            5 => '6',
            6 => '7',
            7 => '8',
            _ => throw new ArgumentException("Position.Rank")
        };
    }

    /// <returns>
    /// Position as string position on board with rank:<br/>
    /// {X = 0, Y = 0} => "a1"<br/>
    /// {X = 7, Y = 7} => "h8"<br/>
    /// </returns>
    public override string ToString() => File().ToString() + Rank();

    /// <summary>
    /// Equalizing Positions objects
    /// </summary>
    public override bool Equals(object obj) => base.Equals(obj);
    /// <summary>
    /// HashCode
    /// </summary>
    public override int GetHashCode() => base.GetHashCode();

    /// <summary>
    /// Equalizing 2 Positions
    /// </summary>
    public static bool operator ==(Position a, Position b) => (a.X == b.X && a.Y == b.Y);
    /// <summary>
    /// Equalizing 2 Positions
    /// </summary>
    public static bool operator !=(Position a, Position b) => !(a.X == b.X && a.Y == b.Y);
}

/// <summary>
/// Move on chess board
/// </summary>
public class Move
{
    const string ParametersSeparator = " - ";

    /// <summary>
    /// Whether Positions are initialized
    /// </summary>
    public bool HasValue => OriginalPosition.HasValue && NewPosition.HasValue;

    /// <summary>
    /// Moved Piece
    /// </summary>
    public Piece Piece { get; internal set; }

    /// <summary>
    /// Original position of moved piece
    /// </summary>
    public Position OriginalPosition { get; internal set; }

    /// <summary>
    /// New Position of moved piece
    /// </summary>
    public Position NewPosition { get; internal set; }

    /// <summary>
    /// Captured piece (if exist) or null
    /// </summary>
    public Piece? CapturedPiece { get; internal set; }

    /// <summary>
    /// Move additional parameter   
    /// </summary>
    public MoveParameter? Parameter { get; internal set; }

    /// <summary>
    /// Move places opponent's king in check? => true
    /// </summary>
    public bool IsCheck { get; internal set; }

    /// <summary>
    /// Move places opponent's king in checkmate => true
    /// </summary>
    public bool IsMate { get; internal set; }

    /// <summary>
    /// Move in SAN Notation<br/>
    /// -> Use board.MoveToSan() to get san string for this move according to your board positions
    /// </summary>
    public string? San { get; internal set; }

    /// <summary>
    /// Initializes new Move that has to be validated
    /// </summary>
    public Move(Position originalPosition, Position newPosition)
    {
        OriginalPosition = originalPosition;
        NewPosition = newPosition;
    }

    /// <summary>
    /// Initalizes new Move from long move notation
    /// </summary>
    /// <param name="move">
    /// Move as long string<br/>
    /// ex.:{wr - a1 - h8 - bq - e.p. - +}<br/>
    /// Or: {a1 - h8}<br/>
    /// See: move.ToString()
    /// </param>
    /// <exception cref="ArgumentException">Move </exception>
    public Move(string move)
    {
        move = move.ToLower();
        var pattern = "^{(((w|b)(b|k|n|p|q|r))" + ParametersSeparator + "|)" +
                    "[a-h][1-8]" + ParametersSeparator + "[a-h][1-8]" +
                    "(" + ParametersSeparator + "(w|b)(b|k|n|p|q|r)|)" +
                    "(" + ParametersSeparator + @"((o-o)|(o-o-o)|(e\.p\.)|(=)|(=q)|(=r)|(=b)|(=n))|)" +
                    "(" + ParametersSeparator + @"(\+|#)|)}$";

        if (!Regex.IsMatch(move, pattern))
            throw new ArgumentException("Move should match pattern: " + pattern);

        var args = move[1..^1].Split(new string[] { ParametersSeparator }, StringSplitOptions.None);


        if (Regex.IsMatch(args[0], "(w|b)(b|k|n|p|q|r)") && args.Length > 2)
        {
            Piece = new Piece(args[0]);
            OriginalPosition = new Position(args[1]);
            NewPosition = new Position(args[2]);
            SetArgsFrom(3);
        }
        else
        {
            OriginalPosition = new Position(args[0]);
            NewPosition = new Position(args[1]);
            SetArgsFrom(2);
        }

        void SetArgsFrom(int index)
        {
            for (int i = index; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case var e when Regex.IsMatch(e, "(w|b)(b|k|n|p|q|r)"):
                        CapturedPiece = new Piece(e);
                        break;
                    case var e when Regex.IsMatch(e, @"(0-0)|(0-0-0)|(e\.p\.)|(=)|(=q)|(=r)|(=b)|(=n)"):
                        Parameter = MoveParameter.FromString(e);
                        break;
                    case var e when Regex.IsMatch(e, @"(\+)"):
                        IsCheck = true;
                        break;
                    case var e when Regex.IsMatch(e, @"(#)"):
                        IsCheck = true; IsMate = true;
                        break;
                }
            }
        }
    }

    /// <summary>
    /// Needed to Generate move from SAN in ChessConvertions
    /// </summary>
    internal Move()
    {
        OriginalPosition = new();
        NewPosition = new();
    }

    /// <returns>
    /// Long move notation as: <br/>
    /// {wr - a1 - h8 - bq - e.p. - +}<br/>
    /// Or: {a1 - h8}
    /// </returns>
    public override string ToString()
    {
        return "{" +
                    (Piece is null ? "" : Piece + ParametersSeparator) +
                    OriginalPosition + ParametersSeparator + NewPosition + // Permanent
                    (CapturedPiece is null ? "" : ParametersSeparator + CapturedPiece) +
                    (Parameter is null ? "" : ParametersSeparator + Parameter.AsShortString) +
                    (!IsMate ? (IsCheck ? ParametersSeparator + "+" : "") : ParametersSeparator + "#")
                + "}";
    }

    /// <summary>
    /// Equalizing Moves objects
    /// </summary>
    public override bool Equals(object obj) => base.Equals(obj);
    /// <summary>
    /// HashCode
    /// </summary>
    public override int GetHashCode() => base.GetHashCode();
}

/// <summary>
/// Chess end game info
/// </summary>
public class EndGameInfo
{
    /// <summary>
    /// Endgame type of current chess game
    /// </summary>
    public EndgameType EndgameType { get; }
    /// <summary>
    /// Won side or null if draw/stalemate
    /// </summary>
    public PieceColor? WonSide { get; }

    /// <summary>
    /// Initializes new object of EndGameInfo with given end game paramters 
    /// </summary>
    public EndGameInfo(EndgameType endgameType, PieceColor? wonSide)
    {
        EndgameType = endgameType;
        WonSide = wonSide;
    }
}