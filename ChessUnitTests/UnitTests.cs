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

            // Normal Pgn
            board.LoadPgn(
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

            // From Position
            board.LoadPgn(
            @"[Variant ""From Position""]
            [FEN ""rnbqkbnr/ppp1pppp/8/3p4/4P3/8/PPPP1PPP/RNBQKBNR w KQkq - 0 1""]
            
            1.exd5 e6 2.dxe6 fxe6");
            Assert.Equal("rnbqkbnr/ppp3pp/4p3/8/8/8/PPPP1PPP/RNBQKBNR w KQkq - 0 3", board.ToFen());

            board.LoadPgn("");
            Assert.Equal("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", board.ToFen());

            // With alternative moves
            board.LoadPgn(
            @"[Variant ""From Position""]
            [FEN ""rnbqkbnr/ppp1pppp/8/3p4/4P3/8/PPPP1PPP/RNBQKBNR w KQkq - 0 1""]
            
            1.exd5 e6 2.dxe6 fxe6 3.d4(3.f4 g5 4.fxg5) 3... c5 4.b4");
            Assert.Equal("rnbqkbnr/pp4pp/4p3/2p5/1P1P4/8/P1P2PPP/RNBQKBNR b KQkq b3 0 4", board.ToFen());

            board.LoadPgn(
            @"[Variant ""From Position""]
            [FEN ""rnbqkbnr/ppp1pppp/8/3p4/4P3/8/PPPP1PPP/RNBQKBNR b KQkq - 0 1""]
            
            1... dxe4 2.g4");
            Assert.Equal("rnbqkbnr/ppp1pppp/8/8/4p1P1/8/PPPP1P1P/RNBQKBNR b KQkq g3 0 2", board.ToFen());

            // Ultimate pgn
            board.LoadPgn(
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

            board.LoadFen("rnb1kbnr/pppppppp/8/1q6/8/8/P1PPPPPP/R3K2R w KQkq - 0 1");

            board.Move("e4");
            board.Move("d5");
            board.Move("exd5");
            board.Resign(PieceColor.Black);

            Assert.Contains("1. e4 d5 2. exd5 1-0", board.ToPgn());

            board.LoadPgn(
            @"[Variant ""From Position""]
            [FEN ""rnbqkbnr/ppp1p1pp/8/3p1p2/4P3/8/PPPP1PPP/RNBQKBNR b KQkq - 0 5""]");

            board.Move("dxe4");
            board.Move("f3");
            board.Move("exf3");
            board.Move("gxf3");
            board.Draw();

            Assert.Contains("5... dxe4 6. f3 exf3 7. gxf3 1/2-1/2", board.ToPgn());
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