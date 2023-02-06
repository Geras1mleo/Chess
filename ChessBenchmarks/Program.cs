using BenchmarkDotNet.Running;
using ChessBenchmarks;

BenchmarkRunner.Run<ChessMoveBenchmark>();
BenchmarkRunner.Run<ChessGenerateMovesBenchmark>();
BenchmarkRunner.Run<ChessIsValidMoveBenchmark>();

BenchmarkRunner.Run<ChessFenConversionsBenchmark>();
BenchmarkRunner.Run<ChessPgnConversionsBenchmark>();
BenchmarkRunner.Run<ChessOverallBenchmark>();
BenchmarkRunner.Run<ChessFenBuilderBenchmark>();
