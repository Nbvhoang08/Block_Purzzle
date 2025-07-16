using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseUI : UICanvas
{
    public Animator animator;
    public override void Open()
    {
        base.Open();
        OnOpenUI();
    }
    public void OnOpenUI()
    {
        animator.SetTrigger("Open");
    }
    public override void Close(float time)
    {
        base.Close(time);
        OnCloseUI();
    }
    public void OnCloseUI()
    {
        animator.SetTrigger("Close");

    }
    public void soundBtn()
    {

    }
    public void resumeBtn()
    {
        GameManager.Instance.gameState = GameState.Playing;
        Close(0.5f);
    }
   

    public void RestartBtn() 
    {
        GameManager.Instance.RestartGame();
        Close(0.4f);
    }
    public void QuitBtn()
    {
        UIManager.Instance.QuitGame();
    }
}
