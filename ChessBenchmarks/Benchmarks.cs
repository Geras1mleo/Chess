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
    //  31/01/2021 => holy fuck thats fast
    //  |              Method |      Mean |    Error |   StdDev | Rank |
    //  |-------------------- |----------:|---------:|---------:|-----:|
    //  | MoveUsingMoveObject |  74.15 us | 3.776 us | 11.13 us |    1 |
    //  |        MoveUsingSan | 145.57 us | 9.105 us | 26.85 us |    2 |
    //
    //  02/02/2022 => 
    //  |              Method |      Mean |    Error |    StdDev | Rank |
    //  |-------------------- |----------:|---------:|----------:|-----:|
    //  | MoveUsingMoveObject |  70.27 us | 2.723 us |  7.985 us |    1 |
    //  |        MoveUsingSan | 126.91 us | 4.889 us | 14.337 us |    2 |
    //  
    //  13/02/2022 => much logic added // todo OPTIMIZE that shit
    //  |              Method |     Mean |   Error |   StdDev | Rank |
    //  |-------------------- |---------:|--------:|---------:|-----:|
    //  | MoveUsingMoveObject | 145.2 us | 3.89 us | 11.28 us |    1 |
    //  |        MoveUsingSan | 234.3 us | 8.48 us | 24.61 us |    2 |
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
    //  31/01/2022 => very good
    //  |        Method |     Mean |    Error |   StdDev | Rank |
    //  |-------------- |---------:|---------:|---------:|-----:|
    //  | MovesSanFalse | 316.8 us | 12.67 us | 37.16 us |    1 |
    //  |  MovesSanTrue | 369.8 us | 20.07 us | 59.19 us |    2 |
    //
    //  02/02/2022 => still worthy
    //  |        Method |     Mean |    Error |   StdDev | Rank |
    //  |-------------- |---------:|---------:|---------:|-----:|
    //  | MovesSanFalse | 317.3 us | 16.32 us | 48.12 us |    1 |
    //  |  MovesSanTrue | 325.2 us |  8.84 us | 25.52 us |    2 |
    //
    //  13/02/2022 => needs optimization
    //  |        Method |     Mean |   Error |   StdDev | Rank |
    //  |-------------- |---------:|--------:|---------:|-----:|
    //  | MovesSanFalse | 469.5 us | 9.24 us | 15.44 us |    1 |
    //  |  MovesSanTrue | 494.9 us | 9.84 us | 18.48 us |    2 |
    //
}

public class ChessIsValidMoveBenchmark
{
    [Benchmark]
    public void IsValidMove()
    {
        var board = new ChessBoard();
        board.IsValidMove(new Move(new("b1"), new("c3")));
        board.IsValidMove(new Move(new("c1"), new("g5")));
        board.IsValidMove(new Move(new("d1"), new("d6")));
        board.IsValidMove(new Move(new("e1"), new("f2")));
        board.IsValidMove(new Move(new("e2"), new("e4")));
        board.IsValidMove(new Move(new("g2"), new("g4")));
        board.IsValidMove(new Move(new("b2"), new("b4")));
    }

    //  Tests:
    //  12/02/2022 using => object == null and object != null
    //  |      Method |     Mean |    Error |   StdDev |
    //  |------------ |---------:|---------:|---------:|
    //  | IsValidMove | 39.35 us | 1.109 us | 3.253 us |
    //  
    //  12/02/2022 using => object is null and object is not null
    //  |      Method |     Mean |    Error |   StdDev |
    //  |------------ |---------:|---------:|---------:|
    //  | IsValidMove | 38.98 us | 1.036 us | 3.056 us |
    //
    //  Conclusion:
    //  Pretty much the same performance
    //
    //  13/02/2022 => slow af =_=
    //  |      Method |     Mean |    Error |   StdDev |
    //  |------------ |---------:|---------:|---------:|
    //  | IsValidMove | 98.77 us | 4.090 us | 12.06 us |
    //
}