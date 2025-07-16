using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using TMPro;
public class Shape : MonoBehaviour 
{
    public GameObject squareShape;
    public Vector3 shapeSelectedScale;
    [HideInInspector]
    public ShapeData CurrentShapeData;
    public int totalSquares { get; set; }

    private List<GameObject> _currentShape = new List<GameObject>();
    private Vector3 _originalScale;
    [SerializeField] private Vector3 _originalPosition;
    private bool _isDragging;
    [SerializeField]private Vector3 _offset;
    private bool _shapeActive = true;
    public Sprite[] sprite;
    public Sprite currentSpriteSelected;
    public void Awake()
    {
        _originalScale = transform.localScale;
        _originalPosition = transform.localPosition;
        _shapeActive = true;
    }

    private void OnEnable()
    {
        GameEvents.OnMoveShapeToStartPosition += MoveToStartPosition;
        GameEvents.SetShapeInactive += SetShapeInactive;
    }

    private void OnDisable()
    {
        GameEvents.OnMoveShapeToStartPosition -= MoveToStartPosition;
        GameEvents.SetShapeInactive -= SetShapeInactive;
     
    }
    void Update()
    {
        if(GameManager.Instance.gameState != GameState.Playing)
            return; 
#if UNITY_EDITOR || UNITY_STANDALONE
        HandleMouse();
#elif UNITY_ANDROID || UNITY_IOS
        HandleTouch();
#endif
    }

    void HandleMouse()
    {
        if (Input.mousePosition.x <= 0 || Input.mousePosition.y <= 0)
            return;
        Vector3 screenPos = Input.mousePosition;
        screenPos.z = Mathf.Abs(Camera.main.transform.position.z); // thường là 10 trong camera 2D
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(screenPos);
        mousePos.z = 0; 


        if (Input.GetMouseButtonDown(0))
        {
            if (IsPointerOverObject(mousePos))
            {
                OnBeginDrag();
                _offset = transform.position - mousePos;
                _isDragging = true;
            }
        }
        else if (Input.GetMouseButton(0) && _isDragging)
        {
            OnDrag(mousePos + _offset);
        }
        else if (Input.GetMouseButtonUp(0) && _isDragging)
        {
            _isDragging = false;
            OnEndDrag();
        }
    }

    void HandleTouch()
    {
        if (Input.touchCount == 0)
            return;

        Touch touch = Input.GetTouch(0);
        Vector3 touchPos = Camera.main.ScreenToWorldPoint(touch.position);
        touchPos.z = 0;

        switch (touch.phase)
        {
            case TouchPhase.Began:
                if (IsPointerOverObject(touchPos))
                {
                    OnBeginDrag();
                    _offset = transform.position - touchPos;
                    _isDragging = true;
                }
                break;

            case TouchPhase.Moved:
            case TouchPhase.Stationary:
                if (_isDragging)
                {
                    OnDrag(touchPos + _offset);
                }
                break;

            case TouchPhase.Ended:
            case TouchPhase.Canceled:
                if (_isDragging)
                {
                    _isDragging = false;
                    OnEndDrag();
                }
                break;
        }
    }

    bool IsPointerOverObject(Vector3 pointerWorldPos)
    {
        Collider2D hit = Physics2D.OverlapPoint(pointerWorldPos);
        return hit != null && (hit.transform == transform || hit.transform.IsChildOf(transform));
    }
    private void SetShapeInactive()
    {
        if(!IsOnStartPosition() && IsAnyOfShapeSquareActive()) 
        {
            foreach (var square in _currentShape)
            {
                square.gameObject.SetActive(false);
            }
        }
    }
    public void DeactivateShape()
    {
        if (_shapeActive)
        {
            foreach (var square in _currentShape)
            {
                square.GetComponent<ShapeSquare>().DeactivateShape();
            }
        }
        _shapeActive = false;
    }
    
    public void ActivateShape() 
    {
        
        if (!_shapeActive)
        {
            currentSpriteSelected = null;
            foreach (var square in _currentShape)
            {
                square.GetComponent<ShapeSquare>().ActivateShape();
            }
            _shapeActive = true;
        }
    }
    void OnBeginDrag()
    {
        DOTween.Kill(transform);
        transform.localScale = shapeSelectedScale;

    }

    void OnDrag(Vector2 newPosition)
    {
        transform.position = newPosition;
    }

    void OnEndDrag()
    {
        transform.localScale = _originalScale;
        GameEvents.CheckIfShapeCanBePlaceMethod();
    }

    public bool IsOnStartPosition()
    {
        return transform.localPosition == _originalPosition;
    }
    public bool IsAnyOfShapeSquareActive()
    {
        foreach (var square in _currentShape)
        {
            if (square.activeSelf)
            {
                return true;
            }
        }
        return false;
    }

    public void RequestNewShape(ShapeData shapeData)
    {
        MoveToStartPosition(); 
        createShape(shapeData);
    }

    public void createShape(ShapeData shapeData)
    {
        CurrentShapeData = shapeData;
        totalSquares = GetNumberOfSquare(shapeData);
        
        while (_currentShape.Count <= totalSquares)
        {
            var square = Instantiate(squareShape, transform) as GameObject;
            _currentShape.Add(square);
        }
        foreach (var square in _currentShape)
        {
            square.gameObject.transform.position = Vector3.zero;
            square.gameObject.SetActive(false);
        }
        var SquareTransform = squareShape.GetComponent<Transform>();
        var moveDistance = new Vector2(1.45f, 1.45f);
        int currentIndex = 0;
        int randomIndex = Random.Range(0, sprite.Length);
        currentSpriteSelected = sprite[randomIndex];
        for (var row = 0; row < shapeData.rows; row++)
        {
            for (int column = 0; column < shapeData.columns; column++)
            {
                if (shapeData.board[row].column[column])
                {
                    var square = _currentShape[currentIndex];
                    square.gameObject.SetActive(true);
                    square.GetComponent<ShapeSquare>().SetSprite(sprite[randomIndex]);
                    currentSpriteSelected = sprite[randomIndex];
                    var xPosition = GetXPositionForShapeSquare(shapeData, column, moveDistance);
                    var yPosition = GetYPositionForShapeSquare(shapeData, row, moveDistance);
                    square.transform.localPosition = new Vector3(xPosition, yPosition, 0);
                    currentIndex++;
                }
            }
        }
        transform.localScale = Vector3.zero;
        DOVirtual.DelayedCall(0.2f, () =>
        {
            TweenHelper.DoLocalScale(transform, _originalScale, 1f, Ease.OutBack);
        });

        
    }

    private void MoveToStartPosition()
    {
        transform.DOLocalMove(_originalPosition, 0.2f)
            .OnComplete(() => transform.localPosition = _originalPosition);

    }
    private float GetYPositionForShapeSquare(ShapeData shapeData, int row, Vector2 moveDistance)
    {
        float onShiftY = 0;
        if (shapeData.rows > 1)
        {
            if (shapeData.rows % 2 != 0)
            {
                var middleSquareIndex = (shapeData.rows - 1) / 2;
                var multiplier = (shapeData.rows - 1) / 2;
                if (row < middleSquareIndex)
                {
                    onShiftY = moveDistance.y;
                    onShiftY *= multiplier;
                }
                else if (row > middleSquareIndex)
                {
                    onShiftY = moveDistance.y * -1;
                    onShiftY *= multiplier;
                }
            }
            else
            {
                var middleSquareIndex2 = (shapeData.rows == 2) ? 1 : (shapeData.rows / 2);
                var middleSquareIndex1 = (shapeData.rows == 2) ? 0 : shapeData.rows - 2;
                var multiplier = shapeData.rows / 2;
                if (row == middleSquareIndex1 || row == middleSquareIndex2)
                {
                    if (row == middleSquareIndex2)
                        onShiftY = (moveDistance.y / 2) * -1;
                    if (row == middleSquareIndex1)
                        onShiftY = moveDistance.y / 2;
                }
                if (row < middleSquareIndex1 && row < middleSquareIndex2)
                {
                    onShiftY = moveDistance.y;
                    onShiftY *= multiplier;
                }
                else if (row > middleSquareIndex1 && row > middleSquareIndex2)
                {
                    onShiftY = moveDistance.y * -1;
                    onShiftY *= multiplier;
                }
            }
        }
        return onShiftY;
    }

    private float GetXPositionForShapeSquare(ShapeData shapeData, int column, Vector2 MoveDistance)
    {
        float shiftOnX = 0;
        if (shapeData.columns > 1)
        {
            if (shapeData.columns % 2 != 0)
            {
                var middleSquareIndex = (shapeData.columns - 1) / 2;
                var muiltiplier = (shapeData.columns - 1) / 2;
                if (column < middleSquareIndex)
                {
                    shiftOnX = MoveDistance.x * -1;
                    shiftOnX *= muiltiplier;
                }
                else if (column > middleSquareIndex)
                {
                    shiftOnX = MoveDistance.x;
                    shiftOnX *= muiltiplier;
                }
            }
            else
            {
                var middleSquareIndex2 = (shapeData.columns == 2) ? 1 : (shapeData.columns / 2);
                var middleSquareIndex1 = (shapeData.columns == 2) ? 0 : shapeData.columns - 1;
                var multiplier = shapeData.columns / 2;
                if (column == middleSquareIndex1 || column == middleSquareIndex2)
                {
                    if (column == middleSquareIndex2)
                        shiftOnX = MoveDistance.x / 2;
                    if (column == middleSquareIndex1)
                        shiftOnX = (MoveDistance.x / 2) * -1;
                }
                if (column < middleSquareIndex1 && column < middleSquareIndex2)
                {
                    shiftOnX = MoveDistance.x * -1;
                    shiftOnX *= multiplier;
                }
                else if (column > middleSquareIndex1 && column > middleSquareIndex2)
                {
                    shiftOnX = MoveDistance.x;
                    shiftOnX *= multiplier;
                }
            }
        }
        return shiftOnX;
    }
    private int GetNumberOfSquare(ShapeData shapeData)
    {
        int number = 0;
        foreach (var rowData in shapeData.board)
        {
            foreach (var active in rowData.column)
            {
                if (active)
                {
                    number++;
                }
            }

        }
        return number;
    }

    
}
