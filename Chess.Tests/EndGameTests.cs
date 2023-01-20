using Chess;
using Xunit;

namespace ChessUnitTests;

public class EndGameTests
{
    [Theory]
    [InlineData("8/8/8/8/4Pk2/8/8/4K3 b - - 0 1")] // Level 1
    [InlineData("8/8/1b6/8/4Pk2/8/8/4K3 b - - 0 1")] // Level 2
    [InlineData("8/8/8/8/1B1bPk2/8/8/4K3 b - - 0 1")] // Level 3
    public void EndGame_InsufficientMaterial(string fen)
    {
        var board = ChessBoard.LoadFromFen(fen);
        board.Move("Kxe4");

        Assert.True(board.IsEndGame);
        Assert.Equal(EndgameType.InsufficientMaterial, board.EndGame.EndgameType);
    }

    [Theory]
    [InlineData("8/8/8/8/4Pk2/8/8/R3K3 b - - 0 1")]
    [InlineData("8/8/8/8/1Bb1Pk2/8/8/4K3 b - - 0 1")]
    [InlineData("8/8/6r1/8/1B2Pk2/8/8/4K3 b - - 0 1")]
    [InlineData("8/8/8/8/1B2Pk2/2B5/8/4K3 b - - 0 1")]
    public void EndGame_InsufficientMaterial_NotTriggered(string fen)
    {
        var board = ChessBoard.LoadFromFen(fen);
        board.Move("Kxe4");

        Assert.False(board.IsEndGame);
        Assert.Null(board.EndGame);
    }

    [Fact]
    public void EndGame_Move50Rule()
    {
        // todo
    }

    [Fact]
    public void EndGame_Repetition()
    {
        var board = ChessBoard.LoadFromFen("4k2r/8/3b4/8/8/5B2/8/R3K3 w Qk - 0 1");

        board.Move("Be4");
        board.Move("Be5");

        board.Move("Bf3");
        board.Move("Bd6");

        board.Move("Be4");
        board.Move("Be5");

        board.Move("Bf3");
        board.Move("Bd6");

        Assert.Equal(EndgameType.Repetition, board.EndGame.EndgameType);
    }

    [Fact]
    public void EndGame_Repetition_Fischer_vs_Petrosian()
    {
        var board = ChessBoard.LoadFromFen("8/pp3p1k/2p2q1p/3r1P2/5R2/7P/P1P1QP2/7K b - - 2 30");

        board.Move("Qe5");

        board.Move("Qh5");
        board.Move("Qf6");

        board.Move("Qe2");
        board.Move("Re5");

        board.Move("Qd3");
        board.Move("Rd5");

        board.Move("Qe2");

        Assert.Equal(EndgameType.Repetition, board.EndGame.EndgameType);
    }

    [Fact]
    public void EndGame_Repetition_CastleRightsChanged_With_Repetition()
    {
        var board = ChessBoard.LoadFromFen("4k2r/8/3b4/8/8/5B2/8/R3K3 w Qk - 0 1");

        // Here both kings lose castle rights
        board.Move("Rb1");
        board.Move("Rg8");

        // From here repetition begins
        board.Move("Ra1");
        board.Move("Rh8");

        board.Move("Rb1");
        board.Move("Rg8");

        board.Move("Ra1");
        board.Move("Rh8");

        board.Move("Rb1");
        board.Move("Rg8");

        Assert.Equal(EndgameType.Repetition, board.EndGame.EndgameType);
    }

    [Fact]
    public void EndGame_Repetition_CastleRightsChanged_No_Repetition()
    {
        var board = ChessBoard.LoadFromFen("4k2r/8/3b4/8/8/5B2/8/R3K3 w Qk - 0 1");

        board.Move("Be4");
        board.Move("Be5");

        board.Move("Bf3");
        board.Move("Bd6");

        // Here both kings lose castle rights
        board.Move("Rb1");
        board.Move("Rg8");

        board.Move("Ra1");
        board.Move("Rh8");

        Assert.False(board.IsEndGame);
    }
}

