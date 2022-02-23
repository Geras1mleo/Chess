// *****************************************************
// *                                                   *
// * O Lord, Thank you for your goodness in our lives. *
// *     Please bless this code to our compilers.      *
// *                     Amen.                         *
// *                                                   *
// *****************************************************
//                                    Made by Geras1mleo

namespace Chess;

internal class Regexes
{
    internal const string SanOneMovePattern = @"(^([PNBRQK])?([a-h])?([1-8])?(x|X|-)?([a-h][1-8])(=[NBRQ]| ?e\.p\.)?|^O-O(-O)?)(\+|\#|\$)?$";

    internal const string SanSanMovesPattern = @"([PNBRQK]?[a-h]?[1-8]?[xX-]?[a-h][1-8](=[NBRQ]| ?e\.p\.)?|O-O(?:-O)?)[+#$]?";

    internal const string HeadersPattern = @"\[(.*?) ""(.*?)""\]";

    internal const string AlternativesPattern = @"\(.*?\)";

    internal const string CommentsPattern = @"\{.*?\}";

    internal const string FenPattern = @"^(((?:[rnbqkpRNBQKP1-8]+\/){7})[rnbqkpRNBQKP1-8]+) ([b|w]) (-|[K|Q|k|q]{1,4}) (-|[a-h][36]) (\d+ \d+)$";

    internal const string PiecePattern = "^[wb][bknpqr]$";

    internal const string FenPiecePattern = "^([bknpqr]|[BKNPQR])$";

    internal const string PositionPattern = "^[a-h][1-8]$";

    internal const string MovePattern = @"^{(([wb][bknpqr]) - )?([a-h][1-8]) - ([a-h][1-8])( - ([wb][bknpqr]))?( - (o-o|o-o-o|e\.p\.|=|=q|=r|=b|=n))?( - ([+#$]))?}$";

    internal readonly static Regex regexSanOneMove = new(SanOneMovePattern, RegexOptions.Compiled, TimeSpan.FromMilliseconds(250));

    internal readonly static Regex regexSanMoves = new(SanSanMovesPattern, RegexOptions.Compiled, TimeSpan.FromMilliseconds(250));

    internal readonly static Regex regexHeaders = new(HeadersPattern, RegexOptions.Compiled, TimeSpan.FromMilliseconds(250));

    internal readonly static Regex regexAlternatives = new(AlternativesPattern, RegexOptions.Compiled | RegexOptions.Singleline, TimeSpan.FromMilliseconds(250));

    internal readonly static Regex regexComments = new(CommentsPattern, RegexOptions.Compiled | RegexOptions.Singleline, TimeSpan.FromMilliseconds(250));

    internal readonly static Regex regexFen = new(FenPattern, RegexOptions.Compiled, TimeSpan.FromMilliseconds(250));

    internal readonly static Regex regexPiece = new(PiecePattern, RegexOptions.Compiled, TimeSpan.FromMilliseconds(250));

    internal readonly static Regex regexFenPiece = new(FenPiecePattern, RegexOptions.Compiled, TimeSpan.FromMilliseconds(250));

    internal readonly static Regex regexPosition = new(PositionPattern, RegexOptions.Compiled, TimeSpan.FromMilliseconds(250));

    internal readonly static Regex regexMove = new(MovePattern, RegexOptions.Compiled, TimeSpan.FromMilliseconds(250));
}
