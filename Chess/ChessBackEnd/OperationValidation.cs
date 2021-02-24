using System;

namespace Chess.ChessBackEnd
{
    public partial class Board
    {
        private short[] enPassantPos = { 0,0};

        /// <summary>
        /// 
        /// </summary>
        /// <param name="figure">Draged Figure</param>
        /// <param name="oldPos">First value is vertical, second => horizontal</param>
        /// <param name="newPos">First value is vertical, second => horizontal</param>
        /// <returns></returns>
        public bool IsValidOperation(Figure figure, short[] oldPos, short[] newPos)
        {
            bool isValid = false;

            if (!ValidPlayerMove(figure)) return false;

            switch (figure.Type)
            {
                case FigureType.Pawn:
                    isValid = PawnValidation(figure, oldPos, newPos);
                    break;
                case FigureType.Rook:
                    isValid = RookValidation(figure, oldPos, newPos);
                    break;
                case FigureType.Knight:
                    // TODO
                    break;
                case FigureType.Bishop:
                    isValid = BishopValidation(figure, oldPos, newPos);
                    break;
                case FigureType.Queen:
                    // For queen just using validation of bishop or rook
                    isValid = BishopValidation(figure, oldPos, newPos) || RookValidation(figure, oldPos, newPos);
                    break;
                case FigureType.King:
                    //TODO
                    break;
            }
            if (isValid)
            {
                CustomChessTable.PlayerMove = figure.Color == FigureColor.White ? FigureColor.Black : FigureColor.White;
                return true;
            }
            else return false;
        }

        public bool IsKingAttacked(TableButton button)
        {
            //TODO
            return false;
        }

        private bool ValidPlayerMove(Figure figure)
        {
            if (CustomChessTable.PlayerMove == figure.Color)
                return true;
            else return false;
        }

        private bool PawnValidation(Figure figure, short[] oldPos, short[] newPos)
        {
            // If pawn is white we will have to + 1 because it can move only forewards, if black - 1
            short bw = figure.Color == FigureColor.White ? (short)1 : (short)-1;

            // First condition is 1 step foreward or 2 steps if on beginning
            if ((oldPos[1] == newPos[1] && (oldPos[0] + bw == newPos[0] || oldPos[0] + (bw * 2) == newPos[0] && (oldPos[0] == 1 || oldPos[0] == 6)) &&
                Figures[oldPos[0] + bw, oldPos[1]] == null)
                ||// Second condition horizontal taking figure
                (oldPos[0] + bw == newPos[0] &&
                ((oldPos[1] + 1 < 8 && oldPos[1] + 1 == newPos[1] && Figures[oldPos[0] + bw, oldPos[1] + 1] != null) ||
                (oldPos[1] - 1 > -1 && oldPos[1] - 1 == newPos[1] && Figures[oldPos[0] + bw, oldPos[1] - 1] != null)) &&
                (Figures[oldPos[0], oldPos[1]].Color != Figures[newPos[0], newPos[1]].Color)))
            {
                // If pawn reached end it turns to Queen
                if (newPos[0] == 7 || newPos[0] == 0)
                    Figures[oldPos[0], oldPos[1]].Type = FigureType.Queen;

                // If there enpassant is possible => remember position
                if (oldPos[0] + (bw * 2) == newPos[0])
                    enPassantPos = newPos;

                return true;
            }
            // If En Passant => clear button and figure
            else if (enPassantPos[0] == oldPos[0] && (enPassantPos[1] == oldPos[1] + 1 || enPassantPos[1] == oldPos[1] - 1) &&
                ((oldPos[0] == 3 && Figures[3, enPassantPos[1]] != null && Figures[3, enPassantPos[1]].Color == FigureColor.White && Figures[3, oldPos[1]].Color == FigureColor.Black) ||
                (oldPos[0] == 4 && Figures[4, enPassantPos[1]] != null && Figures[4, enPassantPos[1]].Color == FigureColor.Black && Figures[4, oldPos[1]].Color == FigureColor.White)) &&
                newPos[0] == enPassantPos[0] + bw && newPos[1] == enPassantPos[1])
            {
                Figures[enPassantPos[0], enPassantPos[1]] = null;
                buttons[enPassantPos[0], enPassantPos[1]].Image = null;
                return true;
            }
            else return false;
        }

        private bool RookValidation(Figure figure, short[] oldPos, short[] newPos)
        {
            // If moving vertical...
            if (oldPos[1] == newPos[1])
            {
                for (int i = oldPos[0]; (oldPos[0] < newPos[0] && i <=newPos[0]) || (oldPos[0] > newPos[0] && i >= newPos[0]); i = i + (oldPos[0] < newPos[0]? 1 : -1))
                {
                    // First iteration skip bc this is old position of rook 
                    if (i == oldPos[0]) continue;

                    // Here comes first figure on the way
                    var firstFigure = Figures[i, oldPos[1]];

                    if (firstFigure != null)
                    {
                        if (firstFigure.Color == figure.Color || newPos[0] != i)
                            return false;

                        else return true;
                    }
                }
                return true;
            }
            // If moving horizontal
            else if (oldPos[0] == newPos[0])
            {
                for (int i = oldPos[1]; (oldPos[1] < newPos[1] && i <= newPos[1]) || (oldPos[1] > newPos[1] && i >= newPos[1]) ; i = i + (oldPos[1] < newPos[1]? 1 : -1))
                {
                    // First iteration skip bc this is old position of rook 
                    if (i == oldPos[1]) continue;

                    // Here comes first figure on the way
                    var firstFigure = Figures[oldPos[0], i];

                    if(firstFigure != null)
                    {
                        if (firstFigure.Color == figure.Color || newPos[1] != i)
                            return false;

                        else return true;
                    }
                }
                return true;
            }
            else return false;
        }

        private bool BishopValidation(Figure figure, short[] oldPos, short[] newPos)
        {
            // If new position is on same vertical or horizontal just go back bc its impossible for bishop
            if (oldPos[0] == newPos[0] || oldPos[1] == newPos[1]) return false;

            for (int i = oldPos[0], j = oldPos[1]; 
            (oldPos[0] < newPos[0] && oldPos[1] < newPos[1] && i <= newPos[0] && j <= newPos[1]) || 
            (oldPos[0] > newPos[0] && oldPos[1] > newPos[1] && i >= newPos[0] && j >= newPos[1]) ||
            (oldPos[0] < newPos[0] && oldPos[1] > newPos[1] && i <= newPos[0] && j >= newPos[1]) ||
            (oldPos[0] > newPos[0] && oldPos[1] < newPos[1] && i >= newPos[0] && j <= newPos[1]) ; 
            i = i + (oldPos[0] < newPos[0]? 1 : -1), j = j + (oldPos[1] < newPos[1] ? 1 : -1))
            {
                // First iteration skip bc this is old position of bishop 
                if (i == oldPos[0] && j == oldPos[1]) continue;

                // Here comes first figure on the way
                var firstFigure = Figures[i, j];

                // Checking if its not empty place and if player set bishop to this position and if not than it means that this figure stays on the way of bishop
                if (firstFigure != null && !(i == newPos[0] && j == newPos[1])) return false;

                if (i == newPos[0] && j == newPos[1])
                {
                    if (firstFigure != null)
                    {
                        if (firstFigure.Color == figure.Color)
                            return false;

                        else return true;
                    }
                    else return true;
                }
            }
            return false;
        }
    }
}
