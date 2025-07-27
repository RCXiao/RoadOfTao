using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

[RequireComponent(typeof(EventTrigger))]
public class ShowBUffEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("对象控制")]
    [SerializeField] GameObject targetObject;           // 需要激活的物体
    [SerializeField] TextMeshProUGUI infoText;          // 文本组件

    [Header("文字内容配置")]
    [SerializeField]
    [TextArea] string hoverText = "Item Info";    // 悬停时显示的文字

    void Start()
    {
        // 初始化状态
        if (targetObject != null) targetObject.SetActive(false);
        if (infoText != null) infoText.alpha = 0;
    }

    // 鼠标进入时触发
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (targetObject != null)
            targetObject.SetActive(true);

            infoText.text = hoverText;
            infoText.alpha = 1;
        
    }

    // 鼠标离开时触发
    public void OnPointerExit(PointerEventData eventData)
    {
        if (targetObject != null)
            targetObject.SetActive(false);
    }
}
