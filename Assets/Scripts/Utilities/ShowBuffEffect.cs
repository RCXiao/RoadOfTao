using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

[RequireComponent(typeof(EventTrigger))]
public class ShowBUffEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("�������")]
    [SerializeField] GameObject targetObject;           // ��Ҫ���������
    [SerializeField] TextMeshProUGUI infoText;          // �ı����

    [Header("������������")]
    [SerializeField]
    [TextArea] string hoverText = "Item Info";    // ��ͣʱ��ʾ������

    void Start()
    {
        // ��ʼ��״̬
        if (targetObject != null) targetObject.SetActive(false);
        if (infoText != null) infoText.alpha = 0;
    }

    // ������ʱ����
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (targetObject != null)
            targetObject.SetActive(true);

            infoText.text = hoverText;
            infoText.alpha = 1;
        
    }

    // ����뿪ʱ����
    public void OnPointerExit(PointerEventData eventData)
    {
        if (targetObject != null)
            targetObject.SetActive(false);
    }
}
