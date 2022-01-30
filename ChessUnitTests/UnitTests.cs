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
            /* 
            1. e4 e6 2. Nc3 c5 
            3. Nf3 g6 4. d4 Qc7 
            5. dxc5 Qxc5 6. Bd3 Nf6 
            7. Na4 Qa5+ 8. Nc3 Qc5 
            9. O-O Bd6 10. Na4 Qa5 
            11. b3 Nc6 12. Bb2 Bb4 
            13. Bxf6 b5 14. Bxh8 Ne5
            15. Nxe5 Bd2 16. Nc5 Be1 
            17. Rxe1 Qd2 18. Qxd2 f6 
            19. Bxf6 b4 20. Qxb4 a6 
            21. Bxa6 Rb8 22. Bxc8 h6 
            23. Qxb8 h5 24. Bg5 
             */
            var moves = new[]
            {
            "e4",
            "e6",
            "Nc3",
            "c5",
            "Nf3",
            "g6",
            "d4",
            "Qc7",
            "dxc5",
            "Qxc5",
            "Bd3",
            "Nf6",
            "Na4",
            "Qa5+",
            "Nc3",
            "Qc5",
            "O-O",
            "Bd6",
            "Na4",
            "Qa5",
            "b3",
            "Nc6",
            "Bb2",
            "Bb4",
            "Bxf6",
            "b5",
            "Bxh8",
            "Ne5",
            "Nxe5",
            "Bd2",
            "Nc5",
            "Be1",
            "Rxe1",
            "Qd2",
            "Qxd2",
            "f6",
            "Bxf6",
            "b4",
            "Qxb4",
            "a6",
            "Bxa6",
            "Rb8",
            "Bxc8",
            "h6",
            "Qxb8",
            "h5",
            "Bg5",
            };
            var board = new ChessBoard();
            var expected = "1QB1k3/3p4/4p1p1/2N1N1Bp/4P3/1P6/P1P2PPP/R3R1K1 b - - 1 23";

            for (int i = 0; i < moves.Length; i++)
                board.Move(moves[i]);

            var fenstr = board.ToFen();
            Assert.Equal(expected, fenstr);
        }

        [Fact]
        public void TestFenLoadMoveFenOut()
        {
            var board = new ChessBoard();

            Assert.Throws<ArgumentException>(()=> { board.Load("8/p7/7R/5pk1/8/3B1r2/PP3P2/2K5 w KQkq e1 1 34"); });

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
            Assert.Throws<ArgumentException>(() => { board.San("z4"); });

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

            Assert.Throws<ChessSanTooAmbiguousException>(() => { board.San("Nc3"); });
            Assert.Throws<ChessSanNotFoundException>(() => { board.San("Nc4"); });
            Assert.Throws<ChessSanNotFoundException>(() => { board.San("O-O"); });
        }

        // todo new move new position new position
    }
}