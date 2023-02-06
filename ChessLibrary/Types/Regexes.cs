// *****************************************************
// *                                                   *
// * O Lord, Thank you for your goodness in our lives. *
// *     Please bless this code to our compilers.      *
// *                     Amen.                         *
// *                                                   *
// *****************************************************
//                                    Made by Geras1mleo

namespace Chess;

internal static class Regexes
{
    internal const string SanOneMovePattern = @"(^([PNBRQK])?([a-h])?([1-8])?(x|X|-)?([a-h][1-8])(=[NBRQ]| ?e\.p\.)?|^O-O(-O)?)(\+|\#|\$)?$";

    internal const string SanMovesPattern = @"(?:[PNBRQK]?[a-h]?[1-8]?[xX-]?[a-h][1-8](?:=[NBRQ]| ?e\.p\.)?|O-O(?:-O)?)[+#$]?";

    internal const string HeadersPattern = @"\[([^ ]+) ""([^""]*)""\]";

    internal const string AlternativesPattern = @"\([^)]*\)";

    internal const string CommentsPattern = @"\{[^}]*\}";

    internal const string FenPattern = @"^(((?:[rnbqkpRNBQKP1-8]+\/){7})[rnbqkpRNBQKP1-8]+) ([bw]) (-|[KQkq]{1,4}) (-|[a-h][36]) (\d+ \d+)$";

    internal const string FenContainsOneWhiteKingPattern = "^[^ K]*K[^ K]* ";

    internal const string FenContainsOneBlackKingPattern = "^[^ k]*k[^ k]* ";

    internal const string PiecePattern = "^[wb][bknpqr]$";

    internal const string FenPiecePattern = "^[bknpqrBKNPQR]$";

    internal const string PositionPattern = "^[a-h][1-8]$";

    internal const string MovePattern = @"^{(([wb][bknpqr]) - )?([a-h][1-8]) - ([a-h][1-8])( - ([wb][bknpqr]))?( - (o-o|o-o-o|e\.p\.|=|=q|=r|=b|=n))?( - ([+#$]))?}$";

    internal static readonly Regex RegexSanOneMove = new(SanOneMovePattern, RegexOptions.Compiled, TimeSpan.FromMilliseconds(250));

    internal static readonly Regex RegexSanMoves = new(SanMovesPattern, RegexOptions.Compiled, TimeSpan.FromMilliseconds(250));

    internal static readonly Regex RegexHeaders = new(HeadersPattern, RegexOptions.Compiled, TimeSpan.FromMilliseconds(250));

    internal static readonly Regex RegexAlternatives = new(AlternativesPattern, RegexOptions.Compiled | RegexOptions.Singleline, TimeSpan.FromMilliseconds(250));

    internal static readonly Regex RegexComments = new(CommentsPattern, RegexOptions.Compiled | RegexOptions.Singleline, TimeSpan.FromMilliseconds(250));

    internal static readonly Regex RegexFen = new(FenPattern, RegexOptions.Compiled, TimeSpan.FromMilliseconds(250));

    internal static readonly Regex RegexFenContainsOneWhiteKing = new(FenContainsOneWhiteKingPattern, RegexOptions.Compiled, TimeSpan.FromMilliseconds(250));

    internal static readonly Regex RegexFenContainsOneBlackKing = new(FenContainsOneBlackKingPattern, RegexOptions.Compiled, TimeSpan.FromMilliseconds(250));

    internal static readonly Regex RegexPiece = new(PiecePattern, RegexOptions.Compiled, TimeSpan.FromMilliseconds(250));

    internal static readonly Regex RegexFenPiece = new(FenPiecePattern, RegexOptions.Compiled, TimeSpan.FromMilliseconds(250));

    internal static readonly Regex RegexPosition = new(PositionPattern, RegexOptions.Compiled, TimeSpan.FromMilliseconds(250));

    internal static readonly Regex RegexMove = new(MovePattern, RegexOptions.Compiled, TimeSpan.FromMilliseconds(250));
}
