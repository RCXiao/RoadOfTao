using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardOnSmelt : MonoBehaviour
{
    public Image cardImage0;
    public Image cardImage1;
    public TextMeshProUGUI cardName0;
    public TextMeshProUGUI cardName1;
    public TextMeshProUGUI cardCost0;
    public TextMeshProUGUI cardCost1;
    public TextMeshProUGUI cardDescription0;
    public TextMeshProUGUI cardDescription1;
    public int cardID0;
    public int cardID1;
    public CardDataSO cardData0;
    public CardDataSO cardData1;

    public ObjectEventSO ChooseCardOnSmeltEvent;

    public void ChickCardOnSmelt()
    {
        ChooseCardOnSmeltEvent.RaiseEvent(this, this);
        Debug.Log("Choose Card On Smelt");
    }
}



