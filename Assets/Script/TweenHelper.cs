using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;
using System;
using TMPro;

public static class TweenHelper
{
    /// <summary>
    /// Scale local của một Transform tới một giá trị nhất định trong khoảng thời gian, với easing tuỳ chọn.
    /// </summary>
    /// <param name="target">Transform cần tween</param>
    /// <param name="toScale">Vector3 giá trị scale mong muốn</param>
    /// <param name="duration">Thời gian tween</param>
    /// <param name="ease">Loại easing (mặc định: Ease.OutBack)</param>
    /// <param name="onComplete">Callback khi tween xong (tuỳ chọn)</param>
    public static Tween DoLocalScale(Transform target, Vector3 toScale, float duration, Ease ease = Ease.OutBack, TweenCallback onComplete = null)
    {
        Tween tween = target.DOScale(toScale, duration)
                             .SetEase(ease);

        if (onComplete != null)
            tween.OnComplete(onComplete);

        return tween;
    }


    /// <summary>
    /// Tạo hiệu ứng scale dạng sóng cho list bất kỳ, với cách truy xuất Transform linh hoạt.
    /// </summary>
    /// <typeparam name="T">Kiểu phần tử trong list (Cell, GameObject, v.v.)</typeparam>
    /// <param name="items">Danh sách đối tượng</param>
    /// <param name="getTransform">Hàm để lấy Transform từ mỗi phần tử</param>
    /// <param name="fromScale">Scale bắt đầu</param>
    /// <param name="toScale">Scale mong muốn</param>
    /// <param name="duration">Thời gian tween</param>
    /// <param name="delayFactor">Độ trễ tính theo khoảng cách</param>
    /// <param name="origin">Gốc lan sóng</param>
    /// <param name="ease">Easing</param>
    public static Sequence DoWaveScale<T>(
         List<T> items,
         Func<T, Transform> getTransform,
         Vector3 fromScale,
         Vector3 toScale,
         float duration,
         float delayFactor,
         Vector3 origin,
         Ease ease = Ease.OutBack
     )
    {
        var sequence = DOTween.Sequence();

        foreach (var item in items)
        {
            var t = getTransform(item);
            if (t == null) continue;

            t.localScale = fromScale;

            float delay = Vector3.Distance(t.position, origin) * delayFactor;

            Tween tween = t.DOScale(toScale, duration)
                           .SetEase(ease)
                           .SetDelay(delay);

            sequence.Insert(0, tween); // chèn vào sequence
        }

        return sequence;
    }


    /// <summary>
    /// Đếm ngược trên UI Text (hoặc TMP_Text) với hiệu ứng phóng to từng số.
    /// </summary>
    /// <param name="target">UI Text hoặc TMP_Text để hiển thị số</param>
    /// <param name="from">Số bắt đầu đếm ngược</param>
    /// <param name="interval">Thời gian giữa các số</param>
    /// <param name="onComplete">Callback khi kết thúc đếm ngược</param>
    public static void DoCountdown(TMP_Text target, int from, float interval, Action onComplete = null)
    {
        target.gameObject.SetActive(true);
        target.transform.localScale = Vector3.one;

        Sequence seq = DOTween.Sequence();

        for (int i = from; i >= 0; i--)
        {
            int currentValue = i;

            seq.AppendCallback(() =>
            {
                target.text = currentValue.ToString();
                target.transform.localScale = Vector3.one * 0.5f;
                target.transform.DOScale(1.5f, 0.4f).SetEase(Ease.OutBack);
            });

            seq.AppendInterval(interval);
        }

        seq.AppendCallback(() =>
        {
            onComplete?.Invoke();
            //target.gameObject.SetActive(false);
        });
    }

}

