using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class DragArrow : MonoBehaviour
{
    private LineRenderer lineRenderer;
    public int pointsCount; //增加点数可以让箭头更平滑
    public float baseArc; //基础弯曲程度
    public float maxArc; //最大弯曲程度

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

        //平滑控制弯曲方向
        float offsetDir = Mathf.Lerp(-1, 1, (direction.x + 1) / 2);

        //平滑调整弯曲程度
        float t = Mathf.Clamp01((length - 0.5f) / 10.0f); //让短箭头弯曲较小，长箭头弯曲更明显
        float dynamicArc = Mathf.Lerp(baseArc, maxArc, Mathf.SmoothStep(0, 1, t)); //平滑插值，避免突变

        //计算贝塞尔曲线控制点
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
