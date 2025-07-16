using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeStorage : MonoBehaviour
{
    public List<ShapeData> shapeData;
    public List<Shape> shapeList;
    void Start()
    {
        foreach (var shape in shapeList)
        {
           var shapeIndex = Random.Range(0, shapeData.Count);
           shape.RequestNewShape(shapeData[shapeIndex]);
        }
    }

    private void OnDisable()
    {
        GameEvents.OnRequestNewShape -= RequestNewShape;
        GameEvents.RestartGameAction -= RequestNewShape;
    }

    private void OnEnable()
    {
        GameEvents.OnRequestNewShape += RequestNewShape;
        GameEvents.RestartGameAction += RequestNewShape;
    }

    private void RequestNewShape()
    {
        foreach (var shape in shapeList)
        {
           var shapeIndex = Random.Range(0, shapeData.Count);
           shape.RequestNewShape(shapeData[shapeIndex]);
        }
    }
    public Shape GetCurrentSelectedShape()
    {
        foreach (var shape in shapeList)
        {
            if (!shape.IsOnStartPosition() && shape.IsAnyOfShapeSquareActive())
            {
                return shape;
            }
        }
        return null;
    }
}
