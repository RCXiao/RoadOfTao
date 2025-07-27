using UnityEngine;
using UnityEngine.UI;

public class GameOverPanel : MonoBehaviour
{
    public Button backToMenuButton;
    public ObjectEventSO loadMenuEvent;

    private void OnEnable()
    {
        backToMenuButton.onClick.AddListener(BackToStart);
    }

    private void BackToStart()
    {
        loadMenuEvent.RaiseEvent(null,this);
    }
}
