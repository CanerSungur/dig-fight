using UnityEngine;
using DG.Tweening;
using TMPro;

namespace ZestCore.Utility
{
    public static class DOTweenUtils
    {
        public static void ShakeTransform(Transform transform, float shakeMagnitude = 0.5f)
        {
            transform.DORewind();
            transform.DOShakeRotation(shakeMagnitude, shakeMagnitude);
            transform.DOShakeScale(shakeMagnitude, shakeMagnitude);
        }

        public static void ChangeTextColorForAWhile(TextMeshProUGUI text, Color defaultColor, Color changedColor, float duration = 1f)
        {
            text.color = changedColor;
            DOVirtual.Color(changedColor, defaultColor, duration, r => {
                text.color = r;
            });
        }
    }
}
