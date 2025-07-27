using System;

[Flags]

public enum RoomType
{
    //���˷��䣺��ͨ���ˣ����ѵ��ˣ�Boss���ˣ�������ˣ������д裬а�����
    Enemy_Normal = 1,
    Enemy_Hard = 2,
    Enemy_Boss = 4,
    Enemy_Special = 8,
    Enemy_Duel = 16,
    Enemy_Devil = 32,

    //�¼����䣺��ȡ��������ȡ������ѧϰ������������У�������Ϣ�������������������緿�䣨�ϸ����棩
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