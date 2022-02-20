using Chess;
using System;
using Xunit;

namespace ChessUnitTests
{
    public class UnitTests
    {
        [Fact]
        public void TestMove()
        {
            var moves = new[]
            {
            "e4",   "e6",
            "Nc3",  "c5",
            "Nf3",  "g6",
            "d4",   "Qc7",
            "dxc5", "Qxc5",
            "Bd3",  "Nf6",
            "Na4",  "Qa5+",
            "Nc3",  "Qc5",
            "O-O",  "Bd6",
            "Na4",  "Qa5",
            "b3",   "Nc6",
            "Bb2",  "Bb4",
            "Bxf6", "b5",
            "Bxh8", "Ne5",
            "Nxe5", "Bd2",
            "Nc5",  "Be1",
            "Rxe1", "Qd2",
            "Qxd2", "f6",
            "Bxf6", "b4",
            "Qxb4", "a6",
            "Bxa6", "Rb8",
            "Bxc8", "h6",
            "Qxb8", "h5",
            "Bg5",
            };
            var board = new ChessBoard();
            
            Assert.Throws<ArgumentNullException>(() => board.Move(new Move(new(), new())));
            Assert.Throws<ChessPieceNotFoundException>(() => board.Move(new Move(new("e3"), new("e4"))));

            for (int i = 0; i < moves.Length; i++)
                Assert.True(board.Move(moves[i]));

            // Stalemate here
            board.LoadFen("rnb1kbnr/pppppppp/8/8/8/8/5q2/7K b kq - 0 1");
            Assert.Throws<ChessGameEndedException>(() => board.Move(new Move(new("f2"), new("e2"))));
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

            var raisedEndGame = 0;
            board.OnEndGame += (sender, e) => raisedEndGame++;

            // En passant pos only 3rd rank possible of 6th
            Assert.Throws<ArgumentException>(() => board.LoadFen("8/p7/7R/5pk1/8/3B1r2/PP3P2/2K5 w KQkq e1 1 34"));

            board.LoadFen("8/p7/7R/5pk1/8/3B1r2/PP3P2/2K5 w - - 1 34");

            for (int i = 0; i < moves.Length; i++)
                Assert.True(board.Move(moves[i]));

            Assert.Equal("8/p7/8/5pk1/8/5r2/PPK2PR1/8 b - - 3 36", board.ToFen());

            // Checkmate
            board.LoadFen("rnb1kbnr/pppppppp/8/8/5PPq/8/PPPPP2P/RNBQKBNR w KQkq - 0 1");

            Assert.Equal(1, raisedEndGame);
            Assert.Equal(EndgameType.Checkmate, board.EndGame.EndgameType);
            raisedEndGame = 0;

            // Stalemate
            board.LoadFen("rnb1kbnr/pppppppp/8/8/8/8/5q2/7K w kq - 0 1");

            Assert.Equal(1, raisedEndGame);
            Assert.Equal(EndgameType.Stalemate, board.EndGame.EndgameType);
        }

        [Fact]
        public void TestPgnLoad()
        {
            var board = new ChessBoard();

            //// Normal Pgn
            //board.LoadPgn(
            //@"[Event ""Live Chess""]
            //[Site ""Chess.com""]
            //[Date ""2022.01.11""]
            //[Round ""?""]
            //[White ""Milan1905""]
            //[Black ""Geras1mleo""]
            //[Result ""1-0""]
            //[ECO ""C47""]
            //[WhiteElo ""1006""]
            //[BlackElo ""626""]
            //[TimeControl ""600""]
            //[EndTime ""11:58:56 PST""]
            //[Termination ""Milan1905 won by resignation""]
            
            //1.e4 e5 2.Nf3 Nf6 3.Nc3 Nc6 4.Bb5 Bc5 5.Bxc6 bxc6 6.Nxe5 Bxf2+ 7.Kxf2 O-O
            //8.d4 d5 9.exd5 cxd5 10.Nc6 Ng4+ 11.Kg1 Qf6 12.Qf1 Qxc6 13.h3 Nf6 14.Bg5
            //Qb6 15.Bxf6 Qxf6 16.Qxf6 gxf6 17.Nxd5 Rb8 18.Nxf6+ Kh8 19.b3 Rb4 20.c3 Bb7
            //21.cxb4 1-0");

            //Assert.Equal("5r1k/pbp2p1p/5N2/8/1P1P4/1P5P/P5P1/R5KR b - - 0 21", board.ToFen());
            //Assert.Equal(EndgameType.Resigned, board.EndGame.EndgameType);

            //// From Position
            //board.LoadPgn(
            //@"[Variant ""From Position""]
            //[FEN ""rnbqkbnr/ppp1pppp/8/3p4/4P3/8/PPPP1PPP/RNBQKBNR w KQkq - 0 1""]
            
            //1.exd5 e6 2.dxe6 fxe6");
            //Assert.Equal("rnbqkbnr/ppp3pp/4p3/8/8/8/PPPP1PPP/RNBQKBNR w KQkq - 0 3", board.ToFen());

            //board.LoadPgn("");
            //Assert.Equal("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", board.ToFen());

            //// With alternative moves
            //board.LoadPgn(
            //@"[Variant ""From Position""]
            //[FEN ""rnbqkbnr/ppp1pppp/8/3p4/4P3/8/PPPP1PPP/RNBQKBNR w KQkq - 0 1""]
            
            //1.exd5 e6 2.dxe6 fxe6 3.d4(3.f4 g5 4.fxg5) 3... c5 4.b4");
            //Assert.Equal("rnbqkbnr/pp4pp/4p3/2p5/1P1P4/8/P1P2PPP/RNBQKBNR b KQkq b3 0 4", board.ToFen());

            //board.LoadPgn(
            //@"[Variant ""From Position""]
            //[FEN ""rnbqkbnr/ppp1pppp/8/3p4/4P3/8/PPPP1PPP/RNBQKBNR b KQkq - 0 1""]
            
            //1... dxe4 2.g4");
            //Assert.Equal("rnbqkbnr/ppp1pppp/8/8/4p1P1/8/PPPP1P1P/RNBQKBNR b KQkq g3 0 2", board.ToFen());

            //// Ultimate pgn
            //board.LoadPgn(
            //@"[Event ""Live Chess""]
            //[Site ""Chess.com""]
            //[Date ""2022.01.03""]
            //[Round ""?""]
            //[White ""Milan1905""]
            //[Black ""Geras1mleo""]
            //[Result ""1/2-1/2""]
            //[ECO ""C42""]
            //[WhiteElo ""1006""]
            //[BlackElo ""626""]
            //[TimeControl ""600""]
            //[EndTime ""9:19:18 PST""]
            //[Termination ""Game drawn by insufficient material""]
            
            //1.e4 e5 2.Nf3 Nf6 3.Nxe5 Nxe4 4.Qe2 d5 5.d3 Bd6 6.dxe4 Bxe5 7.exd5 Qxd5
            //8.c4 Qc5 9.Bf4 Nd7 10.Bxe5 Nxe5 11.Nc3 O-O 12.O-O-O Nxc4 13.Na4 Qc6 14.
            //Qxc4 Bg4 15.Qxc6 bxc6 16.Rd2 Rad8 17.Rxd8 Rxd8 18.Ba6 h6 19.h3 Bf5 20.g4
            //Be4 21.Re1 Rd4 22.Nc3 Bf3 23.Re8+ Kh7 24.Ne2 Bxe2 25.Bxe2 Rf4 26.Bd3+ g6
            //27.Re2 f5 28.gxf5 gxf5 29.Re7+ Kg6 30.Rxc7 Rf3 31.Rxc6+ Kg5 32.h4+ Kxh4
            //33.Rxh6+ Kg5 34.Rh2 Rxd3 35.Kc2 Rf3 36.Rg2+ Kh4 37.Kd2 Kh3 38.Rg8 Rxf2+
            //39.Kc3 Rf3+ 40.Kb4 Rf4+ 41.Ka3 Rf3+ 42.b3 a5 43.Ka4 Rf2 44.a3 Rf4+ 45.b4
            //axb4 46.axb4 Rg4 47.Rh8+ Rh4 48.Rxh4+ Kxh4 49.b5 f4 50.b6 f3 51.b7 f2 52.
            //b8=Q f1=Q 53.Qb4+ Kg3 54.Qb3+ Qf3 55.Qxf3+ Kxf3 1/2-1/2");

            //Assert.Equal("8/8/8/8/K7/5k2/8/8 w - - 0 56", board.ToFen());
            //Assert.True(board.IsEndGame);

            //board.LoadFen("rnb1kbnr/pppppppp/8/1q6/8/8/P1PPPPPP/R3K2R w KQkq - 0 1");

            //board.Move("e4");
            //board.Move("d5");
            //board.Move("exd5");
            //board.Resign(PieceColor.Black);

            //Assert.Contains("1. e4 d5 2. exd5 1-0", board.ToPgn());

            //board.LoadPgn(
            //@"[Variant ""From Position""]
            //[FEN ""rnbqkbnr/ppp1p1pp/8/3p1p2/4P3/8/PPPP1PPP/RNBQKBNR b KQkq - 0 5""]");

            //board.Move("dxe4");
            //board.Move("f3");
            //board.Move("exf3");
            //board.Move("gxf3");
            //board.Draw();

            //Assert.Contains("5... dxe4 6. f3 exf3 7. gxf3 1/2-1/2", board.ToPgn());

            //// Extra
            //board.LoadPgn(
            //@"[Event ""Live Chess""]
            //[Site ""Chess.com""]
            //[Date ""2022.02.20""]
            //[Round ""?""]
            //[White ""Hikaru""]
            //[Black ""tptagain""]
            //[Result ""1-0""]
            //[ECO ""A06""]
            //[WhiteElo ""3319""]
            //[BlackElo ""2909""]
            //[TimeControl ""60+1""]
            //[EndTime ""9:42:10 PST""]
            //[Termination ""Hikaru won by resignation""]

            //1. Nf3 d5 2. b3 Nf6 3. Bb2 Bf5 4. d3 e6 5. g3 h6 6. Ne5 Nbd7 7. Nxd7 Qxd7 8. Bg2
            //Be7 9. Nd2 O-O 10. e4 Bh7 11. Qe2 Rfd8 12. e5 Ne8 13. O-O a5 14. a4 c5 15. Rfd1
            //Nc7 16. Nf1 b5 17. Ne3 Rdb8 18. Bc1 bxa4 19. Rxa4 Rb4 20. Ra1 Nb5 21. Qe1 Nd4
            //22. Bd2 Rb5 23. Bxa5 h5 24. Bc3 Rc8 25. h4 Bg6 26. Bb2 Rbb8 27. Ra2 Ra8 28. Rda1
            //Rxa2 29. Rxa2 Nc6 30. Ra1 Qb7 31. Qe2 Nd4 32. Qd1 Qc7 33. f4 Qb6 34. Kh2 Rd8 35.
            //Qd2 Rc8 36. Rb1 Ra8 37. Ra1 Rxa1 38. Bxa1 Qa7 39. Bb2 Qa2 40. Qc1 Qa6 41. Qa1
            //Qb7 42. Bxd4 cxd4 43. Qxd4 Qc7 44. Qb2 Qb6 45. d4 Qa6 46. Bf1 Qa7 47. c3 Bd8 48.
            //b4 Qa8 49. Be2 Qa4 50. Bd1 Qb5 51. Be2 Qa4 52. Kg1 f6 53. Kf2 fxe5 54. fxe5 Bf7
            //55. Bd1 Qb5 56. Qa2 Qd3 57. Qa8 Qxc3 58. Qxd8+ Kh7 59. Bc2+ g6 60. Qf6 Qd2+ 61.
            //Kf3 Kg8 62. Bxg6 1-0");

            //Assert.Equal("6k1/5b2/4pQB1/3pP2p/1P1P3P/4NKP1/3q4/8 b - - 0 62", board.ToFen());
            //Assert.True(board.IsEndGame);
            //Assert.Equal(PieceColor.White, board.EndGame.WonSide);

            //board.LoadPgn(
            //@"[Event ""Live Chess""]
            //[Site ""Chess.com""]
            //[Date ""2022.02.19""]
            //[Round ""?""]
            //[White ""Hikaru""]
            //[Black ""Bigfish1995""]
            //[Result ""1-0""]
            //[ECO ""B22""]
            //[WhiteElo ""2864""]
            //[BlackElo ""2640""]
            //[TimeControl ""600""]
            //[EndTime ""12:17:19 PST""]
            //[Termination ""Hikaru won on time""]

            //1. e4 c5 2. Nf3 Nc6 3. c3 d5 4. exd5 Qxd5 5. Na3 Nf6 6. d4 e6 7. Nb5 Qd8 8. dxc5
            //Bxc5 9. Qxd8+ Kxd8 10. Bf4 Nd5 11. O-O-O Ke7 12. Bg3 a6 13. c4 Nf6 14. Nc3 Rd8
            //15. Bd3 Bd7 16. a3 a5 17. Rhe1 Nd4 18. Nxd4 Bxd4 19. Nd5+ Nxd5 20. cxd5 Bf6 21.
            //Be4 Ba4 22. Rd2 Rac8+ 23. Kb1 b6 24. Bf4 Rc4 25. f3 h6 26. dxe6 Rxd2 27. Bxd2
            //fxe6 28. Be3 Rc8 29. Re2 b5 30. Bb6 b4 31. Bxa5 bxa3 32. Bb4+ Kf7 33. bxa3 Bd4
            //34. Bc2 Bb5 35. Rd2 e5 36. f4 Bc4 37. fxe5 Bxe5 38. h3 Rb8 39. Be4 Ke6 40. Rc2
            //Bb5 41. Ka2 Ba4 42. Rd2 Bf4 43. Re2 Be5 44. Bc2 Bb5 45. Bb3+ Kf5 46. Rc2 Bd3 47.
            //Rc5 Ke4 48. Bc2 Bxc2 49. Rxc2 Bd4 50. Kb3 g5 51. Rc6 h5 52. Kc4 Bg1 53. a4 Ra8
            //54. a5 Kf4 55. Bd6+ Ke4 56. a6 Bf2 57. Bc5 Bxc5 58. Kxc5 Kf4 59. Kb6 Kg3 60. Rc2
            //Rb8+ 61. Kc7 Rf8 62. a7 g4 63. hxg4 hxg4 64. Ra2 Kh2 65. Kb7 Rf7+ 66. Ka6 Rf8
            //67. Kb5 g3 68. Kc6 Ra8 69. Kd5 Rxa7 70. Rxa7 Kxg2 71. Ke4 Kf2 72. Ra2+ Kg1 73.
            //Kf3 g2 74. Rxg2+ Kh1 (74... Kf1 75. Rf2+ Ke1 76. Re2+ Kd1 77. Rd2+ Kxd2) 75. Rg3
            //1-0");

            //Assert.Equal("8/8/8/8/8/5KR1/8/7k b - - 2 75", board.ToFen());

            board.LoadPgn(
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

            board.LoadPgn(
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
        }

        [Fact]
        public void TestCheck()
        {
            var board = new ChessBoard();

            var wRaisedCheck = 0;
            var bRaisedCheck = 0;

            board.OnWhiteKingCheckedChanged += (sender, args) => wRaisedCheck++;
            board.OnBlackKingCheckedChanged += (sender, args) => bRaisedCheck++;

            board.LoadFen("rnb1kbnr/pppppppp/8/8/5PPq/8/PPPPP2P/RNBQKBNR w KQkq - 0 1");

            Assert.Equal(1, wRaisedCheck);
            Assert.Equal(0, bRaisedCheck);
            wRaisedCheck = 0;
            bRaisedCheck = 0;

            board.Clear();

            Assert.Equal(1, wRaisedCheck);
            Assert.Equal(0, bRaisedCheck);
            wRaisedCheck = 0;
            bRaisedCheck = 0;

            board.LoadFen("rnb1bknr/ppp3pp/8/8/7q/8/PPP1Q1PP/RNB1KBNR w KQkq - 0 1");
            board.Move(new Move(new("e2"), new("f2")));

            Assert.Equal(2, wRaisedCheck);
            Assert.Equal(1, bRaisedCheck);
            wRaisedCheck = 0;
            bRaisedCheck = 0;

            board.Move(new Move(new("h4"), new("f6")));

            Assert.Equal(0, wRaisedCheck);
            Assert.Equal(1, bRaisedCheck);

            board.LoadFen("rnb1kbnr/pppppppp/8/4q3/8/8/PPPP1PPP/R3K2R w KQkq - 0 1");
            Assert.False(board.IsValidMove("O-O"));

            board.LoadFen("rnb1kbnr/pppppppp/8/5q2/8/8/PPPPP1PP/R3K2R w KQkq - 0 1");
            Assert.False(board.IsValidMove("O-O"));

            board.LoadFen("rnb1kbnr/pppppppp/8/6q1/8/8/PPPPPP1P/R3K2R w KQkq - 0 1");
            Assert.False(board.IsValidMove("O-O"));

            board.LoadFen("rnb1kbnr/pppppppp/8/7q/8/8/PPPPPPP1/R3K2R w KQkq - 0 1");
            Assert.True(board.IsValidMove("O-O"));

            board.LoadFen("rnb1kbnr/pppppppp/8/1q6/8/8/P1PPPPPP/R3K2R w KQkq - 0 1");
            Assert.True(board.IsValidMove("O-O-O"));

            board.LoadFen("rnb1kbnr/pppppppp/8/2q5/8/8/PP1PPPPP/R3K2R w KQkq - 0 1");
            Assert.False(board.IsValidMove("O-O-O"));
        }

        [Fact]
        public void TestSan()
        {
            var board = new ChessBoard();

            Assert.Throws<ArgumentNullException>(() => { string? s = null; board.San(s); });
            Assert.Throws<ArgumentException>(() => board.San("z4"));

            // 1. e4 e5 2. Ne2 f6 3. (TEST:"Nc3")
            var moves = new[]
            {
                "e4",
                "e5",
                "Ne2",
                "f6",
            };
            for (int i = 0; i < moves.Length; i++)
                Assert.True(board.Move(moves[i]));

            Assert.Throws<ChessSanTooAmbiguousException>(() => board.San("Nc3"));
            Assert.Throws<ChessSanNotFoundException>(() => board.San("Nc4"));
            // Disabled
            //Assert.Throws<ChessSanNotFoundException>(() => board.San("O-O"));

            board.LoadFen("rnb1kbnr/pppppppp/8/8/8/8/4q3/7K b kq - 0 1");
            board.Move("Qf2");

            Assert.Equal("Qf2$", board.MovesInSan[^1]);
        }

        [Fact]
        public void TestCapturedPieces()
        {
            var board = new ChessBoard();
            board.LoadFen("1nbqkb1r/pppp1ppp/2N5/4p3/3P4/8/PPP1PPPP/RN2KB1R w KQk - 0 1");

            Assert.Equal(2, board.CapturedWhite.Length);
            Assert.Equal(2, board.CapturedBlack.Length);

            board.Move("dxe5");
            board.Move("dxc6");

            Assert.Equal(3, board.CapturedWhite.Length);
            Assert.Equal(3, board.CapturedBlack.Length);

            board.LoadFen("rnbqkbnr/8/8/8/8/8/P7/RNBQKBNR w KQkq - 0 1");

            Assert.Equal(7, board.CapturedWhite.Length);
            Assert.Equal(8, board.CapturedBlack.Length);

            board.LoadFen("1nbqkbn1/pppppppp/NpNpNpNp/pBpBpBpB/bPbPbPbP/PnPnPnPn/PPPPPPPP/1NBQKBN1 w - - 0 1");

            Assert.Equal(2, board.CapturedWhite.Length);
            Assert.Equal(2, board.CapturedBlack.Length);

            board.Clear();

            Assert.Empty(board.CapturedWhite);
            Assert.Empty(board.CapturedBlack);
        }

        [Fact]
        public void TestPutRemove()
        {
            // Not fully implemented yet
            //var board = new ChessBoard();

            //board.LoadFen("rnb2bnr/pppppppp/4q3/1k6/8/8/PPPPPPPP/RNBQKBNR w KQ - 0 1");
            //board.Remove(new("e2"));

            //// Impossible in fact
            //Assert.True(board.WhiteKingChecked);
            //Assert.True(board.BlackKingChecked);

            //board.Put(new("wp"), new("e2"));

            //Assert.False(board.WhiteKingChecked);
            //Assert.False(board.BlackKingChecked);
        }

        [Fact]
        public void TestMoveIndex()
        {
            var board = new ChessBoard();
            var moves = new[]
            {
            "e4",   "e6",
            "Nc3",  "c5",
            "Nf3",  "g6",
            "d4",   "Qc7",
            "dxc5",
            };

            for (int i = 0; i < moves.Length; i++)
                board.Move(moves[i]);

            board.First();
            Assert.Equal("rnbqkbnr/pppppppp/8/8/4P3/8/PPPP1PPP/RNBQKBNR b KQkq e3 0 1", board.ToFen());
            Assert.Empty(board.CapturedWhite);
            Assert.Empty(board.CapturedBlack);

            board.Last();
            Assert.Equal("rnb1kbnr/ppqp1p1p/4p1p1/2P5/4P3/2N2N2/PPP2PPP/R1BQKB1R b KQkq - 0 5", board.ToFen());
            Assert.Empty(board.CapturedWhite);
            Assert.Single(board.CapturedBlack);

            board.MoveIndex = 2;
            Assert.Equal("rnbqkbnr/pppp1ppp/4p3/8/4P3/2N5/PPPP1PPP/R1BQKBNR b KQkq - 1 2", board.ToFen());
            Assert.Empty(board.CapturedWhite);
            Assert.Empty(board.CapturedBlack);

            board.Next();
            Assert.Equal("rnbqkbnr/pp1p1ppp/4p3/2p5/4P3/2N5/PPPP1PPP/R1BQKBNR w KQkq c6 0 3", board.ToFen());

            board.Previous();
            Assert.Equal("rnbqkbnr/pppp1ppp/4p3/8/4P3/2N5/PPPP1PPP/R1BQKBNR b KQkq - 1 2", board.ToFen());

            board.MoveIndex = -1;
            Assert.Equal("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", board.ToFen());

            board.LoadFen("8/p7/8/5p2/5k2/3r2R1/PPK2P2/8 w - - 6 38");
            board.Move("Kc1");
            board.Move("Rd2");

            board.First();
            Assert.Equal("8/p7/8/5p2/5k2/3r2R1/PP3P2/2K5 b - - 7 38", board.ToFen());

            board.Last();
            Assert.Equal("8/p7/8/5p2/5k2/6R1/PP1r1P2/2K5 w - - 8 39", board.ToFen());

            board.MoveIndex = -1;
            Assert.Equal("8/p7/8/5p2/5k2/3r2R1/PPK2P2/8 w - - 6 38", board.ToFen());
        }

        [Fact]
        public void TestCancel()
        {
            var board = new ChessBoard();
            var moves = new[]
            {
            "e4",   "d5",
            "exd5",  "e6",
            "dxe6",  "fxe6"
            };

            for (int i = 0; i < moves.Length; i++)
                board.Move(moves[i]);

            board.Cancel();
            Assert.Equal("rnbqkbnr/ppp2ppp/4P3/8/8/8/PPPP1PPP/RNBQKBNR b KQkq - 0 3", board.ToFen());

            board.Cancel();
            Assert.Equal("rnbqkbnr/ppp2ppp/4p3/3P4/8/8/PPPP1PPP/RNBQKBNR w KQkq - 0 3", board.ToFen());

            board.Cancel();
            Assert.Equal("rnbqkbnr/ppp1pppp/8/3P4/8/8/PPPP1PPP/RNBQKBNR b KQkq - 0 2", board.ToFen());

            board.Cancel();
            Assert.Equal("rnbqkbnr/ppp1pppp/8/3p4/4P3/8/PPPP1PPP/RNBQKBNR w KQkq d6 0 2", board.ToFen());

            board.Cancel();
            Assert.Equal("rnbqkbnr/pppppppp/8/8/4P3/8/PPPP1PPP/RNBQKBNR b KQkq e3 0 1", board.ToFen());

            board.Cancel();
            Assert.Equal("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", board.ToFen());

            board.LoadFen("rnbqkbnr/ppppp2p/6P1/8/8/8/PPPP1PPP/RNBQKBNR b KQkq - 0 3");
            board.Move("hxg6");

            Assert.Single(board.CapturedWhite);
            Assert.Equal(2, board.CapturedBlack.Length);

            board.Cancel();

            Assert.Equal("rnbqkbnr/ppppp2p/6P1/8/8/8/PPPP1PPP/RNBQKBNR b KQkq - 0 3", board.ToFen());
            Assert.Empty(board.CapturedWhite);
            Assert.Equal(2, board.CapturedBlack.Length);
        }

        [Fact]
        public void TestMoves()
        {
            var board = new ChessBoard();

            Assert.Equal(20, board.Moves().Length);

            // Stalemate
            board.LoadFen("rnb1kbnr/pppppppp/8/8/8/8/5q2/7K w kq - 0 1");
            Assert.Empty(board.Moves());

            board.LoadFen("rnb1kbnr/pppppppp/8/8/5P1q/8/PPPPP1PP/RNBQKBNR w KQkq - 0 1");
            Assert.Single(board.Moves());

            // King Castle 2 moves offering (e1-h1 OR e1-g1)
            board.LoadFen("r3kbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQK2R w KQkq - 0 1");
            Assert.Equal(3, board.Moves(new Position("e1")).Length);

            // 2 castles
            board.LoadFen("r3kbnr/pppppppp/8/8/8/8/PPPPPPPP/R3K2R w KQkq - 0 1");
            Assert.Equal(6, board.Moves(new Position("e1")).Length);
        }

        [Fact]
        public void TestEvents()
        {
            var raisedCapture = 0;
            var raisedInvalidMoveKingChecked = 0;
            var raisedPromotePawn = 0;
            var raisedEndGame = 0;

            var board = new ChessBoard();

            board.OnCaptured += (sender, e) => raisedCapture++;
            board.OnPromotePawn += (sender, e) => raisedPromotePawn++;
            board.OnInvalidMoveKingChecked += (sender, e) => raisedInvalidMoveKingChecked++;
            board.OnEndGame += (sender, e) => raisedEndGame++;

            board.LoadFen("rnb1kbnr/pppppppp/8/8/5Q1q/8/PPPPP1PP/RNB1KBNR w KQkq - 0 1");
            board.Move(new Move(new("f4"), new("h4")));

            board.LoadFen("r1bqkbnr/pPpppppp/8/8/8/8/PPPP3P/RNBQKBNR w KQkq - 0 1");
            board.Move(new Move(new("b7"), new("b8")));

            board.LoadFen("rnb1kbnr/pppppppp/8/8/5P1q/8/PPPPP1PP/RNBQKBNR w KQkq - 0 1");
            board.Move(new Move(new("e2"), new("e3")));

            board.LoadFen("rnbqkbnr/pppp1ppp/8/8/8/8/PPPPP2P/RNBQKBNR b KQkq - 0 1");
            board.Move(new Move(new("d8"), new("h4")));

            Assert.Equal(1, raisedCapture);
            Assert.Equal(1, raisedPromotePawn);
            Assert.Equal(1, raisedInvalidMoveKingChecked);
            Assert.Equal(1, raisedEndGame);
        }

        [Fact]
        public void TestChessMechanics()
        {
            var board = new ChessBoard();

            // En Passant
            board.LoadFen("rnbqkbnr/ppppp1pp/8/4Pp2/8/8/PPPP1PPP/RNBQKBNR w KQkq f6 0 1");
            Assert.True(board.IsValidMove(new Move(new("e5"), new("e6"))));
            Assert.True(board.IsValidMove(new Move(new("e5"), new("f6"))));

            board.LoadFen("rnbqkbnr/ppp1pppp/8/8/3pP3/8/PPPP1PPP/RNBQKBNR b KQkq e3 0 1");
            Assert.True(board.IsValidMove(new Move(new("d4"), new("d3"))));
            Assert.True(board.IsValidMove(new Move(new("d4"), new("e3"))));

            board.Move("e5");
            board.Move("c4");

            Assert.True(board.IsValidMove(new Move(new("d4"), new("d3"))));
            Assert.True(board.IsValidMove(new Move(new("d4"), new("c3"))));

            board.Move("dxc3 e.p."); // e.p. is optional (from Long Algebraic Notation)
            Assert.True(board["c4"] is null);

            board.MoveIndex = -1;
            Assert.True(board.IsValidMove(new Move(new("d4"), new("d3"))));
            Assert.True(board.IsValidMove(new Move(new("d4"), new("e3"))));

            // Castle
            board.LoadFen("r3kbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQK2R w KQkq - 0 1");
            Assert.True(board.IsValidMove(new Move(new("e1"), new("h1"))));

            board.LoadFen("r3kbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQK2R b KQkq - 0 1");
            Assert.True(board.IsValidMove(new Move(new("e8"), new("a8"))));

            board.LoadFen("r3kbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQK2R w Qk - 0 1");
            Assert.False(board.IsValidMove(new Move(new("e1"), new("h1"))));

            board.LoadFen("r3kbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQK2R b Qk - 0 1");
            Assert.False(board.IsValidMove(new Move(new("e8"), new("a8"))));

            board.LoadFen("r3kbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQK2R w KQkq - 0 1");
            board.Move("Kf1");
            board.Move("Kd8");
            board.Move("Ke1");
            board.Move("Ke8");
            Assert.Equal("r3kbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQK2R w - - 4 3", board.ToFen());

            board.LoadFen("r3kbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQK2R w KQkq - 0 1");
            board.Move("Rg1");
            board.Move("Rb8");
            board.Move("Rh1");
            board.Move("Ra8");
            Assert.Equal("r3kbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQK2R w Qk - 4 3", board.ToFen());

            // Promotion
            board.LoadFen("rnbqkbnr/pPpppppp/8/8/8/8/P1PPPPPP/RNBQKBNR w KQkq - 0 1");
            board.Move(new Move(new("b7"), new("a8")));
            Assert.True(board["a8"].Type == PieceType.Queen);

            board.LoadPgn(
            @"[Variant ""From Position""]
            [FEN ""rnbqkbnr/pPpppppp/8/8/8/8/P1PPPPPP/RNBQKBNR w KQkq - 0 1""]
            
            1.bxa8=R");
            Assert.True(board["a8"].Type == PieceType.Rook);

            // With prom result
            board.OnPromotePawn += (sender, e) => e.PromotionResult = PromotionType.ToBishop;
            board.LoadFen("rnbqkbnr/pPpppppp/8/8/8/8/P1PPPPPP/RNBQKBNR w KQkq - 0 1");
            board.Move(new Move(new("b7"), new("a8")));
            Assert.True(board["a8"].Type == PieceType.Bishop);
        }
    }
}