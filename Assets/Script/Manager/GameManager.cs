using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public GameState gameState = GameState.Starting ;
    public GameObject CountDownPanel;
    [SerializeField] TextMeshProUGUI countDown;
    public int currentScore = 0;
    private Tween gameOverTween;
    public int BestScore
    {
        get
        {
            return PlayerPrefs.GetInt("BestScore", 0);
        }
        set
        {
            PlayerPrefs.SetInt("BestScore", value);
            PlayerPrefs.Save();
        }
    }
    public override void Awake()
    {
        base.Awake();
    }
    public bool HaveNewRecord
    {
        get
        {
            return currentScore > BestScore;
        }
    }
    void Start()
    {
        gameState = GameState.Playing;
        currentScore = 0;
        CountDownPanel.gameObject.SetActive(false);
    }
    public void RestartGame() 
    {
        gameOverTween?.Kill();
        gameOverTween = DOTween.Sequence()
            .AppendInterval(0.5f)
            .AppendCallback(() =>
            {
                currentScore = 0;
                gameState = GameState.Playing;
                GameEvents.RestartGameAction?.Invoke();
            });
        CountDownPanel.gameObject.SetActive(false);
    }

    public void CoolDown() 
    {
        gameState = GameState.GameOver;
        CountDownPanel.SetActive(true);
        TweenHelper.DoCountdown(
            countDown,
            3,
            1f, 
            () =>
            {
                GameEvents.GameOver();
            }
        );
    }

    public void SaveRecord()
    {
        if (currentScore > BestScore)
        {
            BestScore = currentScore;
        }
    }
}
