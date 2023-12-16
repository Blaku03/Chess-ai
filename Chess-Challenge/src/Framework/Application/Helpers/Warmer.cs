using ChessChallenge.API;

namespace ChessChallenge.Application
{
    public static class Warmer
    {

        public static void Warm()
        {
            Chess.Board b = new();
//            b.LoadStartPosition();
            b.LoadPosition("2k5/4q3/8/2n3p1/6P1/2PPQ3/2K5/8 w - - 0 1");
            Board board = new Board(b);
            Move[] moves = board.GetLegalMoves();

            board.MakeMove(moves[0]);
            board.UndoMove(moves[0]);
        }

    }
}
