namespace Chess.ChessBackEnd
{
    public partial class Board
    {
        private short[] enPassantPos = { 0,0};
        public void SetEnPassantPos(string newPos)
        {
            var pos = newPos.Split(',');
            enPassantPos = new short[] { short.Parse(pos[0]), short.Parse(pos[1]) };
        }

        // These variables are needed to track if castle is still possible
        private bool wKingMoved,
                       wLeftRookMoved, wRightRookMoved,
                       bKingMoved,
                       bLeftRookMoved, bRightRookMoved;

        public void SetCastlePermitations(FigureType type, short[] oldPos)
        {
            switch (type)
            {
                case FigureType.Rook:

                    if (oldPos[0] == 0 && oldPos[1] == 0)
                        wLeftRookMoved = true;
                    else if (oldPos[0] == 0 && oldPos[1] == 7)
                        wRightRookMoved = true;
                     else if (oldPos[0] == 7 && oldPos[1] == 0)
                        bLeftRookMoved = true;
                    else if (oldPos[0] == 7 && oldPos[1] == 7)
                        bRightRookMoved = true;
                    break;
                case FigureType.King:
                    if (oldPos[0] == 0)
                        wKingMoved = true;
                    else if(oldPos[0] == 7) 
                        bKingMoved = true;
                    break;
            }
        }

        /// <summary>
        /// Returns bool => valid move or not
        /// </summary>
        /// <param name="figure">Draged Figure</param>
        /// <param name="oldPos">First value is vertical, second => horizontal</param>
        /// <param name="newPos">First value is vertical, second => horizontal</param>
        /// <returns></returns>
        public bool IsValidOperation(Figure figure, short[] oldPos, short[] newPos)
        {
            bool isValid = false;
            Parameters = "";

            if (CustomChessTable.PlayerMove != figure.Color) return false;

            switch (figure.Type)
            {
                case FigureType.Pawn:
                    isValid = PawnValidation(figure, oldPos, newPos);
                    break;
                case FigureType.Rook:
                    isValid = RookValidation(figure, oldPos, newPos);
                    break;
                case FigureType.Knight:
                    isValid = KnightValidation(figure, oldPos, newPos);
                    break;
                case FigureType.Bishop:
                    isValid = BishopValidation(figure, oldPos, newPos);
                    break;
                case FigureType.Queen:
                    // For queen just using validation of bishop OR rook
                    isValid = BishopValidation(figure, oldPos, newPos) || RookValidation(figure, oldPos, newPos);
                    break;
                case FigureType.King:
                    isValid = KingValidation(figure, oldPos, newPos);
                    break;
            }

            return isValid && !IsKingAttacked(figure, oldPos, newPos);
        }

        /// <summary>
        /// Basically checking if the next operation on position of king is valid for one of figures
        /// </summary>
        /// <param name="figure"></param>
        /// <param name="oldPos"></param>
        /// <param name="newPos"></param>
        /// <returns></returns>
        private bool IsKingAttacked(Figure figure, short[] oldPos, short[] newPos)
        {
            // Remember begin situation of figures and parameters
            var beginSituation = (Figure[,])Figures.Clone();
            var beginParameters = Parameters;

            if(!(oldPos[0] == newPos[0] && oldPos[1] == newPos[1]))
            {
                Figures[newPos[0], newPos[1]] = Figures[oldPos[0], oldPos[1]];
                Figures[oldPos[0], oldPos[1]] = null;
            }

            // Here comes each figure that must be checked and current position of it
            object[,] figures = new object[16, 2];
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (Figures[i,j] != null && Figures[i,j].Color != figure.Color)
                    {
                        for (int k = 0; k < figures.GetLength(0); k++)
                        {
                            if (figures[k, 0] == null)
                            {
                                figures[k, 0] = Figures[i, j];
                                figures[k, 1] = new short[] { (short)i, (short)j };
                                break;
                            }
                        }
                    }
                }
            }

            // Searching for king position
            var kingPos = new short[2];
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if(Figures[i,j] != null && Figures[i,j].Color == figure.Color && Figures[i,j].Type == FigureType.King)
                    {
                        kingPos[0] = (short)i;
                        kingPos[1] = (short)j;
                        goto FoundKing;
                    }
                }
            }
            // Going out from 2 loops
            FoundKing:

            bool isAttacked = false;

            for (int i = 0; i < figures.GetLength(0); i++)
            {
                var fig = figures[i, 0] as Figure;
                if (fig == null) break;

                switch (fig.Type)
                {
                    case FigureType.Pawn:
                        isAttacked = PawnValidation(fig, (short[])figures[i, 1], kingPos);
                        break;
                    case FigureType.Rook:
                        isAttacked = RookValidation(fig, (short[])figures[i, 1], kingPos);
                        break;
                    case FigureType.Knight:
                        isAttacked = KnightValidation(fig, (short[])figures[i, 1], kingPos);
                        break;
                    case FigureType.Bishop:
                        isAttacked = BishopValidation(fig, (short[])figures[i, 1], kingPos);
                        break;
                    case FigureType.Queen:
                        // For queen just using validation of bishop OR rook
                        isAttacked = BishopValidation(fig, (short[])figures[i, 1], kingPos) || RookValidation(fig, (short[])figures[i, 1], kingPos);
                        break;
                    case FigureType.King:
                        isAttacked = KingValidation(fig, (short[])figures[i, 1], kingPos);
                        break;
                }

                if (isAttacked) break;
            }

            Figures = beginSituation;
            Parameters = beginParameters;

            return isAttacked;
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
                    Parameters = "Queen";
                

                // If there enpassant is possible => remember position
                if (oldPos[0] + (bw * 2) == newPos[0] && (newPos[1] + 1 < 8 && Figures[newPos[0], newPos[1] + 1] != null && Figures[newPos[0], newPos[1] + 1].Type == FigureType.Pawn && Figures[newPos[0], newPos[1] + 1].Color != figure.Color || newPos[1] - 1 > -1 && Figures[newPos[0], newPos[1] - 1] != null && Figures[newPos[0], newPos[1] - 1].Type == FigureType.Pawn && Figures[newPos[0], newPos[1] - 1].Color != figure.Color))
                {
                    enPassantPos = newPos;
                    Parameters = "EnpasPos";
                }

                return true;
            }
            // If En Passant => give it in parameters to replace later
            else if (enPassantPos[0] == oldPos[0] && (enPassantPos[1] == oldPos[1] + 1 || enPassantPos[1] == oldPos[1] - 1) &&
                ((oldPos[0] == 3 && Figures[3, enPassantPos[1]] != null && Figures[3, enPassantPos[1]].Color == FigureColor.White && Figures[3, oldPos[1]].Color == FigureColor.Black) ||
                (oldPos[0] == 4 && Figures[4, enPassantPos[1]] != null && Figures[4, enPassantPos[1]].Color == FigureColor.Black && Figures[4, oldPos[1]].Color == FigureColor.White)) &&
                newPos[0] == enPassantPos[0] + bw && newPos[1] == enPassantPos[1])
            {
                Parameters = "EnpasMove:" + enPassantPos[0] + "," + enPassantPos[1];
                return true;
            }
            else return false;
        }

        private bool RookValidation(Figure figure, short[] oldPos, short[] newPos)
        {
            // If moving vertical...
            if (oldPos[1] == newPos[1])
            {
                // Here searching for first figure on da way
                for (int i = oldPos[0]; (oldPos[0] < newPos[0] && i <=newPos[0]) || (oldPos[0] > newPos[0] && i >= newPos[0]); i += (oldPos[0] < newPos[0]? 1 : -1))
                {
                    // First iteration skip bc this is old position of rook 
                    if (i == oldPos[0]) continue;

                    // Here comes first figure on the way
                    var firstFigure = Figures[i, oldPos[1]];

                    if (firstFigure != null)
                    {
                        if (firstFigure.Color == figure.Color || newPos[0] != i)
                            return false;
                        // If taking this first figure return true and pass en parameters that castle is not more possible
                        else
                        {
                            if((figure.Color == FigureColor.White && (!wLeftRookMoved || !wRightRookMoved)) || (figure.Color == FigureColor.Black && (!bLeftRookMoved || !bRightRookMoved)))
                                Parameters = "CastlePerm:Rook";
                            
                            return true;
                        }
                    }
                }
                if ((figure.Color == FigureColor.White && (!wLeftRookMoved || !wRightRookMoved)) || (figure.Color == FigureColor.Black && (!bLeftRookMoved || !bRightRookMoved)))
                    Parameters = "CastlePerm:Rook";
                return true;
            }
            // If moving horizontal
            else if (oldPos[0] == newPos[0])
            {
                // Here searching for first figure on da way
                for (int i = oldPos[1]; (oldPos[1] < newPos[1] && i <= newPos[1]) || (oldPos[1] > newPos[1] && i >= newPos[1]) ; i += (oldPos[1] < newPos[1]? 1 : -1))
                {
                    // First iteration skip bc this is old position of rook 
                    if (i == oldPos[1]) continue;

                    // Here comes first figure on the way
                    var firstFigure = Figures[oldPos[0], i];

                    if(firstFigure != null)
                    {
                        if (firstFigure.Color == figure.Color || newPos[1] != i)
                            return false;

                        else
                        {
                            if ((figure.Color == FigureColor.White && (!wLeftRookMoved || !wRightRookMoved)) || (figure.Color == FigureColor.Black && (!bLeftRookMoved || !bRightRookMoved)))
                                Parameters = "CastlePerm:Rook";

                            return true;
                        }
                    }
                }
                if ((figure.Color == FigureColor.White && (!wLeftRookMoved || !wRightRookMoved)) || (figure.Color == FigureColor.Black && (!bLeftRookMoved || !bRightRookMoved)))
                    Parameters = "CastlePerm:Rook";

                return true;
            }
            else return false;
        }

        private bool KnightValidation(Figure figure, short[] oldPos, short[] newPos)
        {
            if(// Moving forewards
               (newPos[0] - oldPos[0] == 2 && newPos[1] - oldPos[1] == 1) ||
               (newPos[0] - oldPos[0] == 1 && newPos[1] - oldPos[1] == 2) ||
               (newPos[0] - oldPos[0] == 2 && newPos[1] - oldPos[1] == -1) ||
               (newPos[0] - oldPos[0] == 1 && newPos[1] - oldPos[1] == -2) ||
               // Moving backwards
               (newPos[0] - oldPos[0] == -2 && newPos[1] - oldPos[1] == 1) ||
               (newPos[0] - oldPos[0] == -1 && newPos[1] - oldPos[1] == 2) ||
               (newPos[0] - oldPos[0] == -2 && newPos[1] - oldPos[1] == -1) ||
               (newPos[0] - oldPos[0] == -1 && newPos[1] - oldPos[1] == -2))
            {
                if (Figures[newPos[0], newPos[1]] != null && Figures[newPos[0], newPos[1]].Color == figure.Color) return false;
                else return true;
            }
            return false;
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
            i += (oldPos[0] < newPos[0]? 1 : -1), j += (oldPos[1] < newPos[1] ? 1 : -1))
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

        private bool KingValidation(Figure figure, short[] oldPos, short[] newPos)
        {
            if((newPos[0] - oldPos[0] == 0 || newPos[0] - oldPos[0] == 1 || newPos[0] - oldPos[0] == -1) && (newPos[1] - oldPos[1] == 0 || newPos[1] - oldPos[1] == 1 || newPos[1] - oldPos[1] == -1))
            {
                if ((!bKingMoved && figure.Color == FigureColor.Black) || (!wKingMoved && figure.Color == FigureColor.White))
                    Parameters = "CastlePerm:King";

                return Figures[newPos[0], newPos[1]]?.Color != figure.Color;
            }
            // Castle
            else if ((newPos[1] == 0 || newPos[1] == 7) && Figures[newPos[0], newPos[1]]?.Type == FigureType.Rook && Figures[newPos[0], newPos[1]]?.Color == figure.Color)
            {
                switch (figure.Color)
                {
                    case FigureColor.White:
                        if (!wKingMoved && (!wLeftRookMoved || !wRightRookMoved))
                        {
                            // Left Castle
                            if (newPos[1] == 0 && !wLeftRookMoved)
                            {
                                if(Figures[0,1] is null && Figures[0,2] is null && Figures[0,3] is null)
                                {
                                    for (short i = 4; i >= 1; i--)
                                    {
                                        if (IsKingAttacked(Figures[0, 4], new short[] { 0, 4 }, new short[] { 0, i }))
                                            return false;   
                                    }
                                    Parameters = "Castle:2";
                                    return true;
                                }
                            }
                            // Right Castle
                            else if (newPos[1] == 7 && !wRightRookMoved)
                            {
                                if (Figures[0, 5] is null && Figures[0, 6] is null)
                                {
                                    for (short i = 4; i <= 6; i++)
                                    {
                                        if (IsKingAttacked(Figures[0, 4], new short[] { 0, 4 }, new short[] { 0, i }))
                                            return false;
                                    }
                                    Parameters = "Castle:6";
                                    return true;
                                }
                            }
                        }
                        break;
                    case FigureColor.Black:
                        if (!bKingMoved && (!bLeftRookMoved || !bRightRookMoved))
                        {
                            // Left Castle
                            if (newPos[1] == 0 && !bLeftRookMoved)
                            {
                                if (Figures[7, 1] is null && Figures[7, 2] is null && Figures[7, 3] is null)
                                {
                                    for (short i = 4; i >= 1; i--)
                                    {
                                        if (IsKingAttacked(Figures[7, 4], new short[] { 7, 4 }, new short[] { 7, i }))
                                            return false;
                                    }
                                    Parameters = "Castle:2";
                                    return true;
                                }
                            }
                            // Right Castle
                            else if (newPos[1] == 7 && !bRightRookMoved)
                            {
                                if (Figures[7, 5] is null && Figures[7, 6] is null)
                                {
                                    for (short i = 4; i <= 6; i++)
                                    {
                                        if (IsKingAttacked(Figures[7, 4], new short[] { 7, 4 }, new short[] { 7, i }))
                                            return false;
                                    }
                                    Parameters = "Castle:6";
                                    return true;
                                }
                            }
                        }
                        break;
                }
            }
            return false;
        }
    }
}
