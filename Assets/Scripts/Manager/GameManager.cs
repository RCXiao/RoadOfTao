using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public Player player;

    [Header("地图布局")]
    public MapLayoutSO mapLayout;
    public List<Enemy> aliveEnemyList = new List<Enemy>();

    [Header("事件广播")]
    public ObjectEventSO gameWinEvent;
    public ObjectEventSO gameOverEvent;
    public ObjectEventSO gameEndEvent;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    //更新房间,重新加载地图的事件监听函数
    public void UpdateMapLayoutData(object value)
    {
        var roomVector = (Vector2Int)value;

        if (mapLayout.mapRoomDataList.Count == 0)
            return;
        var currentRoom = mapLayout.mapRoomDataList.Find(r => r.colum == roomVector.x && r.line == roomVector.y);

        currentRoom.roomState = RoomState.Visited;

        var sameColunmRoom = mapLayout.mapRoomDataList.FindAll(r => r.colum == roomVector.x);

        foreach (var room in sameColunmRoom)
        {
            if (room.line != roomVector.y)
                room.roomState = RoomState.Locked;
        }

        foreach (var link in currentRoom.linkTo)
        {
            var linkedRoom = mapLayout.mapRoomDataList.Find(r => r.colum == link.x && r.line == link.y);
            linkedRoom.roomState = RoomState.Attainable;
        }

        aliveEnemyList.Clear();
    }

    public void OnRoomLoadedEvent(object obj)
    {
        var enemies = FindObjectsByType<Enemy>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (var enemy in enemies)
        {
            aliveEnemyList.Add(enemy);
        }
    }

    public void OnCharacterDeadEvent(object character)
    {
        Debug.Log(character);
        if (character is Player)
        {
            //发出失败的通知
            StartCoroutine(EventDelayAction(gameOverEvent));
        }

        if (character is Boss)
        {
            aliveEnemyList.Remove(character as Enemy);
            StartCoroutine(EventDelayAction(gameEndEvent));
        }

        if (character is Enemy && !(character is Boss))
        {
            aliveEnemyList.Remove(character as Enemy);

            if (aliveEnemyList.Count == 0)
            {
                //发出获胜的通知
                StartCoroutine(EventDelayAction(gameWinEvent));
                Debug.Log("游戏胜利");
            }
        }
    }

    IEnumerator EventDelayAction(ObjectEventSO eventSO)
    {
        yield return new WaitForSeconds(1.5f);
        eventSO.RaiseEvent(null, this);
    }

    public void OnNewGameEvent()
    {
        mapLayout.mapRoomDataList.Clear();
        mapLayout.linePositionList.Clear();
    }
}
