using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [Header(header: "��ͼ���ñ�")]
    public MapConfigSO mapConfig;

    [Header(header: "��ͼ����")]
    public MapLayoutSO mapLayout;

    [Header(header: "Ԥ����")]
    public Room roomPrefab;
    public LineRenderer linePrefab;

    private float screenHeight;
    private float screenWidth;
    private float columnWidth;

    private Vector3 generatePoint;

    [Header(header: "�߾�")]
    public float border;

    private List<Room> rooms = new();
    private List<LineRenderer> lines = new();

    public List<RoomDataSO> roomDataList = new();
    private Dictionary<RoomType, RoomDataSO> roomDataDict = new();

    public GameObject background;

    private void Awake()
    {
        // ��ȡ��Ļ�߶�
        screenHeight = Camera.main.orthographicSize * 10;
        // ������Ļ�߶Ⱥ���������ݺ�ȼ�����Ļ���
        screenWidth = screenHeight * Camera.main.aspect / 5;

        // ����ÿһ�з���Ŀ��
        columnWidth = (float)(screenHeight / (mapConfig.roomBlueprints.Count + 0.5))/10*9;

        // �����������б�ת��Ϊ�ֵ䣬�Ա���ٲ���
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
        // ����ǰһ�з����б�
        List<Room> previousRowRooms = new();

        // ����ÿ��������ͼ
        for (int row = 0; row < mapConfig.roomBlueprints.Count; row++)
        {
            var blueprint = mapConfig.roomBlueprints[row];
            // ������ͼ�е���С�����ֵ������ɷ�������
            var amount = UnityEngine.Random.Range(blueprint.min, blueprint.max);

            // ������ʼ��ȣ�ʹ�÷�������Ļ������ȷֲ�
            var startWidth = -screenWidth / 2 + screenWidth / (amount + 1);
            // �������ɵ��λ��
            generatePoint = new Vector3(startWidth, screenHeight / 2 - border + columnWidth * row - 33, 0);

            var newPosition = generatePoint;

            // ������ǰ�з����б�
            List<Room> currentRowRooms = new();

            // ���㷿��֮��ļ��
            var roomGapX = screenWidth / (amount + 1);

            // ѭ����ǰ�е����з����������ɷ���
            for (int i = 0; i < amount; i++)
            {
                // ������������yλ��
                newPosition.y = generatePoint.y + Random.Range(-border*3/4,border*3/4);
                // ������������xλ��
                newPosition.x = startWidth + roomGapX * i + Random.Range(-border,border);
                // ���ɷ���
                var room = Instantiate(roomPrefab, transform);
                // �����Ӷ���ľֲ�λ��
                room.transform.localPosition = newPosition;

                // ��ȡ����ķ�������
                RoomType newtype = GetWeightedRandomRoomType(mapConfig.roomBlueprints[row].roomConfigs);

                // ���ֻ�е�һ�з�����Խ���
                if (row == 0)
                    room.roomState = RoomState.Attainable;
                else
                    room.roomState = RoomState.Locked;

                // ���÷���ľ�������
                room.SetupRoom(row, i, GetRoomData(newtype));


                // �����ɵķ�����ӵ�rooms�б�
                rooms.Add(room);
                // �����ɵķ�����ӵ�currentRowRooms�б�
                currentRowRooms.Add(room);
            }

            // �жϵ�ǰ���Ƿ��ǵ�һ�У�������ǣ������ӵ���һ�еķ���
            if (previousRowRooms.Count > 0)
            {
                // ���������б��������
                CreateConnections(previousRowRooms, currentRowRooms);
            }
            // ����ǰһ�з����б�Ϊ��ǰ�з����б�
            previousRowRooms = currentRowRooms;
        }
        // �����ͼ
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

        return roomConfigs[0].roomType; // ���׷��ص�һ��
    }


    private void CreateConnections(List<Room> previousColumnRooms, List<Room> currentColumnRooms)
    {
        // ��X�������򣬼��ٽ��������
        previousColumnRooms.Sort((a, b) => a.transform.position.x.CompareTo(b.transform.position.x));
        currentColumnRooms.Sort((a, b) => a.transform.position.x.CompareTo(b.transform.position.x));

        HashSet<Room> connectedCurrentColumnRooms = new();
        List<(Room, Room)> connections = new(); // �洢�������ӣ����⽻��

        //�����������ߣ��޽��棬��̾��룩
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

        //ȷ����ǰ�����з���������һ��ǰ��
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

        //��Ӷ�������ӣ����⽻�棩
        System.Random random = new System.Random();
        foreach (var room in currentColumnRooms)
        {
            int extraConnections = random.Next(0, 2); //�������� 0-1 ������

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

    //�����������䣬���洢������Ϣ
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

    //Ѱ�Ҳ�����Ķ�������
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

    //�ж������߶��Ƿ��ཻ
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

    //���㷽��
    private float Direction(Vector2 a, Vector2 b, Vector2 c)
    {
        return (c.x - a.x) * (b.y - a.y) - (c.y - a.y) * (b.x - a.x);
    }



    [ContextMenu(itemName: "ReGenerateRoom")]
    public void ReGenerateRoom()
    {
        // �������������ɵķ���
        foreach (var room in rooms)
        {
            Destroy(room.gameObject);
        }

        // �������������ɵ�����
        foreach (var line in lines)
        {
            Destroy(line.gameObject);
        }

        // ���rooms�б�
        rooms.Clear();
        // ���lines�б�
        lines.Clear();

        // �������ɵ�ͼ
        CreateMap();
    }

    private RoomDataSO GetRoomData(RoomType roomType)
    {
        // ���ݷ������ʹ��ֵ��л�ȡ��Ӧ��RoomDataSO
        return roomDataDict[roomType];
    }

    private RoomType GetRandomRoomType(RoomType flags)
    {
        // ��RoomTypeö��ֵת��Ϊ�ַ�������
        string[] options = flags.ToString().Split(',');

        // ���ѡ��һ���ַ�����Ϊ��������
        string randomOption = options[UnityEngine.Random.Range(0, options.Length)];

        // ���ַ���ת����RoomTypeö��ֵ
        RoomType roomType = (RoomType)System.Enum.Parse(typeof(RoomType), randomOption);

        // �������ѡ��ķ�������
        return roomType;
    }

    private void SaveMap()
    {
        mapLayout.mapRoomDataList = new();

        // ��������ɷ���
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
        // �������������
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
        // ��ȡ�������ݲ����ɷ���
        for (int i = 0; i < mapLayout.mapRoomDataList.Count; i++)
        {
            var newPos = new Vector3(mapLayout.mapRoomDataList[i].posX, mapLayout.mapRoomDataList[i].posY, 0);
            var newRoom = Instantiate(roomPrefab,transform);

            // �����Ӷ���ľֲ�λ��
            newRoom.transform.position = newPos;

            newRoom.roomState = mapLayout.mapRoomDataList[i].roomState;
            newRoom.SetupRoom(mapLayout.mapRoomDataList[i].colum, mapLayout.mapRoomDataList[i].line, mapLayout.mapRoomDataList[i].roomData);
            newRoom.linkTo = mapLayout.mapRoomDataList[i].linkTo;
            rooms.Add(newRoom);
        }

        // ��ȡ�������ݲ���������
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
