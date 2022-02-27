<p align="center">
  <img width="128" align="center" src="https://user-images.githubusercontent.com/67554762/152679427-e59a58a8-0a03-449b-9f77-3bb52aed982c.png">
</p>
<h1 align="center">Gera Chess Library</h1>
<div>
	<p align="center">
	  Chess logic made with C# and &hearts; by Geras1mleo
	</p>
</div>


## Chess lib includes:

- Chess board with 2-dimentional array of `Pieces`
- Generation, Validation and Execution of `Moves` on Chess board
- Logic to `Convert`:
  - `Move` into `SAN` and back into Move object
  - `Chess Board`into `FEN` and back into ChessBoard object
  - `Chess Game` into `PGN` and back into ChessBoard object
- Event Handlers:
  -  `OnInvalidMoveKingChecked` - Raises when trying to execute move that places own king in check
  -  `OnWhiteKingCheckedChanged`/`OnBlackKingCheckedChanged` with state (checked or not) and its position
  -  `OnPromotePawn` with PromotionEventArgs.PromotionResult (default: PromotionToQueen)
  -  `OnEndGame` with according end game info (`Checkmate`/`Stalemate`/`Resigned`/`Draw`)
  -  `OnCaptured` with captured piece and all recently captured pieces (White/Black)
- `End Game` Declaration: `Draw` or `Resign` of one of sides
- `Navigation` between executed moves:
  - `First`/`Last`
  - `Next`/`Previous`
  - Also: `board.MoveIndex` property to navigate direct to specific move
- `Cancelation` of last executed move

# Usage!

Example simple console chess game:

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
//  8│ r  n  b  q  k  b  n  r │
//  7│ p  p  p  p  p  .  .  p │
//  6│ .  .  .  .  .  .  .  . │
//  5│ .  .  .  .  .  p  p  Q │
//  4│ .  .  .  .  P  P  .  . │
//  3│ .  .  .  .  .  .  .  . │
//  2│ P  P  P  P  .  .  P  P │
//  1│ R  N  B  .  K  B  N  R │
//   └────────────────────────┘
//     a  b  c  d  e  f  g  h
//
// 1. e4 f5 2. f4 g5 3. Qh5# 1-0
```

Example random chess game:

```csharp
while (!board.IsEndGame)
{
    var moves = board.Moves();
    board.Move(moves[Random.Shared.Next(moves.Length)]);
}

Console.WriteLine(board.ToAscii());
Console.WriteLine(board.ToPgn());

// Todo: End game Insufficient Material
```


## Tracking Pieces

Track pieces using indexers:

```csharp
board["c2"] 		// White Pawn
board['g', 8] 		// Black Bishop
// Counting from 0
board[0, 0] 		// White Rook
board[new Position(4, 7)] // Black King
```
