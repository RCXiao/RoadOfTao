using UnityEngine;
using UnityEngine.UI;

public class HoverAndRotate : MonoBehaviour
{
    [Header("悬浮运动设置")]
    [SerializeField] float amplitude = 0.5f;   // 悬浮幅度
    [SerializeField] float frequency = 1f;     // 悬浮频率

    [Header("旋转设置")]
    [SerializeField] float rotationSpeed = 30f; // 旋转速度（度/秒）

    private Vector3 initialPosition;  // 初始位置锚点

    public Sprite imageAt90Degrees; // 90度时的图像
    public Sprite imageAtMinus90Degrees; // -90度时的图像
    public Image cardImage; // 卡牌图像

    void Start()
    {
        initialPosition = transform.position;
    }

    void Update()
    {
        // 垂直悬浮运动
        transform.position = initialPosition + new Vector3(0, amplitude * Mathf.Sin(Time.time * frequency), 0);

        // 持续绕Y轴旋转
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);

        if (transform.rotation.eulerAngles.y < 90 || transform.rotation.eulerAngles.y > 270)
        {
            GetComponent<Image>().sprite = imageAtMinus90Degrees;
            cardImage.gameObject.SetActive(true);
        }
        else
        {
            GetComponent<Image>().sprite = imageAt90Degrees;
            cardImage.gameObject.SetActive(false);
        }       
    }
}
