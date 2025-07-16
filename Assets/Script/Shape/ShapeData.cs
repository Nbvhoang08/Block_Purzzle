using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ShapeData", menuName = "ScriptableObjects/ShapeData", order = 1)]
[System.Serializable]
public class ShapeData : ScriptableObject
{
    [System.Serializable]
    public class Row 
    {
        public bool[] column;
        private int _size;
        public Row()
        {
            
        }
        public Row(int size)
        {
            createRow(size);
        }
        public void createRow(int size)
        {
            _size = size;
            column = new bool[size];
            clearRow();
        }
        public void clearRow()
        {
            for (int i = 0; i < _size; i++)
            {
                column[i] = false;
            }
        }
    }

    public int columns = 0;
    public int rows = 0;
    public Row[] board;
    public void Clear() 
    {
        for (int i = 0; i < rows; i++)
        {
            board[i].clearRow();
        }
    }

    public void CreateNewBoard() 
    {
        board = new Row[rows];
        for (int i = 0; i < rows; i++)
        {
            board[i] = new Row(columns);
        }
    }
}
