using Chess;
using System;
using Xunit;

namespace ChessUnitTests
{
    public class UnitTests
    {
        [Fact]
        public void TestMovesAndFenOut()
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

            Assert.Throws<ArgumentNullException>(() => board.Move(new Move(new Position(), new Position())));
            Assert.Throws<ChessPieceNotFoundException>(() => board.Move(new Move(new Position("e3"), new Position("e4"))));

            for (int i = 0; i < moves.Length; i++)
                board.Move(moves[i]);

            //var fenstr = board.ToFen();
            Assert.Equal("1QB1k3/3p4/4p1p1/2N1N1Bp/4P3/1P6/P1P2PPP/R3R1K1 b - - 1 23", board.ToFen());

            board.Draw();

            Assert.Throws<ChessGameEndedException>(() => board.Move(new Move(new Position("e8"), new Position("e7"))));
        }

        [Fact]
        public void TestFenLoadMoveFenOut()
        {
            var board = new ChessBoard();

            Assert.Throws<ArgumentException>(() => board.Load("8/p7/7R/5pk1/8/3B1r2/PP3P2/2K5 w KQkq e1 1 34"));

            board.Load("8/p7/7R/5pk1/8/3B1r2/PP3P2/2K5 w - - 1 34");

            var moves = new[]
            {
                "Rh2",
                "Rxd3",
                "Kc2",
                "Rf3",
                "Rg2+",
            };
            for (int i = 0; i < moves.Length; i++)
                board.Move(moves[i]);

            var fenstr = board.ToFen();
            Assert.Equal("8/p7/8/5pk1/8/5r2/PPK2PR1/8 b - - 3 36", fenstr);
        }
        // todo if fen is check/stale -mate

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

        }
    }
}