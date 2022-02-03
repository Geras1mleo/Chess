using Chess;
using System;
using System.Threading;
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
            board.Load("rnb1kbnr/pppppppp/8/8/8/8/5q2/7K b kq - 0 1");
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

            var callEndGame = 0;
            board.OnEndGame += (sender, e) => callEndGame++;

            // En passant pos only 3rd rank possible of 6th
            Assert.Throws<ArgumentException>(() => board.Load("8/p7/7R/5pk1/8/3B1r2/PP3P2/2K5 w KQkq e1 1 34"));

            board.Load("8/p7/7R/5pk1/8/3B1r2/PP3P2/2K5 w - - 1 34");

            for (int i = 0; i < moves.Length; i++)
                Assert.True(board.Move(moves[i]));

            Assert.Equal("8/p7/8/5pk1/8/5r2/PPK2PR1/8 b - - 3 36", board.ToFen());

            // Checkmate
            board.Load("rnb1kbnr/pppppppp/8/8/5PPq/8/PPPPP2P/RNBQKBNR w KQkq - 0 1");

            Assert.Equal(1, callEndGame);
            Assert.Equal(EndgameType.Checkmate, board.EndGame.EndgameType);
            callEndGame = 0;

            // Stalemate
            board.Load("rnb1kbnr/pppppppp/8/8/8/8/5q2/7K w kq - 0 1");

            Assert.Equal(1, callEndGame);
            Assert.Equal(EndgameType.Stalemate, board.EndGame.EndgameType);
        }

        [Fact]
        public void TestCheck()
        {
            var board = new ChessBoard();

            var wCalledCheck = 0;
            var bCalledCheck = 0;

            board.OnWhiteKingCheckedChanged += (sender, args) => wCalledCheck++;
            board.OnBlackKingCheckedChanged += (sender, args) => bCalledCheck++;

            board.Load("rnb1kbnr/pppppppp/8/8/5PPq/8/PPPPP2P/RNBQKBNR w KQkq - 0 1");

            Assert.Equal(1, wCalledCheck);
            Assert.Equal(0, bCalledCheck);
            wCalledCheck = 0;
            bCalledCheck = 0;

            board.Clear();

            Assert.Equal(1, wCalledCheck);
            Assert.Equal(0, bCalledCheck);
            wCalledCheck = 0;
            bCalledCheck = 0;

            board.Load("rnb1bknr/ppp3pp/8/8/7q/8/PPP1Q1PP/RNB1KBNR w KQkq - 0 1");
            board.Move(new Move(new("e2"), new("f2")));

            Assert.Equal(2, wCalledCheck);
            Assert.Equal(1, bCalledCheck);
            wCalledCheck = 0;
            bCalledCheck = 0;

            board.Move(new Move(new("h4"), new("f6")));

            Assert.Equal(0, wCalledCheck);
            Assert.Equal(1, bCalledCheck);
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
            Assert.Throws<ChessSanNotFoundException>(() => board.San("O-O"));

            board.Load("rnb1kbnr/pppppppp/8/8/8/8/4q3/7K b kq - 0 1");
            board.Move("Qf2");

            Assert.Equal("Qf2$", board.ExecutedMoves[^1].San);
        }

        [Fact]
        public void TestCapturedPieces()
        {
            var board = new ChessBoard();
            board.Load("1nbqkb1r/pppp1ppp/2N5/4p3/3P4/8/PPP1PPPP/RN2KB1R w KQk - 0 1");

            Assert.Equal(2, board.WhiteCaptured.Length);
            Assert.Equal(2, board.BlackCaptured.Length);

            board.Move("dxe5");
            board.Move("dxc6");

            Assert.Equal(3, board.WhiteCaptured.Length);
            Assert.Equal(3, board.BlackCaptured.Length);

            board.Load("rnbqkbnr/8/8/8/8/8/P7/RNBQKBNR w KQkq - 0 1");

            Assert.Equal(7, board.WhiteCaptured.Length);
            Assert.Equal(8, board.BlackCaptured.Length);

            board.Load("1nbqkbn1/pppppppp/NpNpNpNp/pBpBpBpB/bPbPbPbP/PnPnPnPn/PPPPPPPP/1NBQKBN1 w - - 0 1");

            Assert.Equal(2, board.WhiteCaptured.Length);
            Assert.Equal(2, board.BlackCaptured.Length);

            board.Clear();

            Assert.Empty(board.WhiteCaptured);
            Assert.Empty(board.BlackCaptured);
        }

        [Fact]
        public void TestPutRemove()
        {
            var board = new ChessBoard();

            board.Load("rnb2bnr/pppppppp/4q3/1k6/8/8/PPPPPPPP/RNBQKBNR w KQ - 0 1");
            board.Remove(new("e2"));

            // Impossible in fact
            Assert.True(board.WhiteKingChecked);
            Assert.True(board.BlackKingChecked);

            board.Put(new("wp"), new("e2"));

            Assert.False(board.WhiteKingChecked);
            Assert.False(board.BlackKingChecked);
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
            Assert.Empty(board.WhiteCaptured);
            Assert.Empty(board.BlackCaptured);

            board.Last();
            Assert.Equal("rnb1kbnr/ppqp1p1p/4p1p1/2P5/4P3/2N2N2/PPP2PPP/R1BQKB1R b KQkq - 0 5", board.ToFen());
            Assert.Empty(board.WhiteCaptured);
            Assert.Single(board.BlackCaptured);

            board.MoveIndex = 2;
            Assert.Equal("rnbqkbnr/pppp1ppp/4p3/8/4P3/2N5/PPPP1PPP/R1BQKBNR b KQkq - 1 2", board.ToFen());
            Assert.Empty(board.WhiteCaptured);
            Assert.Empty(board.BlackCaptured);

            board.Next();
            Assert.Equal("rnbqkbnr/pp1p1ppp/4p3/2p5/4P3/2N5/PPPP1PPP/R1BQKBNR w KQkq c6 0 3", board.ToFen());

            board.Previous();
            Assert.Equal("rnbqkbnr/pppp1ppp/4p3/8/4P3/2N5/PPPP1PPP/R1BQKBNR b KQkq - 1 2", board.ToFen());

            board.MoveIndex = -1;
            Assert.Equal("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", board.ToFen());

            board.Load("8/p7/8/5p2/5k2/3r2R1/PPK2P2/8 w - - 6 38");
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

            board.Load("rnbqkbnr/ppppp2p/6P1/8/8/8/PPPP1PPP/RNBQKBNR b KQkq - 0 3");
            board.Move("hxg6");

            Assert.Single(board.WhiteCaptured);
            Assert.Equal(2, board.BlackCaptured.Length);

            board.Cancel();

            Assert.Equal("rnbqkbnr/ppppp2p/6P1/8/8/8/PPPP1PPP/RNBQKBNR b KQkq - 0 3", board.ToFen());
            Assert.Empty(board.WhiteCaptured);
            Assert.Equal(2, board.BlackCaptured.Length);
        }
    }
}