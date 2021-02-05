namespace Chess.ChessBackEnd
{
    class Board
    {
        public Figure[,] Figures { get; set; }

        public Board()
        {
            Figures = new Figure[8, 8];
            
            Figures[0, 0] = new Figure(FigureColor.White, FigureType.Rook);
            Figures[0, 1] = new Figure(FigureColor.White, FigureType.Knight);
            Figures[0, 2] = new Figure(FigureColor.White, FigureType.Bishop);
            Figures[0, 3] = new Figure(FigureColor.White, FigureType.Queen);
            Figures[0, 4] = new Figure(FigureColor.White, FigureType.King);
            Figures[0, 5] = new Figure(FigureColor.White, FigureType.Bishop);
            Figures[0, 6] = new Figure(FigureColor.White, FigureType.Knight);
            Figures[0, 7] = new Figure(FigureColor.White, FigureType.Rook);

            Figures[1, 0] = new Figure(FigureColor.White, FigureType.Pawn);
            Figures[1, 1] = new Figure(FigureColor.White, FigureType.Pawn);
            Figures[1, 2] = new Figure(FigureColor.White, FigureType.Pawn);
            Figures[1, 3] = new Figure(FigureColor.White, FigureType.Pawn);
            Figures[1, 4] = new Figure(FigureColor.White, FigureType.Pawn);
            Figures[1, 5] = new Figure(FigureColor.White, FigureType.Pawn);
            Figures[1, 6] = new Figure(FigureColor.White, FigureType.Pawn);
            Figures[1, 7] = new Figure(FigureColor.White, FigureType.Pawn);

            Figures[6, 0] = new Figure(FigureColor.Black, FigureType.Pawn);
            Figures[6, 1] = new Figure(FigureColor.Black, FigureType.Pawn);
            Figures[6, 2] = new Figure(FigureColor.Black, FigureType.Pawn);
            Figures[6, 3] = new Figure(FigureColor.Black, FigureType.Pawn);
            Figures[6, 4] = new Figure(FigureColor.Black, FigureType.Pawn);
            Figures[6, 5] = new Figure(FigureColor.Black, FigureType.Pawn);
            Figures[6, 6] = new Figure(FigureColor.Black, FigureType.Pawn);
            Figures[6, 7] = new Figure(FigureColor.Black, FigureType.Pawn);

            Figures[7, 0] = new Figure(FigureColor.Black, FigureType.Rook);
            Figures[7, 1] = new Figure(FigureColor.Black, FigureType.Knight);
            Figures[7, 2] = new Figure(FigureColor.Black, FigureType.Bishop);
            Figures[7, 3] = new Figure(FigureColor.Black, FigureType.Queen);
            Figures[7, 4] = new Figure(FigureColor.Black, FigureType.King);
            Figures[7, 5] = new Figure(FigureColor.Black, FigureType.Bishop);
            Figures[7, 6] = new Figure(FigureColor.Black, FigureType.Knight);
            Figures[7, 7] = new Figure(FigureColor.Black, FigureType.Rook);
        }

    }
}
