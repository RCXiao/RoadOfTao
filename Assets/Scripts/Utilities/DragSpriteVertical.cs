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
        // 设置拖动范围
        minY = -42f; // 最小Y值
        maxY = 0f;  // 最大Y值
    }

    void Update()
    {
        if (isDragging)
        {
            // 获取鼠标位置的世界坐标
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0; // 保持Z轴不变

            // 计算目标位置
            Vector3 targetPosition = new Vector3(background.transform.position.x, mousePosition.y + offset.y, 0);

            // 限制拖动范围
            targetPosition.y = Mathf.Clamp(targetPosition.y, minY, maxY);

            // 更新Sprite位置
            mapLayout.mapV3 = targetPosition;
            background.transform.position = targetPosition;
        }
    }

    void OnMouseDown()
    {
        // 开始拖动
        isDragging = true;

        // 计算鼠标点击位置与Sprite中心的偏移量
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        offset = background.transform.position - mousePosition; 
    }

    void OnMouseUp()
    {
        // 结束拖动
        isDragging = false;

    }
}