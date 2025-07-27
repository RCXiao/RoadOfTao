using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardOnDeck : MonoBehaviour
{
    public ObjectEventSO cardOnDeckBeenClicked;

    public Button cardButton;

    public Image cardImage0;
    public Image cardImage1;
    public TextMeshProUGUI cardName0;
    public TextMeshProUGUI cardName1;
    public TextMeshProUGUI cardCost0;
    public TextMeshProUGUI cardCost1;
    public TextMeshProUGUI cardDescription0;
    public TextMeshProUGUI cardDescription1;


    private void Awake()
    {
        cardButton.transform.position= new Vector3(cardButton.transform.position.x, 103, 0);
    }

    public void StartEvent()
    {
        cardOnDeckBeenClicked.RaiseEvent(this, this);
    }
}

