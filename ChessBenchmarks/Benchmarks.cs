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

        board.Move(new Move("e2", "e4"));
        board.Move(new Move("e7", "e5"));
        board.Move(new Move("g1", "e2"));
        board.Move(new Move("f7", "f6"));
        board.Move(new Move("b1", "c3"));
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
    //  13/02/2022 => much logic added // need to OPTIMIZE that shit
    //  |              Method |     Mean |   Error |   StdDev | Rank |
    //  |-------------------- |---------:|--------:|---------:|-----:|
    //  | MoveUsingMoveObject | 145.2 us | 3.89 us | 11.28 us |    1 |
    //  |        MoveUsingSan | 234.3 us | 8.48 us | 24.61 us |    2 |
    //
    //  20/02/2022 => a bit better
    //  |              Method |     Mean |   Error |   StdDev | Rank |
    //  |-------------------- |---------:|--------:|---------:|-----:|
    //  | MoveUsingMoveObject | 141.8 us | 7.93 us | 23.39 us |    1 |
    //  |        MoveUsingSan | 193.9 us | 8.04 us | 23.32 us |    2 |
    //
    //  27/02/2022 => 
    //  |              Method |     Mean |   Error |  StdDev | Rank |
    //  |-------------------- |---------:|--------:|--------:|-----:|
    //  | MoveUsingMoveObject | 125.9 us | 2.32 us | 4.59 us |    1 |
    //  |        MoveUsingSan | 187.2 us | 3.50 us | 3.28 us |    2 |
    //
    //  1/06/2022 => not bad
    //  |              Method |     Mean |   Error |  StdDev | Rank |
    //  |-------------------- |---------:|--------:|--------:|-----:|
    //  | MoveUsingMoveObject | 117.5 us | 3.30 us | 9.72 us |    1 |
    //  |        MoveUsingSan | 152.0 us | 3.15 us | 8.98 us |    2 |
    //  
    //  6/02/2023 => Spans are so good... 
    //  |              Method |     Mean |    Error |   StdDev | Rank |
    //  |-------------------- |---------:|---------:|---------:|-----:|
    //  | MoveUsingMoveObject | 47.77 us | 0.687 us | 0.609 us |    1 |
    //  |        MoveUsingSan | 77.72 us | 1.236 us | 0.965 us |    2 |
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
    //  20/02/2022 => bruh... why??
    //  |        Method |     Mean |    Error |   StdDev | Rank |
    //  |-------------- |---------:|---------:|---------:|-----:|
    //  | MovesSanFalse | 667.5 us | 39.11 us | 113.5 us |    1 |
    //  |  MovesSanTrue | 745.0 us | 36.14 us | 105.4 us |    2 |
    //
    //  27/02/2022 => 
    //  |        Method |     Mean |    Error |   StdDev | Rank |
    //  |-------------- |---------:|---------:|---------:|-----:|
    //  | MovesSanFalse | 573.3 us | 11.05 us | 20.21 us |    1 |
    //  |  MovesSanTrue | 584.1 us | 17.18 us | 49.29 us |    2 |
    //
    //  27/05/2022 => async approach
    //
    //  |        Method |     Mean |    Error |   StdDev | Rank |
    //  |-------------- |---------:|---------:|---------:|-----:|
    //  | MovesSanFalse | 492.7 us | 16.54 us | 46.93 us |    1 |
    //  |  MovesSanTrue | 528.3 us | 14.13 us | 39.86 us |    2 |
    //
    //  1/06/2022 => fuck yeah
    //  |        Method |     Mean |    Error |   StdDev | Rank |
    //  |-------------- |---------:|---------:|---------:|-----:|
    //  | MovesSanFalse | 321.7 us | 15.32 us | 44.68 us |    1 |
    //  |  MovesSanTrue | 341.2 us |  7.88 us | 21.83 us |    2 |
    //
    //  1/06/2022 2.0 => going fast as fuck
    //  |        Method |     Mean |    Error |   StdDev |   Median | Rank |
    //  |-------------- |---------:|---------:|---------:|---------:|-----:|
    //  | MovesSanFalse | 308.5 us | 11.98 us | 33.39 us | 298.5 us |    1 |
    //  |  MovesSanTrue | 360.5 us | 16.82 us | 47.43 us | 359.3 us |    2 |
    //
    //  6/02/2023 => x2 faster
    //  |        Method |     Mean |   Error |  StdDev | Rank |
    //  |-------------- |---------:|--------:|--------:|-----:|
    //  | MovesSanFalse | 151.3 us | 2.79 us | 3.73 us |    1 |
    //  |  MovesSanTrue | 169.1 us | 3.29 us | 4.28 us |    2 |
}

[MemoryDiagnoser]
public class ChessIsValidMoveBenchmark
{
    [Benchmark]
    public void IsValidMove()
    {
        var board = new ChessBoard();
        board.IsValidMove(new Move("b1", "c3"));
        board.IsValidMove(new Move("c1", "g5"));
        board.IsValidMove(new Move("d1", "d6"));
        board.IsValidMove(new Move("e1", "f2"));
        board.IsValidMove(new Move("e2", "e4"));
        board.IsValidMove(new Move("g2", "g4"));
        board.IsValidMove(new Move("b2", "b4"));
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
    //  20/02/2022 => alright then...
    //  |      Method |     Mean |   Error |   StdDev |
    //  |------------ |---------:|--------:|---------:|
    //  | IsValidMove | 100.2 us | 4.67 us | 13.76 us |
    //
    //  27/02/2022 => 
    //  |      Method |     Mean |    Error |   StdDev |
    //  |------------ |---------:|---------:|---------:|
    //  | IsValidMove | 84.19 us | 2.101 us | 6.029 us |
    //
    //  1/06/2022 => very good
    //  |      Method |     Mean |    Error |   StdDev |
    //  |------------ |---------:|---------:|---------:|
    //  | IsValidMove | 69.73 us | 1.385 us | 3.210 us |
    //  
    //  6/02/2023 => x2.3 faster
    //  |      Method |     Mean |    Error |   StdDev |  Gen 0 | Allocated |
    //  |------------ |---------:|---------:|---------:|-------:|----------:|
    //  | IsValidMove | 30.25 us | 0.422 us | 0.374 us | 0.3967 |     31 KB |
}

[MemoryDiagnoser]
public class ChessFenConversionsBenchmark
{
    [Benchmark]
    public void FenConversion()
    {
        ChessBoard.LoadFromFen("1nbqkbn1/pppppppp/NpNpNpNp/pBpBpBpB/bPbPbPbP/PnPnPnPn/PPPPPPPP/1NBQKBN1 w - - 0 1");
        ChessBoard.LoadFromFen("rnbqkbnr/ppp1pppp/8/8/3pP3/8/PPPP1PPP/RNBQKBNR b KQkq e3 0 1");
        ChessBoard.LoadFromFen("r3kbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQK2R w KQkq - 0 1");
        ChessBoard.LoadFromFen("rnb1kbnr/pppppppp/8/8/8/8/5q2/7K b kq - 0 1");
    }

    //  Tests:
    //  20/02/2022 => before optimizations
    //  |        Method |     Mean |    Error |   StdDev |    Gen 0 | Allocated |
    //  |-------------- |---------:|---------:|---------:|---------:|----------:|
    //  | FenConversion | 656.7 us | 29.29 us | 86.37 us | 168.9453 |    346 KB |
    //
    //  20/02/2022 => after optimizations Regex cached and compiled
    //  |        Method |     Mean |    Error |   StdDev |    Gen 0 | Allocated |
    //  |-------------- |---------:|---------:|---------:|---------:|----------:|
    //  | FenConversion | 416.8 us | 11.61 us | 33.69 us | 169.4336 |    346 KB |
    // 
    //  20/02/2022 =>
    //  |        Method |     Mean |    Error |   StdDev |    Gen 0 | Allocated |
    //  |-------------- |---------:|---------:|---------:|---------:|----------:|
    //  | FenConversion | 504.8 us | 35.08 us | 99.52 us | 203.1250 |    415 KB |
    //
    //  27/02/2022 => 
    //  |        Method |     Mean |   Error |  StdDev |    Gen 0 | Allocated |
    //  |-------------- |---------:|--------:|--------:|---------:|----------:|
    //  | FenConversion | 467.4 us | 8.25 us | 8.10 us | 202.1484 |    414 KB |
    //
    //  1/06/2022 => not a huge difference
    //  |        Method |     Mean |    Error |   StdDev |    Gen 0 | Allocated |
    //  |-------------- |---------:|---------:|---------:|---------:|----------:|
    //  | FenConversion | 481.1 us | 21.65 us | 63.85 us | 208.0078 |    426 KB |
    //
    //  21/01/2023
    //      => before kings constraint in fen import
    //  |        Method |     Mean |   Error |  StdDev |  Gen 0 | Allocated |
    //  |-------------- |---------:|--------:|--------:|-------:|----------:|
    //  | FenConversion | 426.9 us | 8.12 us | 8.69 us | 4.8828 |    427 KB |
    //
    //      => constraint using regex
    //  |        Method |     Mean |   Error |  StdDev |  Gen 0 | Allocated |
    //  |-------------- |---------:|--------:|--------:|-------:|----------:|
    //  | FenConversion | 430.6 us | 6.55 us | 5.81 us | 4.8828 |    427 KB |
    //
    //      => constraint using piecesList extension method
    //  |        Method |     Mean |   Error |  StdDev |  Gen 0 | Allocated |
    //  |-------------- |---------:|--------:|--------:|-------:|----------:|
    //  | FenConversion | 432.0 us | 8.29 us | 7.76 us | 4.8828 |    430 KB |
    //
    //  6/02/2023 => x3 faster and 4x less memory allocated
    //  |        Method |     Mean |   Error |  StdDev |  Gen 0 | Allocated |
    //  |-------------- |---------:|--------:|--------:|-------:|----------:|
    //  | FenConversion | 129.0 us | 1.05 us | 0.99 us | 1.4648 |    108 KB |
}

[MemoryDiagnoser]
public class ChessPgnConversionsBenchmark
{
    [Benchmark]
    public void PgnConversion()
    {
        ChessBoard.LoadFromPgn(
            @"[Event ""Live Chess""]
        [Site ""Chess.com""]
        [Date ""2022.01.11""]
        [Round ""?""]
        [White ""Milan1905""]
        [Black ""Geras1mleo""]
        [Result ""1-0""]
        [ECO ""C47""]
        [WhiteElo ""1006""]
        [BlackElo ""626""]
        [TimeControl ""600""]
        [EndTime ""11:58:56 PST""]
        [Termination ""Milan1905 won by resignation""]
            
        1.e4 e5 2.Nf3 Nf6 3.Nc3 Nc6 4.Bb5 Bc5 5.Bxc6 bxc6 6.Nxe5 Bxf2+ 7.Kxf2 O-O
        8.d4 d5 9.exd5 cxd5 10.Nc6 Ng4+ 11.Kg1 Qf6 12.Qf1 Qxc6 13.h3 Nf6 14.Bg5
        Qb6 15.Bxf6 Qxf6 16.Qxf6 gxf6 17.Nxd5 Rb8 18.Nxf6+ Kh8 19.b3 Rb4 20.c3 Bb7
        21.cxb4 1-0");

        ChessBoard.LoadFromPgn(
            @"[Event ""Live Chess""]
        [Site ""Chess.com""]
        [Date ""2022.01.03""]
        [Round ""?""]
        [White ""Milan1905""]
        [Black ""Geras1mleo""]
        [Result ""1/2-1/2""]
        [ECO ""C42""]
        [WhiteElo ""1006""]
        [BlackElo ""626""]
        [TimeControl ""600""]
        [EndTime ""9:19:18 PST""]
        [Termination ""Game drawn by insufficient material""]
            
        1.e4 e5 2.Nf3 Nf6 3.Nxe5 Nxe4 4.Qe2 d5 5.d3 Bd6 6.dxe4 Bxe5 7.exd5 Qxd5
        8.c4 Qc5 9.Bf4 Nd7 10.Bxe5 Nxe5 11.Nc3 O-O 12.O-O-O Nxc4 13.Na4 Qc6 14.
        Qxc4 Bg4 15.Qxc6 bxc6 16.Rd2 Rad8 17.Rxd8 Rxd8 18.Ba6 h6 19.h3 Bf5 20.g4
        Be4 21.Re1 Rd4 22.Nc3 Bf3 23.Re8+ Kh7 24.Ne2 Bxe2 25.Bxe2 Rf4 26.Bd3+ g6
        27.Re2 f5 28.gxf5 gxf5 29.Re7+ Kg6 30.Rxc7 Rf3 31.Rxc6+ Kg5 32.h4+ Kxh4
        33.Rxh6+ Kg5 34.Rh2 Rxd3 35.Kc2 Rf3 36.Rg2+ Kh4 37.Kd2 Kh3 38.Rg8 Rxf2+
        39.Kc3 Rf3+ 40.Kb4 Rf4+ 41.Ka3 Rf3+ 42.b3 a5 43.Ka4 Rf2 44.a3 Rf4+ 45.b4
        axb4 46.axb4 Rg4 47.Rh8+ Rh4 48.Rxh4+ Kxh4 49.b5 f4 50.b6 f3 51.b7 f2 52.
        b8=Q f1=Q 53.Qb4+ Kg3 54.Qb3+ Qf3 55.Qxf3+ Kxf3 1/2-1/2");
    }

    //  Tests:
    //  20/02/2022 => before optimizations
    //  |        Method |     Mean |     Error |    StdDev |     Gen 0 | Allocated |
    //  |-------------- |---------:|----------:|----------:|----------:|----------:|
    //  | PgnConversion | 8.115 ms | 0.3013 ms | 0.8836 ms | 2179.6875 |      4 MB |
    //
    //  20/02/2022 => after optimizations Regex cached and compiled
    //  |        Method |     Mean |     Error |   StdDev |     Gen 0 | Allocated |
    //  |-------------- |---------:|----------:|---------:|----------:|----------:|
    //  | PgnConversion | 6.876 ms | 0.3544 ms | 1.045 ms | 2179.6875 |      4 MB |
    // 
    //  20/02/2022 => after optimizations San with StringBuilder
    //  |        Method |     Mean |     Error |    StdDev |     Gen 0 | Allocated |
    //  |-------------- |---------:|----------:|----------:|----------:|----------:|
    //  | PgnConversion | 7.360 ms | 0.2578 ms | 0.7601 ms | 2195.3125 |      4 MB |
    // 
    //  Conclusion: still too much memory usage
    //
    //  27/02/2022 => 
    //  |        Method |     Mean |     Error |    StdDev |     Gen 0 | Allocated |
    //  |-------------- |---------:|----------:|----------:|----------:|----------:|
    //  | PgnConversion | 7.918 ms | 0.2930 ms | 0.8641 ms | 2562.5000 |      5 MB |
    //
    //  1/06/2022 => HOLLY SHIT THAT ONE IS GOOOOD
    //  |        Method |     Mean |     Error |    StdDev |    Gen 0 | Allocated |
    //  |-------------- |---------:|----------:|----------:|---------:|----------:|
    //  | PgnConversion | 2.176 ms | 0.1057 ms | 0.3101 ms | 761.7188 |      2 MB |
    //
    //  6/02/2023 => almost x2 faster AND less allocations!!
    //  |        Method |     Mean |     Error |    StdDev |   Gen 0 | Allocated |
    //  |-------------- |---------:|----------:|----------:|--------:|----------:|
    //  | PgnConversion | 1.256 ms | 0.0159 ms | 0.0149 ms | 13.6719 |      1 MB |
}

[MemoryDiagnoser]
public class ChessOverallBenchmark
{
    [Benchmark]
    public void SampleGame()
    {
        var board = ChessBoard.LoadFromPgn(@"
        [White ""AG1612""]
        [Black ""GrafLermontov""]
        [Variant ""From Position""]
        [FEN ""r2k3r/ppp1bpp1/2p1b3/4PnNp/5B2/2N4P/PPP2PP1/R4RK1 b - - 1 12""]
        12... Ke8 13. Rad1 Rh6 14. Nce4 Bd5 15. b3 Rg6 16. c4 Be6", AutoEndgameRules.All);

        var moves = new[] { "Nxe6", "fxe6", "Kh2", "Rd8", "g3", "Nd4", "Rd3", "c5", "Rfd1", "Nf3+", "Kg2", "Rxd3" };

        foreach (var move in moves)
        {
            var generatedMoves = board.Moves();
            var succeed = board.Move(move);
        }

        board.Resign(PieceColor.White);

        var pgn = board.ToPgn();
        var fen = board.ToFen();
    }

    // 4/02/2023
    // |     Method |     Mean |     Error |    StdDev |   Gen 0 |   Gen 1 | Allocated |
    // |----------- |---------:|----------:|----------:|--------:|--------:|----------:|
    // | SampleGame | 4.711 ms | 0.0670 ms | 0.0594 ms | 93.7500 | 31.2500 |      5 MB |
    //
    // 4/02/2023 => after some changes
    // |     Method |     Mean |     Error |    StdDev |   Gen 0 |   Gen 1 | Allocated |
    // |----------- |---------:|----------:|----------:|--------:|--------:|----------:|
    // | SampleGame | 4.906 ms | 0.0906 ms | 0.0847 ms | 93.7500 | 31.2500 |      5 MB |
    //
    // 5/02/2023 => looks good
    // |     Method |     Mean |     Error |    StdDev |   Gen 0 |   Gen 1 | Allocated |
    // |----------- |---------:|----------:|----------:|--------:|--------:|----------:|
    // | SampleGame | 4.200 ms | 0.0590 ms | 0.0523 ms | 85.9375 | 23.4375 |      5 MB |
    //
    //  6/02/2023 => slightly worse but not critical 
    //  |     Method |     Mean |     Error |    StdDev |   Gen 0 |   Gen 1 | Allocated |
    //  |----------- |---------:|----------:|----------:|--------:|--------:|----------:|
    //  | SampleGame | 4.270 ms | 0.0631 ms | 0.0527 ms | 78.1250 | 23.4375 |      5 MB |
}

[MemoryDiagnoser]
public class ChessFenBuilderBenchmark
{
    private readonly ChessBoard _board;
    private readonly ChessBoard _board2;

    public ChessFenBuilderBenchmark()
    {
        _board = ChessBoard.LoadFromFen("r1q1k1r1/p1p1p1p1/b1n1n1b1/1p1p1p1p/P1P1P1P1/B1N1N1B1/1P1P1P1P/R1Q1K1R1 w KQkq g3 100 100");
        _board2 = ChessBoard.LoadFromFen("8/pp3p1k/2p2q1p/3r1P2/5R2/7P/P1P1QP2/7K b - - 2 30");
    }

    [Benchmark]
    public void ToFenUsingSpan()
    {
        _board.ToFen();
        _board2.ToFen();
    }

    // [Benchmark]
    // public void ToFenUsingSpan()
    // {
    //     _board.ToFenUsingSpan();
    //     _board2.ToFenUsingSpan();
    // }

    //  6/02/2023
    // |                  Method |     Mean |     Error |    StdDev |  Gen 0 | Allocated |
    // |------------------------ |---------:|----------:|----------:|-------:|----------:|
    // | ToFenUsingStringBuilder | 3.844 us | 0.0050 us | 0.0039 us | 0.0229 |      2 KB |
    // |          ToFenUsingSpan | 2.489 us | 0.0065 us | 0.0058 us | 0.0114 |      1 KB |
    //
    //  6/02/2023 => after spans
    //  |         Method |     Mean |     Error |    StdDev |  Gen 0 | Allocated |
    //  |--------------- |---------:|----------:|----------:|-------:|----------:|
    //  | ToFenUsingSpan | 2.667 us | 0.0220 us | 0.0206 us | 0.0153 |      1 KB |
}