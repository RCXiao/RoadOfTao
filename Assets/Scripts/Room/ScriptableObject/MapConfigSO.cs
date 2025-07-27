using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "MapConfigSO", menuName = "Map/MapConfigSO")]

public class MapConfigSO : ScriptableObject
{
    public List<RoomBlueprint> roomBlueprints;
}

[System.Serializable]
public class RoomConfig
{
    public RoomType roomType;
    public int roomWeight;
}

[System.Serializable]
public class RoomBlueprint
{
    public int min, max;
    public List<RoomConfig> roomConfigs;

}


