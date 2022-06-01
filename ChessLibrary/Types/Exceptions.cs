// *****************************************************
// *                                                   *
// * O Lord, Thank you for your goodness in our lives. *
// *     Please bless this code to our compilers.      *
// *                     Amen.                         *
// *                                                   *
// *****************************************************
//                                    Made by Geras1mleo

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
namespace Chess;

public class ChessException : Exception
{
    public ChessBoard? Board { get; }
    public ChessException(ChessBoard? board, string message) : base(message) => Board = board;
}

public class ChessGameEndedException : ChessException
{
    public EndGameInfo EndgameInfo { get; }
    public ChessGameEndedException(ChessBoard board, EndGameInfo endgameInfo)
        : this(board, "This game is already ended.", endgameInfo) { }
    public ChessGameEndedException(ChessBoard board, string message, EndGameInfo endgameInfo) : base(board, message) => EndgameInfo = endgameInfo;
}


public class ChessInvalidMoveException : ChessException
{
    public Move Move { get; }
    public ChessInvalidMoveException(ChessBoard board, Move move)
        : this(board, $"Given move: {move} is invalid for current pieces positions.", move) { }
    public ChessInvalidMoveException(ChessBoard board, string message, Move move) : base(board, message) => Move = move;
}

public class ChessPieceNotFoundException : ChessException
{
    public Position Position { get; }
    public ChessPieceNotFoundException(ChessBoard board, Position position)
        : this(board, $"Piece on given position: {position} has been not found in current chess board", position) { }
    public ChessPieceNotFoundException(ChessBoard board, string message, Position position) : base(board, message) => Position = position;
}

public class ChessSanNotFoundException : ChessException
{
    public string SanMove { get; set; }
    public ChessSanNotFoundException(ChessBoard board, string san)
        : this(board, $"Given SAN move: {san} has been not found with current board positions.", san) { }
    public ChessSanNotFoundException(ChessBoard board, string message, string san) : base(board, message) => SanMove = san;
}

public class ChessSanTooAmbiguousException : ChessException
{
    public string SanMove { get; set; }
    public Move[] Moves { get; }
    public ChessSanTooAmbiguousException(ChessBoard board, string san, Move[] moves)
        : this(board, $"Given SAN move: {san} is too ambiguous between moves: {string.Join(", ", moves.Select(m => m.ToString()))}", san, moves) { }
    public ChessSanTooAmbiguousException(ChessBoard board, string message, string san, Move[] moves) : base(board, message)
    {
        SanMove = san;
        Moves = moves;
    }
}

public class ChessArgumentException : ChessException
{
    public ChessArgumentException(ChessBoard? board, string argument, string method)
        : this(board, $"An argument: {argument} in method: {method} is not valid...") { }
    public ChessArgumentException(ChessBoard? board, string message) : base(board, message) { }
}

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member