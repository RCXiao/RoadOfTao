using UnityEditor.SceneManagement;
using UnityEngine;

public class DragSpriteVertical : MonoBehaviour
{
    private bool isDragging = false;
    private Vector3 offset;
    private float minY, maxY;
    public GameObject background;
    public MapLayoutSO mapLayout;
    void Start()
    {
        // �����϶���Χ
        minY = -42f; // ��СYֵ
        maxY = 0f;  // ���Yֵ
    }

    void Update()
    {
        if (isDragging)
        {
            // ��ȡ���λ�õ���������
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0; // ����Z�᲻��

            // ����Ŀ��λ��
            Vector3 targetPosition = new Vector3(background.transform.position.x, mousePosition.y + offset.y, 0);

            // �����϶���Χ
            targetPosition.y = Mathf.Clamp(targetPosition.y, minY, maxY);

            // ����Spriteλ��
            mapLayout.mapV3 = targetPosition;
            background.transform.position = targetPosition;
        }
    }

    void OnMouseDown()
    {
        // ��ʼ�϶�
        isDragging = true;

        // ���������λ����Sprite���ĵ�ƫ����
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        offset = background.transform.position - mousePosition; 
    }

    void OnMouseUp()
    {
        // �����϶�
        isDragging = false;

    }
}