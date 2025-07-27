using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class FadePanel : MonoBehaviour
{
    public Image background;

    private void Awake()
    {

    }

    public void FadeIn(float duration)
    {
        DOVirtual.Float(0, 1, duration, value =>
        {
            background.color =new Color(0, 0, 0, value);
        }).SetEase(Ease.OutCirc);
    }

    public void FadeOut(float duration)
    {
        DOVirtual.Float(1, 0, duration, value =>
        {
            background.color = new Color(0, 0, 0, value);
        }).SetEase(Ease.InCirc);
    }
}
