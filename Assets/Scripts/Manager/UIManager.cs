using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Header("Ãæ°å")]
    public GameObject gamePlayPanel;
    public GameObject gameOverPanel;
    public GameObject gameWinPanel;
    public GameObject showCardPanel;
    public GameObject gameEndPanel;

    public void OnLoadRoomEvent(object data)
    {
        Room currentRoom = (Room)data;

        switch (currentRoom.roomData.roomType)
        {
            case RoomType.Enemy_Normal:
            case RoomType.Enemy_Hard:
            case RoomType.Enemy_Boss:
            case RoomType.Enemy_Special:
            case RoomType.Enemy_Duel:
            case RoomType.Enemy_Devil:
                gamePlayPanel.SetActive(true);
                break;
            case RoomType.Event_Fate:
                break;
            case RoomType.Event_Jewelry:
                break;
            case RoomType.Event_Learn:
                break;
            case RoomType.Event_Shop:
                break;
            case RoomType.Event_Rest:
                break;
            case RoomType.Event_Chaos:
                break;
            case RoomType.Event_Start:
                break;
            default:
                break;
        }
    }

    public void HideAllPanels()
    {
        gamePlayPanel.SetActive(false);
        gameOverPanel.SetActive(false);
        gameWinPanel.SetActive(false);
        showCardPanel.SetActive(false);
    }

    public void OnGameOverEvent()
    {
        gamePlayPanel.SetActive(false);
        gameOverPanel.SetActive(true);
    }

    public void OnGameWinEvent()
    {
        gamePlayPanel.SetActive(false);
        gameWinPanel.SetActive(true);
    }

    public void OnGameEndEvent()
    {
        gamePlayPanel.SetActive(false);
        gameEndPanel.SetActive(true);
    }
}
