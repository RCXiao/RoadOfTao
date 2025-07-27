using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "CardLibrarySO", menuName = "Card/CardLibrarySO")]
public class CardLibrarySO : ScriptableObject
{
    public List<CardLibraryEntry> cardLibraryList;
}

[System.Serializable]
public struct CardLibraryEntry
{
    public CardDataSO cardData0;
    public CardDataSO cardData1;
    public int amount;
}