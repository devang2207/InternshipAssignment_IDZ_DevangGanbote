using UnityEngine;
using System.Collections.Generic;

namespace Chess.Scripts.Core
{
    public class ChessPlayerPlacementHandler : MonoBehaviour
    {
        #region Variables
        [Header("Initial Position")]
        [SerializeField] public int row;
        [SerializeField] public int column;

        [Header("Piece Type")]
        [SerializeField] ChessBoardPlacementHandler.PieceType pieceType = ChessBoardPlacementHandler.PieceType.None;

        [Header("Board Handler")]
        private ChessBoardPlacementHandler _boardHandler;

        #endregion

        #region Piece Init
        private void Start()
        {
            if (_boardHandler == null)
            {
                _boardHandler = ChessBoardPlacementHandler.Instance;
            }
            PlacePiece(row, column);
        }

        // Position the piece at its initial tile.
        private void PlacePiece(int row, int col)
        {
            var tile = _boardHandler.GetTile(row, col);
            if (tile == null)
            {
                Debug.LogError("tile location is invalid");
                return;
            }
            transform.position = _boardHandler.GetTile(row, col).transform.position;
            _boardHandler.AddPieceToDictionary(row, col, pieceType);//add this piece to dictionary of pieces
        }
        #endregion

        #region PossibleMoves
        internal void ShowPossibleMoves()
        {
            HighlightPossibleMoves(pieceType, row, column);//highlight moves according to piece type
        }
        //clears all highlights
        internal void ClearAllHighlights()
        {
            _boardHandler.ClearHighlights();
        }

        internal void HighlightPossibleMoves(ChessBoardPlacementHandler.PieceType pieceType, int currentRow, int currentCol)
        {
            //clear highlights 
            _boardHandler.ClearHighlights();

            //list of moves and 
            List<(int row, int col)> validMoves;
            List<(int row, int col)> capturePositions;
            switch (pieceType)
            {
                case ChessBoardPlacementHandler.PieceType.Pawn:
                    (validMoves, capturePositions) = ChessMoveGenerator.GetPawnMoves(currentRow, currentCol, _boardHandler);
                    break;
                case ChessBoardPlacementHandler.PieceType.King:
                    (validMoves, capturePositions) = ChessMoveGenerator.GetKingMoves(currentRow, currentCol, _boardHandler);
                    break;
                case ChessBoardPlacementHandler.PieceType.Queen:
                    (validMoves, capturePositions) = ChessMoveGenerator.GetQueenMoves(currentRow, currentCol, _boardHandler);
                    break;
                case ChessBoardPlacementHandler.PieceType.Rook:
                    (validMoves, capturePositions) = ChessMoveGenerator.GetRookMoves(currentRow, currentCol, _boardHandler);
                    break;
                case ChessBoardPlacementHandler.PieceType.Bishop:
                    (validMoves, capturePositions) = ChessMoveGenerator.GetBishopMoves(currentRow, currentCol, _boardHandler);
                    break;
                case ChessBoardPlacementHandler.PieceType.Knight:
                    (validMoves, capturePositions) = ChessMoveGenerator.GetKnightMoves(currentRow, currentCol, _boardHandler);
                    break;
                default:
                    validMoves = new List<(int, int)>();
                    capturePositions = new List<(int, int)>();
                    break;

            }
            foreach (var (row, col) in capturePositions)//highlight enemy pieces with red 
            {
                _boardHandler.HighlightEnemy(row, col);
            }
            foreach (var (row, col) in validMoves)//highlight possible moves 
            {
                _boardHandler.Highlight(row, col);
            }
        }
        #endregion
    }
}