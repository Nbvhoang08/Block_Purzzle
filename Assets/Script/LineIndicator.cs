using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineIndicator : MonoBehaviour
{
    public int[,] Line_data = new int[8, 8]
    {
        {0,1,2,3,4,5,6,7},
        {8,9,10,11,12,13,14,15},
        {16,17,18,19,20,21,22,23},
        {24,25,26,27,28,29,30,31},
        {32,33,34,35,36,37,38,39},
        {40,41,42,43,44,45,46,47},
        {48,49,50,51,52,53,54,55},
        {56,57,58,59,60,61,62,63}
    };
    
    [HideInInspector]
    public int[] ColumnIndexes = new int[8]
    {
        0,1,2,3,4,5,6,7
    };

    private (int, int) GetSquarePositon(int square_index)
    {
        int row = -1;
        int col = -1;
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (Line_data[i,j] == square_index)
                {
                   row = i;
                   col = j;
                }
            }
        }
        return (row, col);
    }
    public int[] GetVerticalLine(int square_index) 
    {
        int[] line = new int[8];
        var square_pos_col = GetSquarePositon(square_index).Item2;
        for (int i = 0; i < 8; i++)
        {
            line[i] = Line_data[i, square_pos_col];
        }
        return line;

    }
}
