using System;

[Flags]

public enum RoomType
{
    //敌人房间：普通敌人，困难敌人，Boss敌人，特殊敌人，与人切磋，邪灵敌人
    Enemy_Normal = 1,
    Enemy_Hard = 2,
    Enemy_Boss = 4,
    Enemy_Special = 8,
    Enemy_Duel = 16,
    Enemy_Devil = 32,

    //事件房间：获取天命，获取道器，学习道法，进入道市，打坐休息（升级，遗忘），混沌房间（较高收益）
    Event_Fate = 64,
    Event_Jewelry = 128,
    Event_Learn = 256,
    Event_Shop = 512,
    Event_Rest = 1024,
    Event_Chaos = 2048,
    Event_Start = 4096,
}

public enum RoomState
{
    Locked,
    Visited,
    Attainable,
}

public enum CardType
{
    Self,
    Target,
    enemy,
    ALL
}

public enum EffectTargetType
{
    Self,
    Target,
    enemy,
    ALL
}