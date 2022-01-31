using BenchmarkDotNet.Attributes;
using Chess;

namespace ChessBenchmarks;

[RankColumn]
public class ChessMoveBenchmark
{
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

    //  Tests:
    //  30/01/2021 =>
    //  |              Method |       Mean |     Error |   StdDev | Rank |
    //  |-------------------- |-----------:|----------:|---------:|-----:|
    //  | MoveUsingMoveObject |   961.4 us |  45.39 us | 133.8 us |    1 | 
    //  |        MoveUsingSan | 3,967.7 us | 137.25 us | 404.7 us |    2 |
    //  
    //  31/01/2021 => // Holy fuck thats fast
    //  |              Method |      Mean |    Error |   StdDev |    Median | Rank |
    //  |-------------------- |----------:|---------:|---------:|----------:|-----:|
    //  | MoveUsingMoveObject |  74.15 us | 3.776 us | 11.13 us |  71.66 us |    1 |
    //  |        MoveUsingSan | 145.57 us | 9.105 us | 26.85 us | 136.34 us |    2 |
    //
    //
}

[RankColumn]
public class ChessGenerateMovesBenchmark
{
    [Benchmark]
    public void MovesSanFalse()
    {
        new ChessBoard().Moves(generateSan: false);
    }

    [Benchmark]
    public void MovesSanTrue()
    {
        new ChessBoard().Moves(generateSan: true);
    }

    //  Tests:
    //  30/01/2022 =>
    //  |        Method |       Mean |    Error |    StdDev | Rank |
    //  |-------------- |-----------:|---------:|----------:|-----:|
    //  | MovesSanFalse |   319.5 us | 14.17 us |  41.79 us |    1 |
    //  |  MovesSanTrue | 1,839.6 us | 56.71 us | 167.21 us |    2 |
    //
    //  31/01/2022 =>
    //  |        Method |     Mean |    Error |   StdDev | Rank |
    //  |-------------- |---------:|---------:|---------:|-----:|
    //  | MovesSanFalse | 316.8 us | 12.67 us | 37.16 us |    1 |
    //  |  MovesSanTrue | 369.8 us | 20.07 us | 59.19 us |    2 |
    //
    //
    //

}



public class ChessOnCapturedEventBenchmark
{
    readonly ChessBoard board = new();
    public ChessOnCapturedEventBenchmark()
    {
        board.OnCaptured += Board_OnCaptured;
        board.Load("1nbqkbn1/pppppppp/NpNpNpNp/pBpBpBpB/bPbPbPbP/PnPnPnPn/PPPPPPPP/1NBQKBN1 w - - 0 1");
    }

    [Benchmark]
    public void CallOnCapturedEventAsync()
    {
        //board.OnCapturedEventAsync(new Piece("wp"));
    }

    private static void Board_OnCaptured(object sender, CaptureEventArgs e)
    {
        Thread.Sleep(1);
    }

    [Benchmark]
    public void CallOnCapturedEvent()
    {
        //board.OnCapturedEvent(new Piece("wp"));
    }
}
