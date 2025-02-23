using System;
using UnityEngine;
using System.Diagnostics.CodeAnalysis;
using System.Collections.Generic;

[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
public sealed class ChessBoardPlacementHandler : MonoBehaviour
{
    //having a dictionary to store piecepositions and piece type on the board to later access
    internal Dictionary<(int row, int col), PieceType> piecePositions = new Dictionary<(int row, int col), PieceType>();
    internal enum PieceType //piece types for pieces                            
    {
        None, Pawn, King, Queen, Rook, Bishop, Knight,Enemy
    }

    [Header("Row GameObjects")]
    [SerializeField] private GameObject[] _rowsArray;

    [Header("Highlight Prefabs")]
    [SerializeField] private GameObject _highlightPrefab;
    [SerializeField] private GameObject _enemyHighlightPrefab;

    private GameObject[,] _chessBoard;

    internal static ChessBoardPlacementHandler Instance;     //using Singleton to access the piece positions on board
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        GenerateArray();
    }
    //adds given piece in dictionary at given row col 
    internal void AddPieceToDictionary(int row,int col,PieceType piece)
    {
        piecePositions.Add((row,col), piece);
    }
    //generate 2d array for board
    private void GenerateArray()
    {
        _chessBoard = new GameObject[8, 8];
        for (var i = 0; i < 8; i++)
        {
            for (var j = 0; j < 8; j++)
            {
                _chessBoard[i, j] = _rowsArray[i].transform.GetChild(j).gameObject;
            }
        }
    }

    //tries to get tile at given row col
    internal GameObject GetTile(int i, int j)
    {
        try
        {
            return _chessBoard[i, j];
        }
        catch (Exception)
        {
            Debug.LogError("Invalid row or column.");
            return null;
        }
    }

    //Instantiate Green Circle at given row col 
    internal void Highlight(int row, int col)
    {
        var tile = GetTile(row, col).transform;
        if (tile == null)
        {
            Debug.LogError("Invalid row or column.");
            return;
        }

        Instantiate(_highlightPrefab, tile.transform.position, Quaternion.identity, tile.transform);
    }

    //Instantiate Red Circle at given row col 
    internal void HighlightEnemy(int row,int col)
    {
        var tile = GetTile(row,col).transform;
        if (tile == null)
        {
            Debug.LogError("Invalid row or column.");
            return;
        }
        Instantiate(_enemyHighlightPrefab, tile.transform.position, Quaternion.identity, tile.transform);
    }

    //Destroys all highlights 
    internal void ClearHighlights()
    {
        for (var i = 0; i < 8; i++)
        {
            for (var j = 0; j < 8; j++)
            {
                var tile = GetTile(i, j);
                if (tile.transform.childCount <= 0) continue;
                foreach (Transform childTransform in tile.transform)
                {
                    Destroy(childTransform.gameObject);
                }
            }
        }
    }
}