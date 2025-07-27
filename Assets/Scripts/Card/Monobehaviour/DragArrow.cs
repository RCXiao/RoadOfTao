using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class DragArrow : MonoBehaviour
{
    private LineRenderer lineRenderer;
    public int pointsCount; //���ӵ��������ü�ͷ��ƽ��
    public float baseArc; //���������̶�
    public float maxArc; //��������̶�

    private Vector3 mousePos;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = pointsCount;
    }

    private void Update()
    {
        mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10f));
        DrawArrow();
    }

    private void DrawArrow()
    {
        Vector3 start = transform.position;
        Vector3 direction = mousePos - start;
        float length = direction.magnitude;

        if (length < 0.1f) return;

        direction.Normalize();
        Vector3 perpendicular = new Vector3(-direction.y, direction.x, 0);

        //ƽ��������������
        float offsetDir = Mathf.Lerp(-1, 1, (direction.x + 1) / 2);

        //ƽ�����������̶�
        float t = Mathf.Clamp01((length - 0.5f) / 10.0f); //�ö̼�ͷ������С������ͷ����������
        float dynamicArc = Mathf.Lerp(baseArc, maxArc, Mathf.SmoothStep(0, 1, t)); //ƽ����ֵ������ͻ��

        //���㱴�������߿��Ƶ�
        Vector3 controlPoint = (start + mousePos) * 0.5f + perpendicular * offsetDir * dynamicArc;

        for (int i = 0; i < pointsCount; i++)
        {
            float tPoint = i / (float)(pointsCount - 1);
            lineRenderer.SetPosition(i, GetBezierPoint(tPoint, start, controlPoint, mousePos));
        }
    }

    private Vector3 GetBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        float u = 1 - t;
        return u * u * p0 + 2 * u * t * p1 + t * t * p2;
    }
}
