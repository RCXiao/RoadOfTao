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

        EditorGUILayout.LabelField("����������" + GetListeners().Count.ToString());

        foreach (var listener in GetListeners())
        {
            EditorGUILayout.LabelField(listener.ToString());//��ʾ������������
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
