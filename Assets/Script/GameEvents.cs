using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameEvents
{
    public delegate void CheckIfShapeCanBePlace();
    public static event CheckIfShapeCanBePlace OnCheckIfShapeCanBePlace;
    public static void CheckIfShapeCanBePlaceMethod()
    {
        if (OnCheckIfShapeCanBePlace != null)
        {
            OnCheckIfShapeCanBePlace();
        }
    }
    public delegate void MoveShapeToStartPosition();
    public static event MoveShapeToStartPosition OnMoveShapeToStartPosition;
    public static void MoveShapeToStartPositionMethod()
    {
        if (OnMoveShapeToStartPosition != null)
        {
            OnMoveShapeToStartPosition();
        }
    }

    public delegate void RequestNewShape();
    public static event RequestNewShape OnRequestNewShape;
    public static void RequestNewShapeMethod()
    {
        if (OnRequestNewShape != null)
        {
            OnRequestNewShape();
        }
    }
    public static Action SetShapeInactive;
    public static Action<int> AddScore;
    public static Action GameOver;
    public static Action RestartGameAction;
}

public enum GameState
{
    Starting,
    Playing,
    Paused,
    GameOver
}


