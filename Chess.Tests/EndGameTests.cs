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
        // todo
    }
}

