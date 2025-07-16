using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    [SerializeField] private List<UICanvas> uiCanvases;
    public Transform _effects;
    /*      [SerializeField] private Transform parent; */
    private bool isPaused = false;
    public void Start()
    {
        InitializeUICanvases();
    }
    private void InitializeUICanvases()
    {
        foreach (var canvas in uiCanvases)
        {
            canvas.gameObject.SetActive(false);
        }
    }
    public T OpenUI<T>() where T : UICanvas
    {
        T canvas = GetUI<T>();
        if (canvas != null)
        {
            canvas.Setup();
            canvas.Open();
        }

        return canvas;
    }
    public T OpenUI<T>(Transform customParent) where T : UICanvas
    {
        T canvas = GetUI<T>();
        if (canvas != null)
        {
            /* canvas.transform.SetParent(customParent, false);*/
            canvas.Setup();
            canvas.Open();
        }

        return canvas;
    }

    public void CloseUI<T>(float time) where T : UICanvas
    {
        T canvas = GetUI<T>();
        if (canvas != null)
        {
            canvas.Close(time);
        }
    }

    public void CloseUIDirectly<T>() where T : UICanvas
    {
        T canvas = GetUI<T>();
        if (canvas != null)
        {
            canvas.CloseDirectly();
        }
    }

    public bool IsUIOpened<T>() where T : UICanvas
    {
        T canvas = GetUI<T>();
        return canvas != null && canvas.gameObject.activeSelf;
    }

    // Lấy một UI cụ thể từ danh sách
    public T GetUI<T>() where T : UICanvas
    {
        return uiCanvases.Find(c => c is T) as T;
    }

    // Kích hoạt một UI cụ thể
    public void ActiveUI<T>() where T : UICanvas
    {
        T canvas = GetUI<T>();
        if (canvas != null)
        {
            canvas.gameObject.SetActive(true);
        }
    }



    // Đóng tất cả các UI đang mở
    public void CloseAll()
    {
        foreach (var canvas in uiCanvases)
        {
            if (canvas.gameObject.activeSelf)
            {
                canvas.Close(0);
            }
        }
    }

    // Tạm dừng hoặc tiếp tục game
    public void PauseGame()
    {
        isPaused = !isPaused;
        Debug.Log(isPaused);
        if (isPaused)
        {
            Time.timeScale = 0;
        }
        else
        {
            ResumeGame();
        }
    }

    // Tiếp tục game sau khi tạm dừng
    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1;
    }


    //public void ShowText(string content)
    //{
    //    TextEffect effect = Pool.Instance.TextEffect as TextEffect;
    //    effect.transform.SetParent(_effects);
    //    effect.OnShow(new Vector3(0f, 300f, 0f), content);
    //}

    public void QuitGame()
    {
#if UNITY_EDITOR
        // Trong môi trường phát triển (Unity Editor)
        EditorApplication.isPlaying = false;
#else
        // Trong ứng dụng đã build
         Application.Quit();
#endif
    }
}