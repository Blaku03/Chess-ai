using ChessChallenge.API;
using System;
using System.Linq;

public class MyBot : IChessBot
{
    // pawn, knight, bishop, rook, queen, king
    readonly int[] PieceValues = { 100, 300, 300, 500, 900, 10000 };
    Move bestMove;
    const int MateScore = 100000;
    const int CheckScore = 100;
    const int NegativeInfinity = int.MinValue + 1;
    const int PositiveInfinity = -NegativeInfinity;
    public Move Think(Board board, Timer timer)
    {
        Console.WriteLine(FindBestMove(board, 6));
        return bestMove;
    }

    int FindBestMove(Board board, int depth, int distFromRoot = 0, int alpha = NegativeInfinity, int beta = PositiveInfinity)
    {
        if (board.IsInCheckmate()) return -MateScore;

        if (board.IsDraw()) return 0;

        if (board.IsRepeatedPosition() || board.IsFiftyMoveDraw()) return 10;

        Move[] possibleMoves = OrderMoves(board);

        //Stalemate
        if (possibleMoves.Length == 0) return 0;

        if (depth == 0) return EvaluatePosition(board);

        //TODO: Make that when there are force checks or caputers possible continue the search

        foreach (Move currentMove in possibleMoves)
        {
            board.MakeMove(currentMove);
            int moveEvaluation = -FindBestMove(board, depth - 1, distFromRoot + 1, -beta, -alpha);
            board.UndoMove(currentMove);

            if (moveEvaluation >= beta) return beta;

            if (moveEvaluation > alpha)
            {
                alpha = moveEvaluation;

                if (distFromRoot == 0)
                {
                    bestMove = currentMove;
                }
            }
        }

        return alpha;
    }

    Move[] OrderMoves(Board board)
    {
        var possibleMoves = board.GetLegalMoves();
        var moveScorePair = new (Move, int)[possibleMoves.Length];

        for (int i = 0; i < possibleMoves.Length; i++)
        {
            PieceType pieceCapture = possibleMoves[i].CapturePieceType;
            int moveScore = ((int)pieceCapture) * 50;
            board.MakeMove(possibleMoves[i]);
            if (board.IsInCheckmate()) moveScore += MateScore;
            if (board.IsInCheck()) moveScore += CheckScore;
            board.UndoMove(possibleMoves[i]);
            moveScorePair[i] = (possibleMoves[i], moveScore);
        }

        var sortedMovesPairs = moveScorePair.OrderByDescending(moveScorePair => moveScorePair.Item2).ToArray();
        var sortedMoves = sortedMovesPairs.Select(moveScorePair => moveScorePair.Item1).ToArray();

        return sortedMoves;
    }

    //Basic counting of piceses values
    int EvaluatePosition(Board board)
    {
        PieceList[] allPieces = board.GetAllPieceLists();
        int evaluation = 0;
        bool currentColorWhite = board.IsWhiteToMove;

        for (int i = 0; i < allPieces.Length; i++)
        {
            // Color switches at the middle
            if (i == allPieces.Length / 2) currentColorWhite = !currentColorWhite;
            int currentPieceValue = PieceValues[i % PieceValues.Length];
            if (!currentColorWhite) currentPieceValue *= -1;
            evaluation += currentPieceValue * allPieces[i].Count;
        }

        return evaluation;
    }
}