// *****************************************************
// *                                                   *
// * O Lord, Thank you for your goodness in our lives. *
// *     Please bless this code to our compilers.      *
// *                     Amen.                         *
// *                                                   *
// *****************************************************
//                                    Made by Geras1mleo

namespace Chess;

/// <summary>
/// Coordinate like Position on Chess table counting from 0
/// </summary>
public struct Position
{
    /// <summary>
    /// Whether X and Y are in valid range [0; ChessBoard.MAX_COLS/MAX_ROWS[
    /// </summary>
    public bool HasValue => HasValueX & HasValueY;

    /// <summary>
    /// Whether X is in range [0; ChessBoard.MAX_COLS[
    /// </summary>
    public bool HasValueX => X >= 0 && X < ChessBoard.MAX_COLS;

    /// <summary>
    /// Whether Y is in range [0; ChessBoard.MAX_ROWS[
    /// </summary>
    public bool HasValueY => Y >= 0 && Y < ChessBoard.MAX_ROWS;

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

        if (!Regexes.RegexPosition.IsMatch(position))
            throw new ChessArgumentException(null, "Table position should match pattern: " + Regexes.PositionPattern);

        X = FromFile(position[0]);
        Y = FromRank(position[1]);
    }

    /// <summary>
    /// Initializes an empty Position:<br/>
    /// {X = -1, Y = -1}
    /// </summary>
    public Position() { }

    /// <summary>
    /// Initializes a new Position in chess board<br/>
    /// Counting from 0 
    /// </summary>
    public Position(short x, short y)
    {
        X = x;
        Y = y;
    }

    /// <summary>
    /// Short horizontal position from file char<br/>
    /// 'a' => 0<br/>
    /// 'b' => 1<br/>
    /// 'c' => 2<br/>
    /// 'd' => 3<br/>
    /// 'e' => 4<br/>
    /// 'f' => 5<br/>
    /// 'g' => 6<br/>
    /// 'h' => 7<br/>
    /// </summary>
    public static short FromFile(char file)
    {
        if (file < 'a' || file > 'h')
            throw new ChessArgumentException(null, nameof(file), nameof(Position.FromFile));

        return (short)(file - 'a');
    }

    /// <summary>
    /// Short vertical position from rank char<br/>
    /// '1' => 0<br/>
    /// '2' => 1<br/>
    /// '3' => 2<br/>
    /// '4' => 3<br/>
    /// '5' => 4<br/>
    /// '6' => 5<br/>
    /// '7' => 6<br/>
    /// '8' => 7<br/>
    /// </summary>
    public static short FromRank(char rank)
    {
        if (rank < '1' || rank > '8')
            throw new ChessArgumentException(null, nameof(rank), nameof(Position.FromRank));

        return (short)(rank - '1');
    }

    /// <summary>
    /// Convert X coordinate into file on board:<br/>
    /// 0 => 'a'<br/>
    /// 1 => 'b'<br/>
    /// 2 => 'c'<br/>
    /// 3 => 'd'<br/>
    /// 4 => 'e'<br/>
    /// 5 => 'f'<br/>
    /// 6 => 'g'<br/>
    /// 7 => 'h'<br/>
    /// </summary>
    public char File()
    {
        if (!HasValueX)
            throw new ChessArgumentException(null, nameof(X), nameof(Position.File));

        return (char)(X + 'a');
    }

    /// <summary>
    /// Char from Y<br/>
    /// 0 => '1'<br/>
    /// 1 => '2'<br/>
    /// 2 => '3'<br/>
    /// 3 => '4'<br/>
    /// 4 => '5'<br/>
    /// 5 => '6'<br/>
    /// 6 => '7'<br/>
    /// 7 => '8'<br/>
    /// </summary>
    public char Rank()
    {
        if (!HasValueY)
            throw new ChessArgumentException(null, nameof(Y), nameof(Position.Rank));

        return (char)(Y + '1');
    }

    /// <summary>
    /// Position as string position on board with rank:<br/>
    /// {X = 0, Y = 0} => "a1"<br/>
    /// {X = 7, Y = 7} => "h8"<br/>
    /// </summary>
    public override string ToString() => File().ToString() + Rank();

    /// <summary>
    /// Equalizing 2 Positions
    /// </summary>
    public static bool operator ==(Position a, Position b) => a.X == b.X && a.Y == b.Y;

    /// <summary>
    /// Equalizing 2 Positions
    /// </summary>
    public static bool operator !=(Position a, Position b) => !(a.X == b.X && a.Y == b.Y);

    /// <summary>
    /// Equalizing 2 Positions
    /// </summary>
    public bool Equals(Position other) => X == other.X && Y == other.Y;

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is Position other && Equals(other);

    /// <inheritdoc />
    public override int GetHashCode() => HashCode.Combine(X, Y);
}