using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TopBarUI : UICanvas
{
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI BestScore;
    private void OnEnable()
    {
        GameEvents.AddScore += UpdateScore;
        GameEvents.RestartGameAction += OnReset;
    }
    private void OnDisable()
    {
        GameEvents.AddScore -= UpdateScore;
        GameEvents.RestartGameAction -= OnReset;
    }

    public void Start()
    {
        UpdateScore(0);
        UpdateBestScore();
    }
    public void PauseBtn()
    {
        GameManager.Instance.gameState = GameState.Paused;
        Debug.Log(UIManager.Instance != null);
        UIManager.Instance.OpenUI<PauseUI>();
    }
    public void OnReset()
    {
        UpdateScore(0);
        UpdateBestScore();
    }


    public void UpdateBestScore() 
    {
        if (BestScore != null)
        {
            BestScore.text = GameManager.Instance.BestScore.ToString();
        }
    }

    public void UpdateScore(int score)
    {
        GameManager.Instance.currentScore += score;
        if (scoreText != null)
        {
            scoreText.text = GameManager.Instance.currentScore.ToString();
        }
    }
}
