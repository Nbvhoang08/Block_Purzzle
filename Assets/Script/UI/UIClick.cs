using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class UIButtonClick : MonoBehaviour, IPointerClickHandler
{
    [Header("Click Settings")]
    public float clickScale = 1.2f;
    public float duration = 0.2f;
    public Ease ease = Ease.OutQuad;
    [SerializeField] private bool IsChangeSpriteButton = false;
    private bool IsPositiveButton = true;
    [SerializeField] private Sprite PositiveSprite;
    [SerializeField] private Sprite NegativeSprite;
    private Image image ;
    private Vector3 _originalScale;
    private Tween _clickTween;

    void Start()
    {
        _originalScale = transform.localScale;
        if (IsChangeSpriteButton)
        {
            image = GetComponent<Image>();
            if (image != null)
            {
                image.sprite = IsPositiveButton ? PositiveSprite : NegativeSprite;
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // Kill tween trước đó (nếu có)
        _clickTween?.Kill();
        //GameManager.Instance.PlaySound("ButtonClick");
        // Sequence: thu nhỏ -> phóng to lại
        _clickTween = DOTween.Sequence()
            .Append(transform.DOScale(_originalScale * clickScale, duration).SetEase(Ease.InQuad))
            .Append(transform.DOScale(_originalScale, duration).SetEase(ease))
            .AppendCallback(() => ChangeSpriteImage()); 
    }
    public void ChangeSpriteImage()
    {
        if (!IsChangeSpriteButton) return;

        IsPositiveButton = !IsPositiveButton; // Chuyển đổi trạng thái nút

        if (image != null)
        {
            image.sprite = IsPositiveButton ? PositiveSprite : NegativeSprite;
        }
    }

}
