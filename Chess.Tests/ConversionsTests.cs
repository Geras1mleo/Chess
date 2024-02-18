using Chess;
using Xunit;

namespace ChessUnitTests;

public class ConversionsTests
{
    [Fact]
    public void TestPgnLoad()
    {
        var board = new ChessBoard();

        // Normal Pgn
        board = ChessBoard.LoadFromPgn(
        @"[Event ""Live Chess""]
        [Site ""Chess.com""]
        [Date ""2022.01.11""]
        [Round ""?""]
        [White ""Milan1905""]
        [Black ""Geras1mleo""]
        [Result ""1-0""]
        [ECO ""C47""]
        [WhiteElo ""1006""]
        [BlackElo ""626""]
        [TimeControl ""600""]
        [EndTime ""11:58:56 PST""]
        [Termination ""Milan1905 won by resignation""]
            
        1.e4 e5 2.Nf3 Nf6 3.Nc3 Nc6 4.Bb5 Bc5 5.Bxc6 bxc6 6.Nxe5 Bxf2+ 7.Kxf2 O-O
        8.d4 d5 9.exd5 cxd5 10.Nc6 Ng4+ 11.Kg1 Qf6 12.Qf1 Qxc6 13.h3 Nf6 14.Bg5
        Qb6 15.Bxf6 Qxf6 16.Qxf6 gxf6 17.Nxd5 Rb8 18.Nxf6+ Kh8 19.b3 Rb4 20.c3 Bb7
        21.cxb4 1-0");

        Assert.Equal("5r1k/pbp2p1p/5N2/8/1P1P4/1P5P/P5P1/R5KR b - - 0 21", board.ToFen());
        Assert.Equal(EndgameType.Resigned, board.EndGame.EndgameType);

        board = ChessBoard.LoadFromPgn(
        @"[Event ""Live Chess""]
        [Site ""Chess.com""]
        [Date ""2022.01.03""]
        [Round ""?""]
        [White ""Milan1905""]
        [Black ""Geras1mleo""]
        [Result ""1/2-1/2""]
        [ECO ""C42""]
        [WhiteElo ""1006""]
        [BlackElo ""626""]
        [TimeControl ""600""]
        [EndTime ""9:19:18 PST""]
        [Termination ""Game drawn by insufficient material""]
            
        1.e4 e5 2.Nf3 Nf6 3.Nxe5 Nxe4 4.Qe2 d5 5.d3 Bd6 6.dxe4 Bxe5 7.exd5 Qxd5
        8.c4 Qc5 9.Bf4 Nd7 10.Bxe5 Nxe5 11.Nc3 O-O 12.O-O-O Nxc4 13.Na4 Qc6 14.
        Qxc4 Bg4 15.Qxc6 bxc6 16.Rd2 Rad8 17.Rxd8 Rxd8 18.Ba6 h6 19.h3 Bf5 20.g4
        Be4 21.Re1 Rd4 22.Nc3 Bf3 23.Re8+ Kh7 24.Ne2 Bxe2 25.Bxe2 Rf4 26.Bd3+ g6
        27.Re2 f5 28.gxf5 gxf5 29.Re7+ Kg6 30.Rxc7 Rf3 31.Rxc6+ Kg5 32.h4+ Kxh4
        33.Rxh6+ Kg5 34.Rh2 Rxd3 35.Kc2 Rf3 36.Rg2+ Kh4 37.Kd2 Kh3 38.Rg8 Rxf2+
        39.Kc3 Rf3+ 40.Kb4 Rf4+ 41.Ka3 Rf3+ 42.b3 a5 43.Ka4 Rf2 44.a3 Rf4+ 45.b4
        axb4 46.axb4 Rg4 47.Rh8+ Rh4 48.Rxh4+ Kxh4 49.b5 f4 50.b6 f3 51.b7 f2 52.
        b8=Q f1=Q 53.Qb4+ Kg3 54.Qb3+ Qf3 55.Qxf3+ Kxf3 1/2-1/2");

        Assert.Equal("8/8/8/8/K7/5k2/8/8 w - - 0 56", board.ToFen());
        Assert.True(board.IsEndGame);

        // Load with comments and alternative moves
        board = ChessBoard.LoadFromPgn(
        @"1. e4 {[%timestamp 1]} 1... f5 {[%timestamp 1]} 2. exf5 {[%timestamp 7]} 2... g6
        {[%timestamp 16]} 3. fxg6 {[%timestamp 7]} 3... Nf6 {[%timestamp 26]} 4. g7
        {[%timestamp 10]} 4... Ng4 {[%timestamp 20]} (4... Ne4 5. d3 Nc3 6. f3 Nd5 7. f4
        Nf6) 5. gxh8=N {[%timestamp 14]} 1-0");

        Assert.Equal("rnbqkb1N/ppppp2p/8/8/6n1/8/PPPP1PPP/RNBQKBNR b KQq - 0 5", board.ToFen());
        Assert.Equal("1. e4 f5 2. exf5 g6 3. fxg6 Nf6 4. g7 Ng4 5. gxh8=N 1-0", board.ToPgn());

        // From Chess.com
        board = ChessBoard.LoadFromPgn(
        @"[Event ""Live Chess""]
        [Site ""Chess.com""]
        [Date ""2022.02.20""]
        [Round ""?""]
        [White ""Hikaru""]
        [Black ""tptagain""]
        [Result ""1-0""]
        [ECO ""A06""]
        [WhiteElo ""3319""]
        [BlackElo ""2909""]
        [TimeControl ""60+1""]
        [EndTime ""9:42:10 PST""]
        [Termination ""Hikaru won by resignation""]

        1. Nf3 d5 2. b3 Nf6 3. Bb2 Bf5 4. d3 e6 5. g3 h6 6. Ne5 Nbd7 7. Nxd7 Qxd7 8. Bg2
        Be7 9. Nd2 O-O 10. e4 Bh7 11. Qe2 Rfd8 12. e5 Ne8 13. O-O a5 14. a4 c5 15. Rfd1
        Nc7 16. Nf1 b5 17. Ne3 Rdb8 18. Bc1 bxa4 19. Rxa4 Rb4 20. Ra1 Nb5 21. Qe1 Nd4
        22. Bd2 Rb5 23. Bxa5 h5 24. Bc3 Rc8 25. h4 Bg6 26. Bb2 Rbb8 27. Ra2 Ra8 28. Rda1
        Rxa2 29. Rxa2 Nc6 30. Ra1 Qb7 31. Qe2 Nd4 32. Qd1 Qc7 33. f4 Qb6 34. Kh2 Rd8 35.
        Qd2 Rc8 36. Rb1 Ra8 37. Ra1 Rxa1 38. Bxa1 Qa7 39. Bb2 Qa2 40. Qc1 Qa6 41. Qa1
        Qb7 42. Bxd4 cxd4 43. Qxd4 Qc7 44. Qb2 Qb6 45. d4 Qa6 46. Bf1 Qa7 47. c3 Bd8 48.
        b4 Qa8 49. Be2 Qa4 50. Bd1 Qb5 51. Be2 Qa4 52. Kg1 f6 53. Kf2 fxe5 54. fxe5 Bf7
        55. Bd1 Qb5 56. Qa2 Qd3 57. Qa8 Qxc3 58. Qxd8+ Kh7 59. Bc2+ g6 60. Qf6 Qd2+ 61.
        Kf3 Kg8 62. Bxg6 1-0");

        Assert.Equal("6k1/5b2/4pQB1/3pP2p/1P1P3P/4NKP1/3q4/8 b - - 0 62", board.ToFen());
        Assert.True(board.IsEndGame);
        Assert.Equal(PieceColor.White, board.EndGame.WonSide);

        // With alternatives
        board = ChessBoard.LoadFromPgn(
        @"[Event ""Live Chess""]
        [Site ""Chess.com""]
        [Date ""2022.02.19""]
        [Round ""?""]
        [White ""Hikaru""]
        [Black ""Bigfish1995""]
        [Result ""1-0""]
        [ECO ""B22""]
        [WhiteElo ""2864""]
        [BlackElo ""2640""]
        [TimeControl ""600""]
        [EndTime ""12:17:19 PST""]
        [Termination ""Hikaru won on time""]

        1. e4 c5 2. Nf3 Nc6 3. c3 d5 4. exd5 Qxd5 5. Na3 Nf6 6. d4 e6 7. Nb5 Qd8 8. dxc5
        Bxc5 9. Qxd8+ Kxd8 10. Bf4 Nd5 11. O-O-O Ke7 12. Bg3 a6 13. c4 Nf6 14. Nc3 Rd8
        15. Bd3 Bd7 16. a3 a5 17. Rhe1 Nd4 18. Nxd4 Bxd4 19. Nd5+ Nxd5 20. cxd5 Bf6 21.
        Be4 Ba4 22. Rd2 Rac8+ 23. Kb1 b6 24. Bf4 Rc4 25. f3 h6 26. dxe6 Rxd2 27. Bxd2
        fxe6 28. Be3 Rc8 29. Re2 b5 30. Bb6 b4 31. Bxa5 bxa3 32. Bb4+ Kf7 33. bxa3 Bd4
        34. Bc2 Bb5 35. Rd2 e5 36. f4 Bc4 37. fxe5 Bxe5 38. h3 Rb8 39. Be4 Ke6 40. Rc2
        Bb5 41. Ka2 Ba4 42. Rd2 Bf4 43. Re2 Be5 44. Bc2 Bb5 45. Bb3+ Kf5 46. Rc2 Bd3 47.
        Rc5 Ke4 48. Bc2 Bxc2 49. Rxc2 Bd4 50. Kb3 g5 51. Rc6 h5 52. Kc4 Bg1 53. a4 Ra8
        54. a5 Kf4 55. Bd6+ Ke4 56. a6 Bf2 57. Bc5 Bxc5 58. Kxc5 Kf4 59. Kb6 Kg3 60. Rc2
        Rb8+ 61. Kc7 Rf8 62. a7 g4 63. hxg4 hxg4 64. Ra2 Kh2 65. Kb7 Rf7+ 66. Ka6 Rf8
        67. Kb5 g3 68. Kc6 Ra8 69. Kd5 Rxa7 70. Rxa7 Kxg2 71. Ke4 Kf2 72. Ra2+ Kg1 73.
        Kf3 g2 74. Rxg2+ Kh1 (74... Kf1 75. Rf2+ Ke1 76. Re2+ Kd1 77. Rd2+ Kxd2) 75. Rg3
        1-0");

        Assert.Equal("8/8/8/8/8/5KR1/8/7k b - - 2 75", board.ToFen());

        // Ultimate pgn
        board = ChessBoard.LoadFromPgn(
        @"[Event ""Live Chess""]
        [Site ""Chess.com""]
        [Date ""2022.02.02""]
        [Round ""?""]
        [White ""Hikaru""]
        [Black ""SundramNaam2SunaHoga""]
        [Result ""1-0""]
        [ECO ""B00""]
        [WhiteElo ""3204""]
        [BlackElo ""2169""]
        [TimeControl ""180""]
        [EndTime ""11:33:37 PST""]
        [Termination ""Hikaru won by checkmate""]

        1. e4 Nc6 2. Ke2 d5 3. d3 dxe4 4. dxe4 Bd7 5. c3 e5 6. Be3 f5 7. f3 Nf6 8. Nd2
        Bd6 9. Qb3 Rb8 10. Rd1 Na5 11. Qc2 fxe4 12. Nxe4 Nxe4 13. fxe4 O-O 14. Nf3 Qf6
        15. Kd2 Bg4 16. Be2 b5 17. Kc1 Nc4 18. Bxc4+ bxc4 19. Rhf1 Qg6 20. Rde1 Rb5 21.
        Qa4 Bd7 22. Qxc4+ Kh8 23. Ng5 Be8 24. Rxf8+ Bxf8 25. Rf1 Qd6 26. Nf7+ Bxf7 27.
        Qxb5 Be6 28. Qe8 Kg8 29. Rxf8+ Qxf8 30. Qxe6+ Kh8 31. Qf5 Qe8 32. g4 Qg8 33. g5
        Qxa2 34. Qf8+ Qg8 35. Qxg8+ Kxg8 36. Bxa7 Kf7 37. Bb8 Ke6 38. Bxc7 h6 39. gxh6
        gxh6 40. h3 h5 41. Kc2 Kd7 42. Bxe5 Kc6 43. Bd4 Kd7 44. Kd3 Ke6 45. b4 Kd7 46.
        c4 Kc6 47. b5+ Kb7 48. c5 Kc8 49. e5 Kb7 50. Ke4 Ka8 51. Kf5 Kb7 52. Kg5 Kc8 53.
        Kxh5 Kb7 54. Kg5 Ka8 55. Kf5 Kb7 56. e6 Kc8 57. e7 Kd7 58. Kf6 Kc8 59. h4 Kb7
        60. h5 Ka8 61. h6 Ka7 62. h7 Kb7 63. e8=N Kc8 64. Ke7 Kb7 65. c6+ Kc8 66. h8=N
        Kb8 67. Ba7+ Kc8 68. Bb8 Kxb8 69. b6 Kc8 70. Nd6+ Kb8 71. Nhf7 Ka8 72. Kd7 Kb8
        73. Nc4 Ka8 74. c7 Kb7 75. c8=N Ka6 76. N4d6 Ka5 77. b7 Kb4 78. b8=N Kc3 79. Nc6
        Kd3 80. Ng5 Ke2 81. Ne5 Kf2 82. Nf5 Ke2 83. Nb6 Kd2 84. Nd5 Kc2 85. Kc6 Kb3 86.
        Kc5 Ka4 87. Kb6 Kb3 88. Kb5 Kc2 89. Ne4 Kb2 90. Nfe3 Kc1 91. Nd3+ Kb1 92. Nd2+
        Ka1 93. Nf5 Ka2 94. Nd4 Ka3 95. Ka5 Ka2 96. Ka4 Ka1 97. Ne6 Ka2 98. Nd4 Ka1 99.
        Nc2+ Ka2 100. Nc3# 1-0");

        Assert.Equal("8/8/8/8/K7/2NN4/k1NN4/8 b - - 44 100", board.ToFen());

        // From Lichess
        // Test promotions
        board = ChessBoard.LoadFromPgn(
        @"[Event ""Casual Correspondence game""]
        [Date ""2022.02.20""]
        [White ""Anonymous""]
        [Black ""Anonymous""]
        [Result ""0-1""]
        [UTCDate ""2022.02.20""]
        [UTCTime ""19:10:28""]
        [WhiteElo ""?""]
        [BlackElo ""?""]
        [Variant ""Standard""]
        [TimeControl ""-""]
        [ECO ""B01""]
        [Opening ""Scandinavian Defense: Blackburne-Kloosterboer Gambit""]
        [Termination ""Normal""]
        [Annotator ""lichess.org""]

        1. e4 d5 2. exd5 c6 { B01 Scandinavian Defense: Blackburne-Kloosterboer Gambit } 
        3. dxc6 Nf6 4. cxb7 Bg4 5. bxa8=B e5 6. f4 exf4 7. g3 fxg3 8. Be2 g2 
        9. Bd3 gxh1=Q 10. c4 a5 11. c5 a4 12. c6 a3 13. Qe2+ Be7 14. Qe3 axb2 15. Qd4 bxa1=N { White resigns. } 0-1");

        Assert.Equal("Bn1qk2r/4bppp/2P2n2/8/3Q2b1/3B4/P2P3P/nNB1K1Nq w k - 0 16", board.ToFen());

        board = ChessBoard.LoadFromPgn(@"
        [Event ""Casual Rapid game""]
        [Site ""https://lichess.org/LiqfXWwa""]
        [Date ""2022.02.26""]
        [White ""Anonymous""]
        [Black ""Anonymous""]
        [Result ""0-1""]
        [UTCDate ""2022.02.26""]
        [UTCTime ""20:43:22""]
        [WhiteElo ""?""]
        [BlackElo ""?""]
        [Variant ""Standard""]
        [TimeControl ""600+0""]
        [ECO ""D06""]
        [Opening ""Queen's Gambit Declined: Marshall Defense""]
        [Termination ""Time forfeit""]
        [Annotator ""lichess.org""]

        1. d4 d5 2. c4 Nf6 { D06 Queen's Gambit Declined: Marshall Defense } 3. Nc3 dxc4 4. e3 g6 5. Bxc4 Bg7 6. Qb3 O-O 7. Nf3 c6 8. Ng5 e6 9. O-O b5 10. Bd3 a5 11. a4 bxa4 12. Nxa4 Na6 13. Nb6 Rb8 14. Bxa6 Bxa6 15. Qa3 Bxf1 16. Kxf1 Qxb6 17. Bd2 Ra8 18. Qe7 Qa6+ 19. Kg1 Bh6 20. Nxe6 fxe6 21. Qxe6+ Rf7 22. Rxa5 Qb7 { White left the game. } 0-1");

        Assert.Equal("r5k1/1q3r1p/2p1Qnpb/R7/3P4/4P3/1P1B1PPP/6K1 w - - 1 23", board.ToFen());
    }

    [Fact]
    public void Test_Pgn_With_EnPassant()
    {
        string pgn = @"[Variant ""From Position""]
        [FEN ""rnbqkbnr/pppp1ppp/8/4pP2/8/8/PPPPP1PP/RNBQKBNR w KQkq e6 0 1""]
        1. fxe6";

        var board = ChessBoard.LoadFromPgn(pgn);
        Assert.Null(board["e5"]);
        Assert.True(board.ExecutedMoves[^1].Parameter is MoveEnPassant);
    }

    [Fact]
    public void Test_Pgn_With_EnPassant_Extended_Original_Position()
    {
        string pgn = @"[Variant ""From Position""]
        [FEN ""rnbqkbnr/pppp1ppp/8/4pP2/8/8/PPPPP1PP/RNBQKBNR w KQkq e6 0 1""]
        1. f5xe6";

        var board = ChessBoard.LoadFromPgn(pgn);
        Assert.Null(board["e5"]);
        Assert.True(board.ExecutedMoves[^1].Parameter is MoveEnPassant);
    }

    [Fact]
    public void Test_Pgn_Should_Load()
    {
        // Issue #19
        string pgn = @"1.e4 e6 2.d4 d5 3.Nc3 Nf6 4.Bg5 dxe4 5.Nxe4 Be7 6.Bxf6 gxf6 7.Nf3 f5 8.Nc3 a6
        9.Qd2 b5 10.O-O-O b4 11.Na4 Bb7 12.Bc4 Bd5 13.Qe2 Nc6 14.Rhe1 Na5 15.Bxd5 Qxd5
        16.b3 Nc6 17.c4 bxc3 18.Nxc3 Qa5 19.Qc4 Nb4 20.Kb1 O-O 21.Ne5 Rad8 22.g4 f4
        23.Nd3 Nxd3 24.Rxd3 Bf6 25.Red1 Bg7 26.Qc5 Qxc5 27.dxc5 Rxd3 28.Rxd3 f5 29.gxf5 Bxc3
        30.Rxc3 Rxf5 31.Kc2 Rg5 32.Kd3 Rg2 33.Ke2 Rxh2 34.Rd3 e5 35.Rd5 e4 36.Rg5+ Kh8
        37.Rg4 e3 38.Rxf4 exf2 39.Rf7 c6 40.a4 Kg8 41.Rc7 Rh3 42.Rxc6 Rxb3 43.Rxa6 Rb2+
        44.Kf1 Kf7 45.a5 Ke8 46.Ra8+ Kd7 47.a6 Ra2 48.a7 Kc7 1/2-1/2";

        var board = ChessBoard.LoadFromPgn(pgn);
        Assert.True(board.IsEndGame);
    }

    [Fact]
    public void TestPgnLoadFromPosition()
    {
        var board = new ChessBoard();
        // From Position
        board = ChessBoard.LoadFromPgn(
            @"[Variant ""From Position""]
        [FEN ""rnbqkbnr/ppp1pppp/8/3p4/4P3/8/PPPP1PPP/RNBQKBNR w KQkq - 0 1""]
            
        1.exd5 e6 2.dxe6 fxe6");
        Assert.Equal("rnbqkbnr/ppp3pp/4p3/8/8/8/PPPP1PPP/RNBQKBNR w KQkq - 0 3", board.ToFen());

        board = ChessBoard.LoadFromPgn("");
        Assert.Equal("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", board.ToFen());

        // With alternative moves
        board = ChessBoard.LoadFromPgn(
        @"[Variant ""From Position""]
        [FEN ""rnbqkbnr/ppp1pppp/8/3p4/4P3/8/PPPP1PPP/RNBQKBNR w KQkq - 0 1""]
            
        1.exd5 e6 2.dxe6 fxe6 3.d4(3.f4 g5 4.fxg5) 3... c5 4.b4");
        Assert.Equal("rnbqkbnr/pp4pp/4p3/2p5/1P1P4/8/P1P2PPP/RNBQKBNR b KQkq b3 0 4", board.ToFen());

        board = ChessBoard.LoadFromPgn(
        @"[Variant ""From Position""]
        [FEN ""rnbqkbnr/ppp1pppp/8/3p4/4P3/8/PPPP1PPP/RNBQKBNR b KQkq - 0 1""]
            
        1... dxe4 2.g4");
        Assert.Equal("rnbqkbnr/ppp1pppp/8/8/4p1P1/8/PPPP1P1P/RNBQKBNR b KQkq g3 0 2", board.ToFen());
        Assert.Contains("1... dxe4 2. g4", board.ToPgn());

        board = ChessBoard.LoadFromPgn(
        @"[Variant ""From Position""]
        [FEN ""rnbqkbnr/ppp1p1pp/8/3p1p2/4P3/8/PPPP1PPP/RNBQKBNR b KQkq - 0 5""]");

        board.Move("dxe4");
        board.Move("f3");
        board.Move("exf3");
        board.Move("gxf3");
        board.Draw();

        Assert.Contains("5... dxe4 6. f3 exf3 7. gxf3 1/2-1/2", board.ToPgn());

        // Self made
        board = ChessBoard.LoadFromFen("rnb1kbnr/pppppppp/8/1q6/8/8/P1PPPPPP/R3K2R w KQkq - 0 1");

        board.Move("e4");
        board.Move("d5");
        board.Move("exd5");
        board.Resign(PieceColor.Black);

        Assert.Contains("1. e4 d5 2. exd5 1-0", board.ToPgn());
    }

    [Fact]
    public void TestFenLoad()
    {
        var board = new ChessBoard();
        var moves = new[]
        {
                "Rh2",
                "Rxd3",
                "Kc2",
                "Rf3",
                "Rg2+",
        };

        // En passant pos only 3rd rank possible of 6th
        Assert.Throws<ChessArgumentException>(() => ChessBoard.LoadFromFen("8/p7/7R/5pk1/8/3B1r2/PP3P2/2K5 w KQkq e1 1 34"));

        board = ChessBoard.LoadFromFen("8/p7/7R/5pk1/8/3B1r2/PP3P2/2K5 w - - 1 34");

        for (int i = 0; i < moves.Length; i++)
            Assert.True(board.Move(moves[i]));

        Assert.Equal("8/p7/8/5pk1/8/5r2/PPK2PR1/8 b - - 3 36", board.ToFen());

        // Checkmate
        board = ChessBoard.LoadFromFen("rnb1kbnr/pppppppp/8/8/5PPq/8/PPPPP2P/RNBQKBNR w KQkq - 0 1");

        Assert.Equal(EndgameType.Checkmate, board.EndGame.EndgameType);

        // Stalemate
        board = ChessBoard.LoadFromFen("rnb1kbnr/pppppppp/8/8/8/8/5q2/7K w kq - 0 1");

        Assert.Equal(EndgameType.Stalemate, board.EndGame.EndgameType);
    }

    [Fact]
    public void TestCastlingConversion()
    {
        var moves = new[]
        {
            "e3", "d6",
            "Be2", "Be6",
            "Nf3", "Qd7",
            "O-O", "Nc6",
            "d3", "O-O-O"
        };
        var board = new ChessBoard();

        foreach (var move in moves)
        {
            board.Move(move);
            if (move == "O-O")
                Assert.Equal("g1", board.ExecutedMoves[^1].NewPosition.ToString());
            if (move == "O-O-0")
                Assert.Equal("c8", board.ExecutedMoves[^1].NewPosition.ToString());
        }
    }
}