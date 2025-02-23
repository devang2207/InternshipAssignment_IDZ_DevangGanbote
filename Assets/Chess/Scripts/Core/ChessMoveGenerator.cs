using System.Collections.Generic;

namespace Chess.Scripts.Core
{
    //Helper script that returns list of Moves And EnemyPieces for the given type of piece
    public static class ChessMoveGenerator
    {
        private const int BoardSize = 8;

        // Returns a tuple of lists: (normal moves, enemy moves)
        internal static (List<(int row, int col)> moves, List<(int row, int col)> enemyMoves) GetPawnMoves(int currentRow, int currentCol, ChessBoardPlacementHandler boardHandler)
        {
            var moves = new List<(int row, int col)>();
            var enemyMoves = new List<(int row, int col)>();
            int direction = 1; // Adjust as needed for pawn color

            int forwardRow = currentRow + direction;
            if (IsWithinBounds(forwardRow, currentCol) && IsTileEmpty(forwardRow, currentCol, boardHandler))
            {
                moves.Add((forwardRow, currentCol));
            }
            // Check diagonals for enemy pieces.
            int diagLeft = currentCol - 1;
            if (IsWithinBounds(forwardRow, diagLeft) && IsEnemyPiece(forwardRow, diagLeft, boardHandler))
            {
                enemyMoves.Add((forwardRow, diagLeft));
            }
            int diagRight = currentCol + 1;
            if (IsWithinBounds(forwardRow, diagRight) && IsEnemyPiece(forwardRow, diagRight, boardHandler))
            {
                enemyMoves.Add((forwardRow, diagRight));
            }

            return (moves, enemyMoves);
        }

        internal static (List<(int row, int col)> moves, List<(int row, int col)> enemyMoves) GetKingMoves(int currentRow, int currentCol, ChessBoardPlacementHandler boardHandler)
        {
            var moves = new List<(int row, int col)>();
            var enemyMoves = new List<(int row, int col)>();

            //go through each tile around the king and add possible moves
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (i == 0 && j == 0)//skip if kings location
                        continue;

                    int targetRow = currentRow + i;
                    int targetCol = currentCol + j;

                    if (!IsWithinBounds(targetRow, targetCol))
                        continue;

                    if (IsEnemyPiece(targetRow, targetCol, boardHandler))
                    {
                        enemyMoves.Add((targetRow, targetCol));
                    }
                    else if (IsTileEmpty(targetRow, targetCol, boardHandler))
                    {
                        moves.Add((targetRow, targetCol));
                    }
                }
            }
            return (moves, enemyMoves);
        }

        // A helper function for linear moves that takes directions to check for.
        private static (List<(int row, int col)> moves, List<(int row, int col)> enemyMoves) GetLinearMoves(int currentRow, int currentCol, List<(int rowDir, int colDir)> directions, ChessBoardPlacementHandler boardHandler)
        {
            var moves = new List<(int row, int col)>();
            var enemyMoves = new List<(int row, int col)>();

            foreach (var (rowDir, colDir) in directions)
            {
                for (int i = 1; i < BoardSize; i++)
                {
                    int newRow = currentRow + i * rowDir;
                    int newCol = currentCol + i * colDir;
                    if (!IsWithinBounds(newRow, newCol))
                        break;

                    if (IsEnemyPiece(newRow, newCol, boardHandler))
                    {
                        enemyMoves.Add((newRow, newCol));
                        break; // Stop moving in this direction after an enemy is detected.
                    }
                    else if (IsTileEmpty(newRow, newCol, boardHandler))
                    {
                        moves.Add((newRow, newCol));
                    }
                    else // There is friendly piece here
                    {
                        break;
                    }
                }
            }
            return (moves, enemyMoves);
        }

        //Rook moves in all directions using 
        internal static (List<(int row, int col)> moves, List<(int row, int col)> enemyMoves) GetRookMoves(int currentRow, int currentCol, ChessBoardPlacementHandler boardHandler)
        {
            return GetLinearMoves(currentRow, currentCol, new List<(int, int)> { (1, 0), (-1, 0), (0, 1), (0, -1) }, boardHandler);
        }

        internal static (List<(int row, int col)> moves, List<(int row, int col)> enemyMoves) GetBishopMoves(int currentRow, int currentCol, ChessBoardPlacementHandler boardHandler)
        {
            return GetLinearMoves(currentRow, currentCol, new List<(int, int)> { (1, 1), (1, -1), (-1, 1), (-1, -1) }, boardHandler);
        }

        internal static (List<(int row, int col)> moves, List<(int row, int col)> enemyMoves) GetQueenMoves(int currentRow, int currentCol, ChessBoardPlacementHandler boardHandler)
        {
            // Combine rook and bishop moves for the queen.
            var rookResult = GetRookMoves(currentRow, currentCol, boardHandler);
            var bishopResult = GetBishopMoves(currentRow, currentCol, boardHandler);

            var moves = new List<(int, int)>();
            moves.AddRange(rookResult.moves);
            moves.AddRange(bishopResult.moves);

            var enemyMoves = new List<(int, int)>();
            enemyMoves.AddRange(rookResult.enemyMoves);
            enemyMoves.AddRange(bishopResult.enemyMoves);

            return (moves, enemyMoves);
        }

        internal static (List<(int row, int col)> moves, List<(int row, int col)> enemyMoves) GetKnightMoves(int currentRow, int currentCol, ChessBoardPlacementHandler boardHandler)
        {
            var moves = new List<(int row, int col)>();
            var enemyMoves = new List<(int row, int col)>();
            var potentialMoves = new List<(int, int)>
        {
            //every possible move of a knight.
            (currentRow + 2, currentCol + 1),
            (currentRow + 2, currentCol - 1),
            (currentRow - 2, currentCol + 1),
            (currentRow - 2, currentCol - 1),
            (currentRow + 1, currentCol + 2),
            (currentRow + 1, currentCol - 2),
            (currentRow - 1, currentCol + 2),
            (currentRow - 1, currentCol - 2)
        };

            foreach (var (row, col) in potentialMoves)
            {
                if (!IsWithinBounds(row, col))
                    continue;
                //add to enemy piece list if enemy piece detected
                if (IsEnemyPiece(row, col, boardHandler))
                {
                    enemyMoves.Add((row, col));
                }
                else if (IsTileEmpty(row, col, boardHandler))//else add to moves
                {
                    moves.Add((row, col));
                }
            }
            return (moves, enemyMoves);
        }

        #region Helper Methods
        //checks if there is a tile at given row , col
        private static bool IsWithinBounds(int row, int col)
        {
            return row >= 0 && row < BoardSize && col >= 0 && col < BoardSize;
        }

        // Returns true if there is no piece at the given tile.
        private static bool IsTileEmpty(int row, int col, ChessBoardPlacementHandler boardHandler)
        {
            return !boardHandler.piecePositions.ContainsKey((row, col));
        }
        //checks if the row and col has enemy piece
        private static bool IsEnemyPiece(int row, int col, ChessBoardPlacementHandler boardHandler)
        {
            if (boardHandler.piecePositions.TryGetValue((row, col), out var type))
            {
                return type == ChessBoardPlacementHandler.PieceType.Enemy;
            }
            return false;
        }

        #endregion
    }

}

