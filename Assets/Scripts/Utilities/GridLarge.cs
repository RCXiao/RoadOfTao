using UnityEngine;
using UnityEngine.UI;

public class GridLarge : MonoBehaviour
{
    public ScrollRect scrollRect;
    private RectTransform rectTransform;
    public int theY;

    private void OnEnable()
    {
        rectTransform = GetComponent<RectTransform>();

        // 将ScrollRect滚动到最上方
        scrollRect.verticalNormalizedPosition = 1f;
    }
    private void Update()
    {
        if (rectTransform.rect.height < theY)
        {   
            //Debug.Log(rectTransform.rect.height);
            transform.GetComponent<ContentSizeFitter>().enabled = false;
            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, theY);
        }
    }

    private void OnDisable()
    {
        transform.GetComponent<ContentSizeFitter>().enabled = true;
    }

}
