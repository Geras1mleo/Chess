namespace Chess.ChessBackEnd
{
    public enum FigureColor : short
    {
        White,
        Black
    }
    public enum FigureType : short
    {
        Pawn,
        Rook,
        Knight,
        Bishop,
        Queen,
        King
    }
    public enum TableColor : short
    {
        Green,
        Blue
    }
    public enum Sound : short
    {
        MoveSelf,
        MoveOpponent,
        MoveCheck,
        Castle,
        Capture,
        Promote,
        GameStart,
        GameEnd,
        IllegalMove
    }
}
