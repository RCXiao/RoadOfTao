using System;
using UnityEngine;
using UnityEngine.UI;

public class MenuPanel : MonoBehaviour
{
    public Button newGameButton, quitButton;
    public ObjectEventSO newGameEvent;

    private void OnEnable()
    {
        newGameButton.onClick.AddListener(OnNewGameButtonClicked);
        quitButton.onClick.AddListener(OnQuitButtonClicked);
    }

    private void OnNewGameButtonClicked()
    {
        newGameEvent.RaiseEvent(null, this);
    }

    private void OnQuitButtonClicked()
    {
        Application.Quit();
    }
}
