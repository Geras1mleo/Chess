// *****************************************************
// *                                                   *
// * O Lord, Thank you for your goodness in our lives. *
// *     Please bless this code to our compilers.      *
// *                     Amen.                         *
// *                                                   *
// *****************************************************
//                                    Made by Geras1mleo

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Chess;

public enum EndgameType : byte
{
    Checkmate,
    Resigned,
    Timeout,
    Stalemate,
    DrawDeclared,
    InsufficientMaterial,
    FiftyMoveRule,
    Repetition,
}

[Flags]
public enum AutoEndgameRules : byte
{
    None = 0,
    InsufficientMaterial = 1,
    Repetition = 2,
    FiftyMoveRule = 4,
    All = 7
}

public enum CastleType : byte
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