// *****************************************************
// *                                                   *
// * O Lord, Thank you for your goodness in our lives. *
// *     Please bless this code to our compilers.      *
// *                     Amen.                         *
// *                                                   *
// *****************************************************
//                                    Made by Geras1mleo

namespace Chess;

public partial class ChessBoard
{
    /// <summary>
    /// Tries to
    /// parse San-notated move into Move object<br/>
    /// Long algebraic notation is also acceptable
    /// </summary>
    /// <param name="san">San-notated move to parse</param>
    /// <param name="move">Result with parsed Move object</param>
    /// <param name="resetSan">Whether SAN needs to be regenerated</param>
    /// <returns>Whether convert succeeded</returns>
    public bool TryParseFromSan(string san, [NotNullWhen(true)] out Move? move, bool resetSan = false)
    {
        return SanBuilder.TryParse(this, san, out move, resetSan).succeeded;
    }

    /// <summary>
    /// Parses San-notated move into Move object<br/>
    /// Long algebraic notation is also acceptable
    /// </summary>
    /// <param name="san">San-notated move to parse</param>
    /// <param name="resetSan">Whether SAN needs to be regenerated</param>
    /// <returns>Parsed Move object</returns>
    /// <exception cref="ChessArgumentException">Given San-notated move didn't match the Regex pattern</exception>
    /// <exception cref="ChessSanNotFoundException">Given San-notated move is not valid for current board positions</exception>
    /// <exception cref="ChessSanTooAmbiguousException">Given San-notated move is too ambiguous between multiple moves</exception>
    public Move ParseFromSan(string san, bool resetSan = false)
    {
        var (succeeded, exception) = SanBuilder.TryParse(this, san, out var move, resetSan);

        if (!succeeded && exception is not null)
            throw exception;

        return move!;
    }

    /// <summary>
    /// Tries to
    /// parse Move object into Standard Algebraic Notation move-string
    /// </summary>
    /// <param name="move">Move to parse</param>
    /// <param name="san">Result with parsed SAN-string</param>
    /// <returns>Whether convertion succeeded</returns>
    public bool TryParseToSan(Move move, [NotNullWhen(true)] out string? san)
    {
        return SanBuilder.TryParse(this, move, out san).succeeded;
    }

    /// <summary>
    /// Parses Move object into Standard Algebraic Notation move-string
    /// </summary>
    /// <param name="move">Move to parse</param>
    /// <returns>Parsed Move in SAN</returns>
    /// <exception cref="ArgumentNullException">Given move was null or didn't have valid positions values</exception>
    public string ParseToSan(Move move)
    {
        var (succeeded, exception) = SanBuilder.TryParse(this, move, out var san);

        if (!succeeded && exception is not null)
            throw exception;

        return san!;
    }
}
