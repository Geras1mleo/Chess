<p align="center">
  <img width="128" align="center" src="/Images/knight.png">
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
  - `Move` object into `SAN` and back into Move object
  - `Board` object into `FEN` and back into Board object
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
