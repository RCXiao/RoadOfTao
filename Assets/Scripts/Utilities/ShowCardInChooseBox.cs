using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShowCardInChooseBox : MonoBehaviour
{
    public Button cardButton;

    public GameObject cardObject0;
    public GameObject cardObject1;

    public Image cardImage0;
    public Image cardImage1;
    public TextMeshProUGUI cardName0;
    public TextMeshProUGUI cardName1;
    public TextMeshProUGUI cardCost0;
    public TextMeshProUGUI cardCost1;
    public TextMeshProUGUI cardDescription0;
    public TextMeshProUGUI cardDescription1;

    public CardDataSO cardData;

    public ObjectEventSO cardBeenChooseEvent;

    public bool isCurrentCard = false;

    private void Start()
    {
        cardButton.onClick.AddListener(() =>
        {
            //ÓÒ¼ü
            if (cardObject0.transform.GetSiblingIndex() != 0)
            {
                cardObject0.transform.SetAsFirstSibling();
            }
            else
            {
                cardObject1.transform.SetAsFirstSibling();
            }

            //×ó¼ü
            if (cardButton.image.color.a != 0)
            {
                cardButton.image.color = new Color(cardButton.image.color.r, cardButton.image.color.g, cardButton.image.color.b, 0);
                cardBeenChooseEvent.RaiseEvent(this, this);
            }
            else
            {
                cardButton.image.color = new Color(cardButton.image.color.r, cardButton.image.color.g, cardButton.image.color.b, 120/255f);
                cardBeenChooseEvent.RaiseEvent(null, this);
            }

            isCurrentCard = !isCurrentCard;
        });
    }

    public void AnotherCardBeenChoose(object obj)
    {
        if ((ShowCardInChooseBox)obj != this)
        {
            cardButton.image.color = new Color(cardButton.image.color.r, cardButton.image.color.g, cardButton.image.color.b, 120 / 255f);
            isCurrentCard = false;
        }
    }
}
