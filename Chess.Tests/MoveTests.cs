using Chess;
using System;
using System.Linq;
using Xunit;

namespace ChessUnitTests;

public class MoveTests
{
    [Fact]
    public void TestMove()
    {
        var moves = new[]
        {
            "e4", "e6",
            "Nc3", "c5",
            "Nf3", "g6",
            "d4", "Qc7",
            "dxc5", "Qxc5",
            "Bd3", "Nf6",
            "Na4", "Qa5+",
            "Nc3", "Qc5",
            "O-O", "Bd6",
            "Na4", "Qa5",
            "b3", "Nc6",
            "Bb2", "Bb4",
            "Bxf6", "b5",
            "Bxh8", "Ne5",
            "Nxe5", "Bd2",
            "Nc5", "Be1",
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
        Assert.Throws<ChessPieceNotFoundException>(() => board.Move(new Move("e3", "e4")));

        for (int i = 0; i < moves.Length; i++)
            Assert.True(board.Move(moves[i]));

        // Stalemate here
        board = ChessBoard.LoadFromFen("rnb1kbnr/pppppppp/8/8/8/8/5q2/7K w kq - 0 1");
        Assert.Throws<ChessGameEndedException>(() => board.Move(new Move("f2", "e2")));
    }

    [Theory]
    [InlineData("rnb1kbnr/pppppppp/8/4q3/8/8/PPPP1PPP/R3K2R w KQkq - 0 1", "O-O", false)]
    [InlineData("rnb1kbnr/pppppppp/8/5q2/8/8/PPPPP1PP/R3K2R w KQkq - 0 1", "O-O", false)]
    [InlineData("rnb1kbnr/pppppppp/8/6q1/8/8/PPPPPP1P/R3K2R w KQkq - 0 1", "O-O", false)]
    [InlineData("rnb1kbnr/pppppppp/8/7q/8/8/PPPPPPP1/R3K2R w KQkq - 0 1", "O-O", true)]
    [InlineData("rnb1kbnr/pppppppp/8/1q6/8/8/P1PPPPPP/R3K2R w KQkq - 0 1", "O-O-O", true)]
    [InlineData("rnb1kbnr/pppppppp/8/2q5/8/8/PP1PPPPP/R3K2R w KQkq - 0 1", "O-O-O", false)]
    public void TestCheckOnCastle(string fen, string sanMove, bool isValid)
    {
        var board = ChessBoard.LoadFromFen(fen);
        Assert.True(board.IsValidMove(sanMove) == isValid);
    }

    [Fact]
    public void TestSan()
    {
        var board = new ChessBoard();

        Assert.Throws<ChessArgumentException>(() => board.ParseFromSan("z4"));

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

        Assert.Throws<ChessSanTooAmbiguousException>(() => board.ParseFromSan("Nc3"));
        Assert.Throws<ChessSanNotFoundException>(() => board.ParseFromSan("Nc4"));

        board = ChessBoard.LoadFromFen("rnb1kbnr/pppppppp/8/8/8/8/4q3/7K b kq - 0 1");
        board.Move("Qf2");

        Assert.Equal("Qf2$", board.MovesToSan[^1]);
    }

    [Theory]
    [InlineData("7k/8/3Q4/8/8/3Q1Q2/8/4K3 w - - 0 1", "d3", "d5", "Qd3d5")]
    [InlineData("4k3/8/8/8/8/8/3N4/4K1N1 w - - 0 1", "d2", "f3", "Ndf3")]
    [InlineData("4k3/8/8/8/3N4/8/3N4/4K1N1 w - - 0 1", "d2", "f3", "N2f3")]
    [InlineData("4k3/8/8/8/8/8/3N3N/4K1N1 w - - 0 1", "d2", "f3", "Ndf3")]
    [InlineData("4k3/8/8/8/8/8/3N3N/4K1N1 w - - 0 1", "g1", "f3", "Ngf3")]
    [InlineData("4k3/8/8/8/8/8/3N3N/4K1N1 w - - 0 1", "h2", "f3", "Nhf3")]
    [InlineData("4k3/8/8/8/3N4/8/3N3N/4K3 w - - 0 1", "d2", "f3", "Nd2f3")]
    [InlineData("4k3/8/8/6N1/3N4/8/7N/4K3 w - - 0 1", "d4", "f3", "Ndf3")]
    public void TestSanAmbiguousMoveGeneration(string fen, string fromPos, string toPos, string expectedSan)
    {
        var board = ChessBoard.LoadFromFen(fen);

        board.Move(new Move(fromPos, toPos));

        Assert.Equal(expectedSan, board.ExecutedMoves[0].San);
    }

    [Theory]
    [InlineData("4k3/8/8/8/3N4/8/3N4/4K1N1 w - - 0 1", "N2f3", "4k3/8/8/8/3N4/5N2/8/4K1N1 b - - 1 1")]
    [InlineData("4k3/8/8/8/3N4/8/3N4/4K1N1 w - - 0 1", "Nd2f3", "4k3/8/8/8/3N4/5N2/8/4K1N1 b - - 1 1")]
    [InlineData("4k3/8/8/8/8/8/3N3N/4K1N1 w - - 0 1", "Ndf3", "4k3/8/8/8/8/5N2/7N/4K1N1 b - - 1 1")]
    [InlineData("4k3/8/8/8/8/8/3N3N/4K1N1 w - - 0 1", "Nd2f3", "4k3/8/8/8/8/5N2/7N/4K1N1 b - - 1 1")]
    [InlineData("4k3/8/8/8/3N4/8/3N3N/4K3 w - - 0 1", "Nd2f3", "4k3/8/8/8/3N4/5N2/7N/4K3 b - - 1 1")]
    public void TestAmbiguousMoveExecution(string fen, string san, string expectedFen)
    {
        var board = ChessBoard.LoadFromFen(fen);
        board.Move(san);
        Assert.Equal(expectedFen, board.ToFen());
    }

    [Fact]
    public void TestParseToSan()
    {
        var board = new ChessBoard();
        var move = new Move("e2", "e4");
        Assert.True(board.IsValidMove(move));

        string? san;
        Assert.True(board.TryParseToSan(move, out san));
        Assert.NotNull(san);
        Assert.Equal("e4", san);
    }

    [Fact]
    public void TestCapturedPieces()
    {
        var board = new ChessBoard();
        board = ChessBoard.LoadFromFen("1nbqkb1r/pppp1ppp/2N5/4p3/3P4/8/PPP1PPPP/RN2KB1R w KQk - 0 1");

        Assert.Equal(2, board.CapturedWhite.Length);
        Assert.Equal(2, board.CapturedBlack.Length);

        board.Move("dxe5");
        board.Move("dxc6");

        Assert.Equal(3, board.CapturedWhite.Length);
        Assert.Equal(3, board.CapturedBlack.Length);

        board = ChessBoard.LoadFromFen("rnbqkbnr/8/8/8/8/8/P7/RNBQKBNR w KQkq - 0 1");

        Assert.Equal(7, board.CapturedWhite.Length);
        Assert.Equal(8, board.CapturedBlack.Length);

        board = ChessBoard.LoadFromFen("1nbqkbn1/pppppppp/NpNpNpNp/pBpBpBpB/bPbPbPbP/PnPnPnPn/PPPPPPPP/1NBQKBN1 w - - 0 1");

        Assert.Equal(2, board.CapturedWhite.Length);
        Assert.Equal(2, board.CapturedBlack.Length);

        board.Clear();

        Assert.Empty(board.CapturedWhite);
        Assert.Empty(board.CapturedBlack);
    }

    [Fact]
    public void TestMoveIndex()
    {
        var board = new ChessBoard();
        var moves = new[]
        {
            "e4", "e6",
            "Nc3", "c5",
            "Nf3", "g6",
            "d4", "Qc7",
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

        board = ChessBoard.LoadFromFen("8/p7/8/5p2/5k2/3r2R1/PPK2P2/8 w - - 6 38");
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
            "e4", "d5",
            "exd5", "e6",
            "dxe6", "fxe6"
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

        board = ChessBoard.LoadFromFen("rnbqkbnr/ppppp2p/6P1/8/8/8/PPPP1PPP/RNBQKBNR b KQkq - 0 3");
        board.Move("hxg6");

        Assert.Single(board.CapturedWhite);
        Assert.Equal(2, board.CapturedBlack.Length);

        board.Cancel();

        Assert.Equal("rnbqkbnr/ppppp2p/6P1/8/8/8/PPPP1PPP/RNBQKBNR b KQkq - 0 3", board.ToFen());
        Assert.Empty(board.CapturedWhite);
        Assert.Equal(2, board.CapturedBlack.Length);
    }

    [Theory]
    // Stalemate
    [InlineData("rnb1kbnr/pppppppp/8/8/8/8/5q2/7K w kq - 0 1", 0)]
    // One possible move
    [InlineData("rnb1kbnr/pppppppp/8/8/5P1q/8/PPPPP1PP/RNBQKBNR w KQkq - 0 1", 1)]
    public void TestMovesCount(string fen, int movesCount)
    {
        var board = new ChessBoard();

        board = ChessBoard.LoadFromFen(fen);

        Assert.Equal(movesCount, board.Moves(false, false).Length);
    }

    [Theory]
    // King Castle
    [InlineData("r3kbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQK2R w KQkq - 0 1", 2, "e1")]
    // King/Queen Castle
    [InlineData("r3kbnr/pppppppp/8/8/8/8/PPPPPPPP/R3K2R w KQkq - 0 1", 4, "e1")]
    public void TestMovesCountOnPosition(string fen, int movesCount, string pos)
    {
        var board = new ChessBoard();

        board = ChessBoard.LoadFromFen(fen);

        Assert.Equal(movesCount, board.Moves(new Position(pos), false, false).Length);
    }

    [Fact]
    public void TestChessMechanics()
    {
        var board = new ChessBoard();

        // En Passant
        board = ChessBoard.LoadFromFen("rnbqkbnr/ppppp1pp/8/4Pp2/8/8/PPPP1PPP/RNBQKBNR w KQkq f6 0 1");
        Assert.True(board.IsValidMove(new Move("e5", "e6")));
        Assert.True(board.IsValidMove(new Move("e5", "f6")));

        board = ChessBoard.LoadFromFen("rnbqkbnr/ppp1pppp/8/8/3pP3/8/PPPP1PPP/RNBQKBNR b KQkq e3 0 1");
        Assert.True(board.IsValidMove(new Move("d4", "d3")));
        Assert.True(board.IsValidMove(new Move("d4", "e3")));

        board.Move("e5");
        board.Move("c4");

        Assert.True(board.IsValidMove(new Move("d4", "d3")));
        Assert.True(board.IsValidMove(new Move("d4", "c3")));

        board.Move("dxc3 e.p."); // e.p. is optional (from Long Algebraic Notation)
        Assert.True(board["c4"] is null);

        board.MoveIndex = -1;
        Assert.True(board.IsValidMove(new Move("d4", "d3")));
        Assert.True(board.IsValidMove(new Move("d4", "e3")));

        board.Last();
        board.Cancel();
        Assert.Equal("rnbqkbnr/ppp2ppp/8/4p3/2PpP3/8/PP1P1PPP/RNBQKBNR b KQkq c3 0 2", board.ToFen());

        // Castle
        board = ChessBoard.LoadFromFen("r3kbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQK2R w KQkq - 0 1");
        Assert.True(board.IsValidMove(new Move("e1", "h1")));

        board = ChessBoard.LoadFromFen("r3kbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQK2R b KQkq - 0 1");
        Assert.True(board.IsValidMove(new Move("e8", "a8")));

        board = ChessBoard.LoadFromFen("r3kbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQK2R w Qk - 0 1");
        Assert.False(board.IsValidMove(new Move("e1", "h1")));

        board = ChessBoard.LoadFromFen("r3kbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQK2R b Qk - 0 1");
        Assert.False(board.IsValidMove(new Move("e8", "a8")));

        board = ChessBoard.LoadFromFen("r3kbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQK2R w KQkq - 0 1");
        board.Move("Kf1");
        board.Move("Kd8");
        board.Move("Ke1");
        board.Move("Ke8");
        Assert.Equal("r3kbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQK2R w - - 4 3", board.ToFen());

        board = ChessBoard.LoadFromFen("r3kbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQK2R w KQkq - 0 1");
        board.Move("Rg1");
        board.Move("Rb8");
        board.Move("Rh1");
        board.Move("Ra8");
        Assert.Equal("r3kbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQK2R w Qk - 4 3", board.ToFen());

        // Promotion
        board = ChessBoard.LoadFromFen("rnbqkbnr/pPpppppp/8/8/8/8/P1PPPPPP/RNBQKBNR w KQkq - 0 1");
        board.Move(new Move("b7", "a8"));
        Assert.True(board["a8"].Type == PieceType.Queen);

        board = ChessBoard.LoadFromPgn(
            @"[Variant ""From Position""]
        [FEN ""rnbqkbnr/pPpppppp/8/8/8/8/P1PPPPPP/RNBQKBNR w KQkq - 0 1""]
            
        1.bxa8=R");
        Assert.True(board["a8"].Type == PieceType.Rook);

        // With prom result
        board = ChessBoard.LoadFromFen("rnbqkbnr/pPpppppp/8/8/8/8/P1PPPPPP/RNBQKBNR w KQkq - 0 1");

        board.OnPromotePawn += (sender, e) => e.PromotionResult = PromotionType.ToBishop;

        board.Move(new Move("b7", "a8"));

        Assert.True(board["a8"].Type == PieceType.Bishop);
    }

    [Theory]
    [InlineData(null, 20, 400, 8_902)]
    [InlineData("rnbq1k1r/pp1Pbppp/2p5/8/2B5/8/PPP1NnPP/RNBQK2R w KQ - 1 8", 44, 1_486, 62_379)]
    [InlineData("r3k2r/Pppp1ppp/1b3nbN/nP6/BBP1P3/q4N2/Pp1P2PP/R2Q1RK1 w kq - 0 1", 6, 264, 9_467)]
    public void TestNumOfMoves(string? fen, int oneCount, int twoCount, int threeCount)
    {
        var board = new ChessBoard();

        if (fen is not null)
        {
            board = ChessBoard.LoadFromFen(fen);
        }

        var numOfMoves = CountMoves(board, 1);
        Assert.Equal(oneCount, numOfMoves);

        numOfMoves = CountMoves(board, 2);
        Assert.Equal(twoCount, numOfMoves);

        numOfMoves = CountMoves(board, 3);
        Assert.Equal(threeCount, numOfMoves);
    }

    public int CountMoves(ChessBoard board, int depth)
    {
        if (depth == 0)
            return 1;

        var numOfMoves = 0;

        foreach (var move in board.Moves(false, false))
        {
            board.Move(move);
            numOfMoves += CountMoves(board, depth - 1);
            board.Cancel();
        }

        return numOfMoves;
    }

    [Fact]
    public void TestPromotion_Queen_And_Rook_Check_King()
    {
        var board = ChessBoard.LoadFromFen("k7/7P/8/8/8/8/8/K7 w - - 0 1");
        var moves = board.Moves(new Position(7, 6));

        Assert.True(moves.Where(
            move => (move.Parameter as MovePromotion).PromotionType == PromotionType.ToQueen
                    || (move.Parameter as MovePromotion).PromotionType == PromotionType.ToRook).All(move => move.IsCheck));

        Assert.True(moves.Where(
            move => (move.Parameter as MovePromotion).PromotionType == PromotionType.ToBishop
                    || (move.Parameter as MovePromotion).PromotionType == PromotionType.ToKnight).All(move => !move.IsCheck));
    }

    [Fact]
    public void TestPromotion_Knight_Mates_King()
    {
        var board = ChessBoard.LoadFromFen("R7/4rkrP/4ppp1/8/8/8/8/K7 w - - 0 1");
        var moves = board.Moves(new Position(7, 6));

        Assert.True(moves.Where(
            move => (move.Parameter as MovePromotion)!.PromotionType == PromotionType.ToKnight).All(move => move.IsCheck && move.IsMate));

        Assert.True(moves.Where(
            move => (move.Parameter as MovePromotion)!.PromotionType == PromotionType.ToBishop
                    || (move.Parameter as MovePromotion)!.PromotionType == PromotionType.ToRook
                    || (move.Parameter as MovePromotion)!.PromotionType == PromotionType.ToQueen).All(move => !move.IsCheck && !move.IsMate));
    }

    [Fact]
    public void MoveReturnedFromBoardMoves_Should_Be_Valid()
    {
        var board = ChessBoard.LoadFromFen("4k3/8/8/3Pp3/8/8/8/4K3 w - e6 0 2");
        var moves = board.Moves(new Position(3, 4)).Where(m => m.Parameter is MoveEnPassant);

        Assert.True(board.Move(moves.First()));
    }

    [Fact]
    public void ToFen_Should_IncludeEnPassant()
    {
        string fen = "rnbqkbnr/ppppp1pp/8/8/4P1pP/8/PPPP1P2/RNBQKBNR b KQkq h3 0 4";
        var board = ChessBoard.LoadFromFen(fen);

        Assert.Equal(fen, board.ToFen());
    }

    [Fact]
    public void Moves_QueenPromotion_ShouldHaveSan()
    {
        var board = ChessBoard.LoadFromFen("8/6P1/8/2k5/8/8/8/K7 w - - 0 1");
        var moves = board.Moves(generateSan: true);

        Assert.All(moves, m => Assert.NotNull(m.San));

        var move = moves.Single(m => m.NewPosition == new Position("g8") && m.Parameter is MovePromotion mp && mp.PromotionType == PromotionType.ToQueen);

        Assert.Equal("g8=Q", move.San);
    }

    [Fact]
    public void En_Passant_recognized_as_valid_move_on_check()
    {
        var board = ChessBoard.LoadFromFen("8/7p/3Bp2k/p2pP1pP/P3p1PK/8/r4R2/8 w - g6 0 39");
        var moves = board.Moves(generateSan: true);

        Assert.All(moves, m => Assert.NotNull(m.San));

        var move = moves.Single(m => m.NewPosition == new Position("g6") && m.Parameter is MoveEnPassant);

        Assert.Equal("hxg6", move.San);
    }

    [Fact]
    public void Castle_Should_Standardise_e1h1_to_e1g8()
    {
        var board = ChessBoard.LoadFromFen("r3kbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQK2R w KQkq - 0 1");
        board.StandardiseCastlingPositions = true;
        board.Move(new Move(new Position("e1"), new Position("h1")));
        Assert.Equal("g1", board.ExecutedMoves[^1].NewPosition.ToString());
    }
}