using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using DG.Tweening;
using System.Linq;
public class GridGenerator : MonoBehaviour
{
    public ShapeStorage shapeStorage;
    public int rows = 10;
    public int columns = 10;
    public float cellSize = 1f;

    public GameObject cellPrefab;
    public Transform gridParent;
    public List<GridSquare> Collection;
    [SerializeField] private LineIndicator _lineIndicator;
    private void OnEnable()
    {
        GameEvents.OnCheckIfShapeCanBePlace += CheckIfShapeCanBePlaced;
        GameEvents.RestartGameAction += SetUpGrid;
        GameEvents.GameOver += GameOver;
    }
    private void OnDisable()
    {
        GameEvents.OnCheckIfShapeCanBePlace -= CheckIfShapeCanBePlaced;
        GameEvents.RestartGameAction -= SetUpGrid;
        GameEvents.GameOver -= GameOver;
    }
    private void Start()
    {
        _lineIndicator = GetComponent<LineIndicator>();
        SetUpGrid();
    }

    public void CheckIfShapeCanBePlaced()
    {
        var squareIndex = new List<int>();
        foreach (var grid in Collection)
        {
            if (!grid.SquareOccupied && grid.Selected)
            {
                squareIndex.Add(grid.Index);
                grid.Selected = false;
            }
        }
        var currentSelectedShape = shapeStorage.GetCurrentSelectedShape();
        if (currentSelectedShape == null) return;
        if (currentSelectedShape.totalSquares == squareIndex.Count)
        {
            foreach (var index in squareIndex)
            {
                var gridSquare = Collection.Find(x => x.Index == index);
                if (gridSquare != null)
                {
                    gridSquare.PlaceShapeOnBoard(currentSelectedShape.currentSpriteSelected);
                }
            }
            var shapeLeft = 0;
            foreach (var shape in shapeStorage.shapeList)
            {
                if (shape.IsOnStartPosition() && shape.IsAnyOfShapeSquareActive())
                {
                    shapeLeft++;
                }
            }

            if (shapeLeft == 0)
            {
                GameEvents.RequestNewShapeMethod();
            }
            else
            {

                GameEvents.SetShapeInactive();

            }
            Debug.Log($"Shape left: {shapeLeft}");
            CheckIfCompletedLine();
        }
        else
        {
            GameEvents.MoveShapeToStartPositionMethod();
        }

    }
    void CheckIfCompletedLine()
    {
        List<int[]> Lines = new List<int[]>();
        // column
        foreach (int column in _lineIndicator.ColumnIndexes)
        {
            Lines.Add(_lineIndicator.GetVerticalLine(column));
        }
        // row
        for (int row = 0; row < 8; row++)
        {
            List<int> data = new List<int>(8);
            for (int index = 0; index < 8; index++)
            {
                data.Add(_lineIndicator.Line_data[row, index]);
            }
            Lines.Add(data.ToArray());
        }

        var completedLines = CheckIfSquareAreCompleted(Lines);
        if (completedLines > 2)
        {
            //TODO : Play Bonus Animation

        }
        //TODO : Add Score 
        var totalScore = 10 * completedLines;
        GameEvents.AddScore(totalScore);
        
    }

    private int CheckIfSquareAreCompleted(List<int[]> data)
    {
        List<int[]> completedLines = new List<int[]>();
        var LineCompleted = 0;
        foreach (var line in data)
        {
            bool isCompleted = true;
            foreach (var index in line)
            {
                var gridSquare = Collection.Find(x => x.Index == index);
                if (gridSquare == null || !gridSquare.SquareOccupied)
                {
                    isCompleted = false;
                }
            }
            if (isCompleted)
            {
                completedLines.Add(line);
            }
        }
        foreach (var line in completedLines)
        {
            bool completed = false;
            foreach (var index in line)
            {
                var gridSquare = Collection.Find(x => x.Index == index);
                if (gridSquare != null)
                {
                    completed = true;
                }
            }
            if (completed)
            {
                LineCompleted++;

            }
        }

        var allIndexes = completedLines.SelectMany(line => line).Distinct().ToList();

        TweenHelper.DoWaveScale(
            allIndexes,
            index => Collection.Find(cell => cell.Index == index).transform.Find("OccupiedSprite"),
            Vector3.one*1.25f,
            Vector3.zero,
            0.5f,
            0.1f,
            Vector3.zero,
            Ease.InOutBack
        )
        .OnComplete(() => 
        {
                foreach (var line in completedLines)
                {
                    foreach (var index in line)
                    {
                        var gridSquare = Collection.Find(x => x.Index == index);
                        if (gridSquare != null)
                        { 
                            gridSquare.DeActivateSquare();
                            gridSquare.ClearOccupied();
                        }
                    }
                   
                }
            CheckPlayerLost();
        }
        );

        
        return LineCompleted;
    }

    private void CheckPlayerLost()
    {
        var validShape = 0;
        for (var index = 0; index < shapeStorage.shapeList.Count; index++)
        {
            var isShapeActive = shapeStorage.shapeList[index].IsAnyOfShapeSquareActive();
            if (CheckIfShapeCanBePlaceOnGrid(shapeStorage.shapeList[index]) && isShapeActive)
            {
                shapeStorage.shapeList[index]?.ActivateShape();
                validShape++;
            }
        }
        if (validShape == 0)
        {
            GameManager.Instance.CoolDown();

        }
    }
    private bool CheckIfShapeCanBePlaceOnGrid(Shape currentShape)
    {
        var currentShapeSquares = currentShape.CurrentShapeData;
        var shapeColumn = currentShapeSquares.columns;
        var shapeRow = currentShapeSquares.rows;

        // All indexes of filled up Squares
        List<int> originalShapeFilledUpSquares = new List<int>();
        var squareIndex = 0;
        for (int row = 0; row < shapeRow; row++)
        {
            for (int col = 0; col < shapeColumn; col++)
            {
                if (currentShapeSquares.board[row].column[col])
                {
                    originalShapeFilledUpSquares.Add(squareIndex);
                }
                squareIndex++;
            }
        }
        if (currentShape.totalSquares != originalShapeFilledUpSquares.Count)
        {
            Debug.LogWarning("Shape squares count does not match the original shape filled up squares count.");
        }
        var squareList = GetAllSquareCombination(shapeColumn, shapeRow);
        bool canBePlace = false;

        foreach (var square in squareList)
        {
            bool isValid = true;
            foreach (var indexToCheck in originalShapeFilledUpSquares)
            {
                var comp = Collection.Find(x => x.Index == square[indexToCheck]);
                if (comp == null || comp.SquareOccupied)
                {
                    isValid = false;
                    break;
                }
            }
            if (isValid)
            {
                canBePlace = true;
            }
        }
        return canBePlace;
    }
    private List<int[]> GetAllSquareCombination(int columns, int rows)
    {
        var squareList = new List<int[]>();
        var LastColumnIndex = 0;
        var LastRowIndex = 0;
        int SafeIndex = 0;
        while (LastRowIndex + (rows - 1) < 8)
        {
            var rowData = new List<int>();
            for (int row = LastRowIndex; row < LastRowIndex + rows; row++)
            {
                for (int col = LastColumnIndex; col < LastColumnIndex + columns; col++)
                {
                    var squareIndex = _lineIndicator.Line_data[row, col];
                    rowData.Add(squareIndex);
                }
            }
            squareList.Add(rowData.ToArray());

            LastColumnIndex++;

            if (LastColumnIndex + (columns - 1) >= 8)
            {
                LastRowIndex++;
                LastColumnIndex = 0;

            }
            SafeIndex++;
            if (SafeIndex > 100)
            {
                Debug.LogWarning("Safe index exceeded, breaking to avoid infinite loop.");
                break;
            }
        }
        return squareList;
    }

    public void GameOver()
    {
        DOVirtual.DelayedCall(1f, () => {
            TweenHelper.DoWaveScale(
                Collection,
                x => x.transform,
                Vector3.one,
                Vector3.zero,
                1f,
                0.1f,
                Vector3.zero,
                Ease.InOutBack
            )
            .OnComplete(() => {
                UIManager.Instance.OpenUI<GameOverUI>();
            });
        });


        //UIManager.Instance.OpenUI<GameOverUI>();
    }
    public void SetUpGrid() 
    {
       
        TweenHelper.DoWaveScale(
            Collection,
            x => x.transform,
            Vector3.zero,
            Vector3.one,
            1f,
            0.1f,
            Vector3.zero,
            Ease.InOutBack
        );
    }

    public void GenerateGridInEditor()
    {
#if UNITY_EDITOR
        if (cellPrefab == null)
        {
            Debug.LogError("Cell prefab is missing.");
            return;
        }

        if (gridParent == null)
            gridParent = this.transform;
        Collection.Clear();
        // Xoá cái cũ nếu có
        while (gridParent.childCount > 0)
        {
            DestroyImmediate(gridParent.GetChild(0).gameObject);
            
        }

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                Vector3 pos = new Vector3(col * cellSize, -row * cellSize, 0f);
                GameObject cell = (GameObject)UnityEditor.PrefabUtility.InstantiatePrefab(cellPrefab, gridParent);
                cell.transform.localPosition = pos;
                cell.name = $"Cell_{row}_{col}";
                GridSquare grid = cell.GetComponent<GridSquare>();
                if (grid != null)
                {
                    grid.Index = row * columns + col; 
                    Collection.Add(grid);
                }
                else
                {
                    Debug.LogWarning($"Cell prefab does not have a Grid component: {cell.name}");
                }
            }
        }

        float width = columns * cellSize;
        float height = rows * cellSize;
        gridParent.localPosition = new Vector3(-width / 2f + cellSize / 2f, height / 2f - cellSize / 2f, 0f);
#endif
    }
}
