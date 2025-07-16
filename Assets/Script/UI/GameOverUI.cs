using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameOverUI : UICanvas
{
    public Animator animator;
    public TextMeshProUGUI scoreText;
    [SerializeField] private GameObject NewBest;
    [SerializeField] private GameObject WellDone;
    public override void Open()
    {
        base.Open();
        OnOpenUI();
    }

    public void OnOpenUI()
    {
        animator.SetTrigger("Open");
        UpdateScore();
    }
    public void OnCloseUI() 
    {
        animator.SetTrigger("Close");
        GameManager.Instance.SaveRecord();
        GameManager.Instance.RestartGame();
        UIManager.Instance.CloseUI<GameOverUI>(0.6f);
    }

    public void UpdateScore()
    {
        if (GameManager.Instance.HaveNewRecord) 
        {
            scoreText.text = $"Best: {GameManager.Instance.currentScore.ToString()}";
        }
        else 
        {
            scoreText.text = $"{GameManager.Instance.currentScore.ToString()}";
        }
        NewBest.SetActive(GameManager.Instance.HaveNewRecord);
        WellDone.SetActive(!GameManager.Instance.HaveNewRecord);

    }


}
