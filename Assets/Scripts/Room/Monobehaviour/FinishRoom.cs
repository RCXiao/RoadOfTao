using UnityEngine;

public class FinishRoom : MonoBehaviour
{
    public ObjectEventSO loadMapEvent;

    private void OnMouseDown()
    {
        //���ص�ͼ
        loadMapEvent.RaiseEvent(null, this);
    }
}
