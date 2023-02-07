![knight](https://user-images.githubusercontent.com/67554762/171267199-45341351-5968-4f68-802d-2e80136ea4ab.png)

# Gera Chess Library

Develop your chess app with C# lib and &hearts; from Geras1mleo

![GitHub last commit](https://img.shields.io/github/last-commit/geras1mleo/chess?label=Last%20commit&logo=git&style=for-the-badge) [![Nuget](https://img.shields.io/nuget/dt/gera.chess?logo=nuget&style=for-the-badge&label=Downloads&logoColor=blue)](https://www.nuget.org/packages/Gera.Chess/)

The library includes a **ChessBoard** with a 2-dimensional array of **Pieces** and the ability to ***generate***, ***validate***, and ***execute*** moves with ease. You can also parse **Move** objects into Standard Algebraic Notation **[(SAN)](https://en.wikipedia.org/wiki/Algebraic_notation_(chess))** and back. Plus, you can load and play a chess game from Forsyth-Edwards Notation **[(FEN)](https://en.wikipedia.org/wiki/Forsyth%E2%80%93Edwards_Notation)** and Portable Game Notation **[(PGN)](https://en.wikipedia.org/wiki/Portable_Game_Notation)** and vice versa.

Get ready for an eventful chess programming experience with the Gera Chess Library. The library has event handlers that are raised for an **invalid move** that places the king in **check**, change in **king checked status**, when a pawn is **promoted** with ability to specify new **promoted piece**, and when the end game is declared.

You can also **navigate** between **executed moves**, **cancel** the last executed move, and **declare a draw or resign** for one of the sides.

Since recent version [FIDE](https://handbook.fide.com/chapter/E012018) and [chess.com](https://www.chess.com/article/view/how-chess-games-can-end-8-ways-explained) **end game rules** such as: **InsufficientMaterial**, **FiftyMoveRule** and **Repetition** are available. These rules are optional and can be specified with **AutoEndgameRules** property flags.

# Usage!

Example simple **console** chess game:

```csharp
using Chess;

var board = new ChessBoard();

while (!board.IsEndGame)
{
    Console.WriteLine(board.ToAscii());
    board.Move(Console.ReadLine());
}

Console.WriteLine(board.ToAscii());
Console.WriteLine(board.ToPgn());

// Outcome after last move:
// Qh5
//   ┌────────────────────────┐
// 8 │ r  n  b  q  k  b  n  r │
// 7 │ p  p  p  p  p  .  .  p │
// 6 │ .  .  .  .  .  .  .  . │
// 5 │ .  .  .  .  .  p  p  Q │
// 4 │ .  .  .  .  P  P  .  . │
// 3 │ .  .  .  .  .  .  .  . │
// 2 │ P  P  P  P  .  .  P  P │
// 1 │ R  N  B  .  K  B  N  R │
//   └────────────────────────┘
//     a  b  c  d  e  f  g  h
//
// 1. e4 f5 2. f4 g5 3. Qh5# 1-0
```

Example **random** chess game:

```csharp
using Chess;

var board = new ChessBoard() { AutoEndgameRules = AutoEndgameRules.All };

while (!board.IsEndGame)
{
    var moves = board.Moves();
    board.Move(moves[Random.Shared.Next(moves.Length)]);
}

Console.WriteLine(board.ToAscii());
Console.WriteLine(board.ToPgn());

```

## Track Pieces

Keep track pieces on board using C# **[indexers](https://learn.microsoft.com/en-Us/dotnet/csharp/programming-guide/indexers/)**:

```csharp
board["c2"]     // => White Pawn
board['g',8]    // => Black Bishop

// Coordinates counting from 0
board[0, 0]                   // => White Rook
// Custom Position object
board[new Position(4, 7)]     // => Black King
```

Keep track of all **captured pieces**:

```csharp
board.CapturedWhite // => White pieces that has been captured by black player
board.CapturedBlack // => Black pieces that has been captured by white player
```
Properties above also include captured pieces when board has been loaded from **FEN**.

Track **kings** and their state (checked/unchecked):

```csharp
board.WhiteKing // => White king position on chess board
board.BlackKing // => Black king position on chess board

board.WhiteKingChecked // => State of White king
board.BlackKingChecked // => State of Black king
```
`board.WhiteKing` and `board.BlackKing` are computed properties, the positions are determined at the time of invocation. On the other hand, `board.WhiteKingChecked` and `board.BlackKingChecked` are cached properties that are set after each move.

## Move Pieces

Move pieces using **SAN/LAN**:

```csharp
board.Move("e4");        // => Good
board.Move("N-f6");      // => Good
board.Move("NXf6");      // => Good
board.Move("dxc3 e.p."); // => Good
board.Move("Pe4");       // => Good
board.Move("Pe5xd6");    // => Good
board.Move("O-O-O+");    // => Good

board.Move("ne5");  // => Bad
board.Move("e8=K"); // => Bad
board.Move("0-0");  // => Bad
```

Move pieces using **Move object** and corresponding positions:

```csharp
board.Move(new Move("b1", "c3"));
```

**Ambiguity**:

```csharp
if(ChessBoard.TryLoadFromPgn("1. e4 e5 2. Ne2 f6", out var board))
{
  board.ToAscii();
  //   ┌────────────────────────┐
  // 8 │ r  n  b  q  k  b  n  r │
  // 7 │ p  p  p  p  .  .  p  p │
  // 6 │ .  .  .  .  .  p  .  . │
  // 5 │ .  .  .  .  p  .  .  . │
  // 4 │ .  .  .  .  P  .  .  . │
  // 3 │ .  .  .  .  .  .  .  . │
  // 2 │ P  P  P  P  N  P  P  P │
  // 1 │ R  N  B  Q  K  B  .  R │
  //   └────────────────────────┘
  //     a  b  c  d  e  f  g  h

  board.Move("Nc3"); 	// => Throws exception: ChessSanTooAmbiguousException. Both knights can move to c3
  board.Move("Nc4"); 	// => Throws exception: ChessSanNotFoundException. None of knights can move to c4
}
```

## Load Chess game/board

Load chess board Variant: **From Position** (using **FEN**):

```csharp
board = ChessBoard.LoadFromFen("1nbqkb1r/pppp1ppp/2N5/4p3/3P4/8/PPP1PPPP/RN2KB1R w KQk - 0 1");
board.ToAscii();
//   ┌────────────────────────┐
// 8 │ .  n  b  q  k  b  .  r │
// 7 │ p  p  p  p  .  p  p  p │
// 6 │ .  .  N  .  .  .  .  . │
// 5 │ .  .  .  .  p  .  .  . │
// 4 │ .  .  .  P  .  .  .  . │
// 3 │ .  .  .  .  .  .  .  . │
// 2 │ P  P  P  .  P  P  P  P │
// 1 │ R  N  .  .  K  B  .  R │
//   └────────────────────────┘
//     a  b  c  d  e  f  g  h

board.CapturedWhite // => { White Bishop, White Queen }
board.CapturedBlack // => { Black Rook, Black Knight }

// Stalemate
board.LoadFen("rnb1kbnr/pppppppp/8/8/8/8/5q2/7K w kq - 0 1");
board.EndGame // => { EndgameType = Stalemate, WonSide = null }
```

Load full chess game from **PGN**:

```csharp
board = ChessBoard.LoadFromPgn(
@"[Variant ""From Position""]
[FEN ""rnbqkbnr/ppp1pppp/8/3p4/4P3/8/PPPP1PPP/RNBQKBNR w KQkq - 0 1""]

1.exd5 e6 2.dxe6 fxe6 3.d4(3.f4 g5 4.fxg5) 3... c5 4.b4");

board.ToAscii();
//   ┌────────────────────────┐
// 8 │ r  n  b  q  k  b  n  r │
// 7 │ p  p  .  .  .  .  p  p │
// 6 │ .  .  .  .  p  .  .  . │
// 5 │ .  .  p  .  .  .  .  . │
// 4 │ .  P  .  P  .  .  .  . │
// 3 │ .  .  .  .  .  .  .  . │
// 2 │ P  .  P  .  .  P  P  P │
// 1 │ R  N  B  Q  K  B  N  R │
//   └────────────────────────┘
//     a  b  c  d  e  f  g  h
```

Alternative moves and comments are (temporarily) skipped.

In further versions: navigation between alternative branches (variations), also loading those branches from **PGN**. Also functionality to add comments to each move.

## End Game

Declare **Draw/Resign**:

```csharp
board.Draw();
board.EndGame // => { EndgameType = DrawDeclared, WonSide = null }

board.Clear(); // Reset board

board.Resign(PieceColor.Black);
board.EndGame // => { EndgameType = Resigned, WonSide = White }
```

As mentioned before, **[InsufficientMaterial](https://www.chessprogramming.org/Material#InsufficientMaterial)**, **[FiftyMoveRule](https://www.chessprogramming.org/Fifty-move_Rule)** and **[Repetition](https://www.chessprogramming.org/Repetitions)** rules are available.

## Unit Tests
[Here](https://github.com/Geras1mleo/Chess/tree/master/Chess.Tests) you can see all the tests that have been used to test and improve chess library. Also useful for code examples.

## Benchmarks
[Here](https://github.com/Geras1mleo/Chess/blob/master/ChessBenchmarks/Benchmarks.cs) you can see the evolution of performance of chess library

## Like the project?

Give it a :star: Star!

## Found a bug?

Drop to [Issues](https://github.com/Geras1mleo/Chess/issues)

Or: sviatoslav.harasymchuk@gmail.com

Thanks in advance!
