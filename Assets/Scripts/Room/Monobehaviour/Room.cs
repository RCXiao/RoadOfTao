using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public int column;//纵
    public int line;//横
    private SpriteRenderer spriteRenderer;
    public RoomDataSO roomData;
    public RoomState roomState;
    public List<Vector2Int> linkTo = new();
    public List<Vector2Int> beLinkBy = new();

    [Header("广播")]
    public ObjectEventSO loadRoomEvent;

    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void OnMouseDown()
    {
        // 处理房间点击事件
        Debug.Log("Clicked on Room: " + roomData.roomType);

        if (roomState == RoomState.Attainable)
        {
            loadRoomEvent.RaiseEvent(this, this);
        }
    }

    //外部创建房间时调用配置房间
    public void SetupRoom(int column, int line, RoomDataSO roomData)
    {
        this.column = column;
        this.line = line;
        this.roomData = roomData;

        spriteRenderer.sprite = roomData.roomIcon;

        spriteRenderer.color = roomState switch
        {
            RoomState.Attainable => new Color(1f, 1f, 1f, 1f),
            RoomState.Visited => new Color(0.5f, 0.8f, 0.5f, 0.5f),
            RoomState.Locked => new Color(0.5f, 0.5f, 0.5f, 1f),
            _ => throw new System.NotImplementedException(),
        };
    }
}
