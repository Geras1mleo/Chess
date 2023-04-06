using Chess;
using Xunit;

namespace ChessUnitTests;

public class EndGameTests
{
    [Fact]
    public void EndGame_InsufficientMaterial_Basic()
    {
        var board = ChessBoard.LoadFromFen("4k3/8/8/8/8/8/8/4K3 b - - 0 1", AutoEndgameRules.All);

        Assert.Equal(EndgameType.InsufficientMaterial, board.EndGame.EndgameType);
    }

    [Theory]
    [InlineData("8/8/8/8/4Pk2/8/8/4K3 b - - 0 1")] // Level 1
    [InlineData("8/8/1b6/8/4Pk2/8/8/4K3 b - - 0 1")] // Level 2
    [InlineData("8/8/8/8/1B1bPk2/8/8/4K3 b - - 0 1")] // Level 3
    [InlineData("8/8/8/8/1n1nPk2/8/8/4K3 b - - 0 1")] // Level 3
    public void EndGame_InsufficientMaterial(string fen)
    {
        var board = ChessBoard.LoadFromFen(fen, AutoEndgameRules.All);

        board.Move("Kxe4");

        Assert.Equal(EndgameType.InsufficientMaterial, board.EndGame.EndgameType);
    }

    [Theory]
    [InlineData("8/8/8/8/4Pk2/8/8/R3K3 b - - 0 1")]
    [InlineData("8/8/8/8/1Bb1Pk2/8/8/4K3 b - - 0 1")]
    [InlineData("8/8/6r1/8/1B2Pk2/8/8/4K3 b - - 0 1")]
    [InlineData("8/8/8/8/1B2Pk2/2B5/8/4K3 b - - 0 1")]
    [InlineData("8/8/8/8/1n1NPk2/8/8/4K3 b - - 0 1")]
    public void EndGame_InsufficientMaterial_Ignored(string fen)
    {
        var board = ChessBoard.LoadFromFen(fen, AutoEndgameRules.All);

        board.Move("Kxe4");

        Assert.False(board.IsEndGame);
    }

    [Theory] // Anna Ushenina vs Olga Girya
    [InlineData("8/8/1N6/5B2/8/4K3/8/2k5 w - - 97 121", "Na4", "Kd1", "Nb2+")]
    [InlineData("8/8/8/5B2/2N5/3K4/8/3k4 b - - 104 124")]
    public void EndGame_FiftyMoveRule(string fen, params string[] moves)
    {
        var board = ChessBoard.LoadFromFen(fen, AutoEndgameRules.All);

        foreach (var move in moves)
            board.Move(move);

        Assert.Equal(EndgameType.FiftyMoveRule, board.EndGame.EndgameType);
    }

    [Theory]
    [InlineData("4k2r/8/3b4/8/8/5B2/8/R3K3 w Qk - 0 1", "Be4", "Be5", "Bf3", "Bd6", "Be4", "Be5", "Bf3", "Bd6")]
    [InlineData("8/pp3p1k/2p2q1p/3r1P2/5R2/7P/P1P1QP2/7K b - - 2 30", "Qe5", "Qh5", "Qf6", "Qe2", "Re5", "Qd3", "Rd5", "Qe2")] // Fischer vs Petrosian
    public void EndGame_Repetition(string fen, params string[] moves)
    {
        var board = ChessBoard.LoadFromFen(fen, AutoEndgameRules.All);

        foreach (var move in moves)
            board.Move(move);

        Assert.Equal(EndgameType.Repetition, board.EndGame.EndgameType);
    }

    [Fact]
    public void EndGame_Repetition_CastleRightsChanged()
    {
        var board = ChessBoard.LoadFromFen("4k2r/8/3b4/8/8/5B2/8/R3K3 w Qk - 0 1", AutoEndgameRules.All);

        // Here both kings lose castle rights
        var moves1 = new[] { "Rb1", "Rg8" };

        // From here repetition begins
        var moves2 = new[] { "Ra1", "Rh8", "Rb1", "Rg8", "Ra1", "Rh8", "Rb1", "Rg8" };

        foreach (var move in moves1)
            board.Move(move);

        foreach (var move in moves2)
            board.Move(move);

        Assert.Equal(EndgameType.Repetition, board.EndGame.EndgameType);
    }

    [Fact]
    public void EndGame_Repetition_Ignored_Due_CastleRightsChanged()
    {
        var board = ChessBoard.LoadFromFen("4k2r/8/3b4/8/8/5B2/8/R3K3 w Qk - 0 1", AutoEndgameRules.All);

        var moves1 = new[] { "Be4", "Be5", "Bf3", "Bd6" };

        // Here both kings lose castle rights
        var moves2 = new[] { "Rb1", "Rg8", "Ra1", "Rh8" };

        foreach (var move in moves1)
            board.Move(move);

        foreach (var move in moves2)
            board.Move(move);

        Assert.False(board.IsEndGame);
    }

    [Fact]
    public void EndGame_Repetition_Ignored_Due_EnPassant()
    {
        var board = ChessBoard.LoadFromFen("4k2r/8/8/3Pp3/8/8/8/R3K3 w - e6 0 2", AutoEndgameRules.All);

        var moves = new[] { "Rb1", "Rg8", "Ra1", "Rh8", "Rb1", "Rg8", "Ra1", "Rh8" };

        foreach (var move in moves)
            board.Move(move);

        Assert.False(board.IsEndGame);
    }

    [Fact]
    public void EndGame_Stalemate_FromFen()
    {
        var board = ChessBoard.LoadFromFen("1r5k/8/3b4/8/8/8/7r/K7 b - - 0 1", AutoEndgameRules.All);
        Assert.False(board.IsEndGame); // black has moves, and it's black's turn
        Assert.Null(board.EndGame);

        board = ChessBoard.LoadFromFen("1r5k/8/3b4/8/8/8/7r/K7 w - - 0 1", AutoEndgameRules.All);
        Assert.True(board.IsEndGame); // same board but with white to move, it's a stalemate
        Assert.Equal(EndgameType.Stalemate, board.EndGame.EndgameType);
    }

    [Fact]
    public void EndGame_Checkmate_CastlingEscape()
    {
        var board = ChessBoard.LoadFromFen("rn1qk2r/ppp1bQpp/5n2/4N3/3pP3/8/PP3PPP/RNB1K2R b KQ - 0 1", AutoEndgameRules.All);
        Assert.True(board.IsEndGame);
        Assert.NotNull(board.EndGame);
        Assert.Equal(EndgameType.Checkmate, board.EndGame.EndgameType);
        Assert.Equal(PieceColor.White, board.EndGame.WonSide);
    }
}

