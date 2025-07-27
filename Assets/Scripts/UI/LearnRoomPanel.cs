using System;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class LearnRoomPanel : MonoBehaviour
{
    public Button backMapBtn;
    public Button getCardBtn;
    public ObjectEventSO loadMapEvent;
    public GameObject PickCardPanel;

    private void OnEnable()
    {
        backMapBtn.onClick.AddListener(BackToMap);
        getCardBtn.onClick.AddListener(GetCard);
        getCardBtn.transform.gameObject.SetActive(true);
    }
    
    private void BackToMap()
    {
        loadMapEvent.RaiseEvent(null, this);
    }

    private void GetCard()
    {
        getCardBtn.transform.gameObject.SetActive(false);
        PickCardPanel.gameObject.SetActive(true);

    }
}
