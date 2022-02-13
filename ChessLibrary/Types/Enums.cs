#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
namespace Chess;

public enum EndgameType : byte
{
    Checkmate,
    Stalemate,
    Resigned,
    Draw,
}

internal enum CastleType : byte
{
    King,
    Queen,
}

public enum PromotionType : byte
{
    Default,
    ToQueen,
    ToRook,
    ToBishop,
    ToKnight,
}

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member