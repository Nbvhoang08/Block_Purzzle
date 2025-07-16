using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGScaler : MonoBehaviour
{
    private SpriteRenderer backgroundSprite;
    private Camera mainCamera;

    void Start()
    {
        // Lấy component SpriteRenderer của background
        backgroundSprite = GetComponent<SpriteRenderer>();

        // Lấy reference đến main camera
        mainCamera = Camera.main;

        // Đảm bảo các component cần thiết tồn tại
        if (backgroundSprite == null || mainCamera == null)
        {
            Debug.LogError("Missing required components!");
            return;
        }

        // Thực hiện scale lần đầu khi game bắt đầu
        ScaleBackground();
    }

    void Update()
    {
        // Liên tục kiểm tra và scale background
        ScaleBackground();
    }

    void ScaleBackground()
    {
        // Tính toán kích thước viewport
        float screenHeight = mainCamera.orthographicSize * 2;
        float screenWidth = screenHeight * mainCamera.aspect;

        // Lấy kích thước thực của sprite
        float spriteWidth = backgroundSprite.sprite.bounds.size.x;
        float spriteHeight = backgroundSprite.sprite.bounds.size.y;

        // Tính tỉ lệ scale cần thiết
        float scaleX = screenWidth / spriteWidth;
        float scaleY = screenHeight / spriteHeight;

        // Chọn scale lớn hơn để đảm bảo background luôn phủ đầy màn hình
        //float scale = Mathf.Max(scaleX, scaleY);

        // Áp dụng scale mới
        transform.localScale = new Vector3(scaleX, scaleY, 1);
    }
}