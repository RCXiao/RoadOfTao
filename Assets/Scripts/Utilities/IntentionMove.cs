using UnityEngine;
using UnityEngine.UI;

public class HoverAndRotate : MonoBehaviour
{
    [Header("�����˶�����")]
    [SerializeField] float amplitude = 0.5f;   // ��������
    [SerializeField] float frequency = 1f;     // ����Ƶ��

    [Header("��ת����")]
    [SerializeField] float rotationSpeed = 30f; // ��ת�ٶȣ���/�룩

    private Vector3 initialPosition;  // ��ʼλ��ê��

    public Sprite imageAt90Degrees; // 90��ʱ��ͼ��
    public Sprite imageAtMinus90Degrees; // -90��ʱ��ͼ��
    public Image cardImage; // ����ͼ��

    void Start()
    {
        initialPosition = transform.position;
    }

    void Update()
    {
        // ��ֱ�����˶�
        transform.position = initialPosition + new Vector3(0, amplitude * Mathf.Sin(Time.time * frequency), 0);

        // ������Y����ת
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
