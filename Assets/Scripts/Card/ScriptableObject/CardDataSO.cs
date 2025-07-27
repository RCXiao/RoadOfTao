using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "CardDataSO", menuName = "Card/CardDataSO")]
public class CardDataSO : ScriptableObject
{
    public string cardName0;
    public string cardName1;

    public Sprite cardImage0;
    public Sprite cardImage1;

    public int cardID0;
    public int cardID1;

    public int cardCost_0;
    public int cardCost_1;

    public CardType cardType_0;
    public CardType cardType_1;

    [TextArea]
    public string cardDescription_0;
    [TextArea]
    public string cardDescription_1;

    public List<Effect> effects_0;
    public List<Effect> effects_1;
}