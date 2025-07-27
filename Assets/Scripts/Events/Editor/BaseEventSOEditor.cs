using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
[CustomEditor(typeof(BaseEventSO<>))]
public class BaseEventOSEditor<T> : Editor
{
    private BaseEventSO<T> baseEventSO;

    private void OnEnable()
    {
        if (baseEventSO == null)
            baseEventSO = (BaseEventSO<T>)target;
    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUILayout.LabelField("订阅数量：" + GetListeners().Count.ToString());

        foreach (var listener in GetListeners())
        {
            EditorGUILayout.LabelField(listener.ToString());//显示监听器的名称
        }
    }

    private List<MonoBehaviour> GetListeners()
    {
        List<MonoBehaviour> listeners = new();

        if (baseEventSO == null || baseEventSO.OnEventRaised == null) return listeners;

        var subscribers = baseEventSO.OnEventRaised.GetInvocationList();
        foreach (var subscriber in subscribers)
        {
            var obj = subscriber.Target as MonoBehaviour;
            if (!listeners.Contains(obj))
            {
                listeners.Add(obj);
            }
        }
        return listeners;
    }
}
