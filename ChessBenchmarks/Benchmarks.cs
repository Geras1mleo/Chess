using BenchmarkDotNet.Attributes;
using Chess;

namespace ChessBenchmarks;

[RankColumn]
public class ChessMoveBenchmark
{
    [Benchmark]
    public void MoveUsingSan()
    {
        var board = new ChessBoard();

        board.Move("e4");
        board.Move("e5");
        board.Move("Ne2");
        board.Move("f6");
        board.Move("Nbc3");
    }

    [Benchmark]
    public void MoveUsingMoveObject()
    {
        var board = new ChessBoard();

        board.Move(new Move(new Position("e2"), new Position("e4")));
        board.Move(new Move(new Position("e7"), new Position("e5")));
        board.Move(new Move(new Position("g1"), new Position("e2")));
        board.Move(new Move(new Position("f7"), new Position("f6")));
        board.Move(new Move(new Position("b1"), new Position("c3")));
    }
}

[RankColumn]
public class ChessGenerateMovesBenchmark
{
    [Benchmark]
    public void MovesSanTrue()
    {
        new ChessBoard().Moves(generateSan: true);
    }

    [Benchmark]
    public void MovesSanFalse()
    {
        new ChessBoard().Moves(generateSan: false);
    }
}
