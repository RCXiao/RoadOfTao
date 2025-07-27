using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [Header(header: "地图配置表")]
    public MapConfigSO mapConfig;

    [Header(header: "地图布局")]
    public MapLayoutSO mapLayout;

    [Header(header: "预制体")]
    public Room roomPrefab;
    public LineRenderer linePrefab;

    private float screenHeight;
    private float screenWidth;
    private float columnWidth;

    private Vector3 generatePoint;

    [Header(header: "边距")]
    public float border;

    private List<Room> rooms = new();
    private List<LineRenderer> lines = new();

    public List<RoomDataSO> roomDataList = new();
    private Dictionary<RoomType, RoomDataSO> roomDataDict = new();

    public GameObject background;

    private void Awake()
    {
        // 获取屏幕高度
        screenHeight = Camera.main.orthographicSize * 10;
        // 根据屏幕高度和摄像机的纵横比计算屏幕宽度
        screenWidth = screenHeight * Camera.main.aspect / 5;

        // 计算每一列房间的宽度
        columnWidth = (float)(screenHeight / (mapConfig.roomBlueprints.Count + 0.5))/10*9;

        // 将房间数据列表转换为字典，以便快速查找
        foreach (var roomData in roomDataList)
        {
            roomDataDict.Add(roomData.roomType, roomData);
        }
    }

    private void OnEnable()
    {
        if (mapLayout.mapRoomDataList.Count > 0)
        {
            LoadMap();
        }
        else
        {
            CreateMap();
        }
    }

    public void CreateMap()
    {
        // 创建前一行房间列表
        List<Room> previousRowRooms = new();

        // 遍历每个房间蓝图
        for (int row = 0; row < mapConfig.roomBlueprints.Count; row++)
        {
            var blueprint = mapConfig.roomBlueprints[row];
            // 根据蓝图中的最小和最大值随机生成房间数量
            var amount = UnityEngine.Random.Range(blueprint.min, blueprint.max);

            // 计算起始宽度，使得房间在屏幕中央均匀分布
            var startWidth = -screenWidth / 2 + screenWidth / (amount + 1);
            // 设置生成点的位置
            generatePoint = new Vector3(startWidth, screenHeight / 2 - border + columnWidth * row - 33, 0);

            var newPosition = generatePoint;

            // 创建当前行房间列表
            List<Room> currentRowRooms = new();

            // 计算房间之间的间距
            var roomGapX = screenWidth / (amount + 1);

            // 循环当前行的所有房间数量生成房间
            for (int i = 0; i < amount; i++)
            {
                // 随机调整房间的y位置
                newPosition.y = generatePoint.y + Random.Range(-border*3/4,border*3/4);
                // 随机调整房间的x位置
                newPosition.x = startWidth + roomGapX * i + Random.Range(-border,border);
                // 生成房间
                var room = Instantiate(roomPrefab, transform);
                // 设置子对象的局部位置
                room.transform.localPosition = newPosition;

                // 获取随机的房间类型
                RoomType newtype = GetWeightedRandomRoomType(mapConfig.roomBlueprints[row].roomConfigs);

                // 最初只有第一行房间可以进入
                if (row == 0)
                    room.roomState = RoomState.Attainable;
                else
                    room.roomState = RoomState.Locked;

                // 设置房间的具体数据
                room.SetupRoom(row, i, GetRoomData(newtype));


                // 将生成的房间添加到rooms列表
                rooms.Add(room);
                // 将生成的房间添加到currentRowRooms列表
                currentRowRooms.Add(room);
            }

            // 判断当前行是否是第一行，如果不是，则连接到上一行的房间
            if (previousRowRooms.Count > 0)
            {
                // 创建两个列表房间的连线
                CreateConnections(previousRowRooms, currentRowRooms);
            }
            // 更新前一行房间列表为当前行房间列表
            previousRowRooms = currentRowRooms;
        }
        // 保存地图
        SaveMap();
    }

    private RoomType GetWeightedRandomRoomType(List<RoomConfig> roomConfigs)
    {
        int totalWeight = roomConfigs.Sum(config => config.roomWeight);
        int randomWeight = Random.Range(0, totalWeight);
        int currentWeight = 0;

        foreach (var config in roomConfigs)
        {
            currentWeight += config.roomWeight;
            if (randomWeight < currentWeight)
            {
                return config.roomType;
            }
        }

        return roomConfigs[0].roomType; // 兜底返回第一个
    }


    private void CreateConnections(List<Room> previousColumnRooms, List<Room> currentColumnRooms)
    {
        // 按X坐标排序，减少交叉可能性
        previousColumnRooms.Sort((a, b) => a.transform.position.x.CompareTo(b.transform.position.x));
        currentColumnRooms.Sort((a, b) => a.transform.position.x.CompareTo(b.transform.position.x));

        HashSet<Room> connectedCurrentColumnRooms = new();
        List<(Room, Room)> connections = new(); // 存储已有连接，避免交叉

        //建立基础连线（无交叉，最短距离）
        int j = 0;
        foreach (var room in previousColumnRooms)
        {
            while (j < currentColumnRooms.Count - 1 &&
                   Mathf.Abs(currentColumnRooms[j + 1].transform.position.x - room.transform.position.x) <
                   Mathf.Abs(currentColumnRooms[j].transform.position.x - room.transform.position.x))
            {
                j++;
            }

            ConnectRooms(room, currentColumnRooms[j], connections);
            connectedCurrentColumnRooms.Add(currentColumnRooms[j]);
        }

        //确保当前列所有房间至少有一个前驱
        j = 0;
        foreach (var room in currentColumnRooms)
        {
            if (!connectedCurrentColumnRooms.Contains(room))
            {
                while (j < previousColumnRooms.Count - 1 &&
                       Mathf.Abs(previousColumnRooms[j + 1].transform.position.x - room.transform.position.x) <
                       Mathf.Abs(previousColumnRooms[j].transform.position.x - room.transform.position.x))
                {
                    j++;
                }

                ConnectRooms(previousColumnRooms[j], room, connections);
            }
        }

        //添加额外的连接（避免交叉）
        System.Random random = new System.Random();
        foreach (var room in currentColumnRooms)
        {
            int extraConnections = random.Next(0, 2); //额外增加 0-1 条连接

            for (int k = 0; k < extraConnections; k++)
            {
                Room extraTarget = FindNonCrossingRoom(room, previousColumnRooms, connections);
                if (extraTarget != null)
                {
                    ConnectRooms(extraTarget, room, connections);
                }
            }
        }
    }

    //连接两个房间，并存储连接信息
    private void ConnectRooms(Room from, Room to, List<(Room, Room)> connections)
    {
        from.linkTo.Add(new(to.column, to.line));
        to.beLinkBy.Add(new(from.column, from.line));

        var line = Instantiate(linePrefab, transform);
        line.SetPosition(0, from.transform.localPosition);
        line.SetPosition(1, to.transform.localPosition);
        lines.Add(line);

        connections.Add((from, to));
    }

    //寻找不交叉的额外连接
    private Room FindNonCrossingRoom(Room from, List<Room> candidates, List<(Room, Room)> connections)
    {
        foreach (var candidate in candidates)
        {
            if (from == candidate) continue;

            bool hasConflict = connections.Any(c =>
                LinesIntersect(from.transform.position, candidate.transform.position,
                               c.Item1.transform.position, c.Item2.transform.position));

            if (!hasConflict)
            {
                return candidate;
            }
        }
        return null;
    }

    //判断两条线段是否相交
    private bool LinesIntersect(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4)
    {
        float d1 = Direction(p3, p4, p1);
        float d2 = Direction(p3, p4, p2);
        float d3 = Direction(p1, p2, p3);
        float d4 = Direction(p1, p2, p4);

        if (((d1 > 0 && d2 < 0) || (d1 < 0 && d2 > 0)) &&
            ((d3 > 0 && d4 < 0) || (d3 < 0 && d4 > 0)))
        {
            return true;
        }
        return false;
    }

    //计算方向
    private float Direction(Vector2 a, Vector2 b, Vector2 c)
    {
        return (c.x - a.x) * (b.y - a.y) - (c.y - a.y) * (b.x - a.x);
    }



    [ContextMenu(itemName: "ReGenerateRoom")]
    public void ReGenerateRoom()
    {
        // 销毁所有已生成的房间
        foreach (var room in rooms)
        {
            Destroy(room.gameObject);
        }

        // 销毁所有已生成的连线
        foreach (var line in lines)
        {
            Destroy(line.gameObject);
        }

        // 清空rooms列表
        rooms.Clear();
        // 清空lines列表
        lines.Clear();

        // 重新生成地图
        CreateMap();
    }

    private RoomDataSO GetRoomData(RoomType roomType)
    {
        // 根据房间类型从字典中获取对应的RoomDataSO
        return roomDataDict[roomType];
    }

    private RoomType GetRandomRoomType(RoomType flags)
    {
        // 将RoomType枚举值转换为字符串数组
        string[] options = flags.ToString().Split(',');

        // 随机选择一个字符串作为房间类型
        string randomOption = options[UnityEngine.Random.Range(0, options.Length)];

        // 将字符串转换回RoomType枚举值
        RoomType roomType = (RoomType)System.Enum.Parse(typeof(RoomType), randomOption);

        // 返回随机选择的房间类型
        return roomType;
    }

    private void SaveMap()
    {
        mapLayout.mapRoomDataList = new();

        // 添加已生成房间
        for (int i = 0; i < rooms.Count; i++)
        {
            var room = new MapRoomData()
            {
                posX = rooms[i].transform.localPosition.x,
                posY = rooms[i].transform.localPosition.y,
                colum = rooms[i].column,
                line = rooms[i].line,
                roomData = rooms[i].roomData,
                roomState = rooms[i].roomState,
                linkTo = rooms[i].linkTo
            };

            mapLayout.mapRoomDataList.Add(room);
        }

        mapLayout.linePositionList = new();
        // 添加已生成连线
        for (int i = 0; i < lines.Count; i++)
        {
            var line = new LinePosition()
            {
                startPos = new SerializeVector3(lines[i].GetPosition(0)),
                endPos = new SerializeVector3(lines[i].GetPosition(1))
            };

            mapLayout.linePositionList.Add(line);
        }
    }

    private void LoadMap()
    {
        // 读取房间数据并生成房间
        for (int i = 0; i < mapLayout.mapRoomDataList.Count; i++)
        {
            var newPos = new Vector3(mapLayout.mapRoomDataList[i].posX, mapLayout.mapRoomDataList[i].posY, 0);
            var newRoom = Instantiate(roomPrefab,transform);

            // 设置子对象的局部位置
            newRoom.transform.position = newPos;

            newRoom.roomState = mapLayout.mapRoomDataList[i].roomState;
            newRoom.SetupRoom(mapLayout.mapRoomDataList[i].colum, mapLayout.mapRoomDataList[i].line, mapLayout.mapRoomDataList[i].roomData);
            newRoom.linkTo = mapLayout.mapRoomDataList[i].linkTo;
            rooms.Add(newRoom);
        }

        // 读取连线数据并生成连线
        for (int i = 0; i < mapLayout.linePositionList.Count; i++)
        {
            var newLine = Instantiate(linePrefab, transform);
            newLine.SetPosition(0, mapLayout.linePositionList[i].startPos.ToVector3());
            newLine.SetPosition(1, mapLayout.linePositionList[i].endPos.ToVector3());

            lines.Add(newLine);
        }

        background.transform.position = mapLayout.mapV3;
    }
}
