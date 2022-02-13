using BenchmarkDotNet.Running;
using ChessBenchmarks;

BenchmarkRunner.Run<ChessMoveBenchmark>();
BenchmarkRunner.Run<ChessGenerateMovesBenchmark>();
BenchmarkRunner.Run<ChessIsValidMoveBenchmark>();

Console.ReadLine();