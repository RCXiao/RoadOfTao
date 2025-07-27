using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;

public class CardPilePanel : MonoBehaviour
{
    public Button discardPileButton;
    public Button cardDeckButton;

    public GameObject cardPilePanel;
    public GameObject grid;
    public GameObject cardPrefab;

    public GameObject showCardPanel;
    public Image cardImage0;
    public Image cardImage1;
    public TextMeshProUGUI cardName0;
    public TextMeshProUGUI cardName1;
    public TextMeshProUGUI cardCost0;
    public TextMeshProUGUI cardCost1;
    public TextMeshProUGUI cardDescription0;
    public TextMeshProUGUI cardDescription1;

    void Start()
    {
        discardPileButton.onClick.AddListener(() =>
        {
            ShowCardOnDiscardPile();
            cardPilePanel.SetActive(true);

        });

        cardDeckButton.onClick.AddListener(() =>
        {
            ShowCardOnCardDeck();
            cardPilePanel.SetActive(true);
        });
    }

    public void ShowCardOnDiscardPile()
    {
        if(grid.transform.childCount>0)
        {
            foreach (Transform child in grid.transform)
            {
                // 销毁子物体
                Destroy(child.gameObject);
            }
        }

        foreach (var cardList in CardDeck.Instance._discardDeck)
        {
            GameObject card = Instantiate(cardPrefab, Vector3.zero, quaternion.identity, grid.transform);
            CardOnDeck cardOnDeck = card.GetComponent<CardOnDeck>();
            cardOnDeck.cardImage0.sprite = cardList[0].cardImage0;
            cardOnDeck.cardImage1.sprite = cardList[1].cardImage1;
            cardOnDeck.cardCost0.text = cardList[0].cardCost_0.ToString();
            cardOnDeck.cardCost1.text = cardList[1].cardCost_1.ToString();
            cardOnDeck.cardName0.text = cardList[0].cardName0;
            cardOnDeck.cardName1.text = cardList[1].cardName1;
            cardOnDeck.cardDescription0.text = cardList[0].cardDescription_0;
            cardOnDeck.cardDescription1.text = cardList[1].cardDescription_1;
        }
    }

    public void ShowCardOnCardDeck()
    {
        if (grid.transform.childCount > 0)
        {
            foreach (Transform child in grid.transform)
            {
                // 销毁子物体
                Destroy(child.gameObject);
            }
        }

        List<List<CardDataSO>> cardAfterArray;
        cardAfterArray = CardDeck.Instance._drawDeck;
        cardAfterArray = cardAfterArray.OrderBy(innerList => innerList.Count > 0 ? innerList[0].cardID0 : int.MaxValue).ToList();

        foreach (var cardList in cardAfterArray)
        {
            GameObject card = Instantiate(cardPrefab, Vector3.zero, quaternion.identity, grid.transform);
            CardOnDeck cardOnDeck = card.GetComponent<CardOnDeck>();
            cardOnDeck.cardImage0.sprite = cardList[0].cardImage0;
            cardOnDeck.cardImage1.sprite = cardList[1].cardImage1;
            cardOnDeck.cardCost0.text = cardList[0].cardCost_0.ToString();
            cardOnDeck.cardCost1.text = cardList[1].cardCost_1.ToString();
            cardOnDeck.cardName0.text = cardList[0].cardName0;
            cardOnDeck.cardName1.text = cardList[1].cardName1;
            cardOnDeck.cardDescription0.text = cardList[0].cardDescription_0;
            cardOnDeck.cardDescription1.text = cardList[1].cardDescription_1;
        }
    }

    public void ShowCardPanel(object obj)
    {
        CardOnDeck cardOnDeck = obj as CardOnDeck;
        cardImage0.sprite = cardOnDeck.cardImage0.sprite;
        cardImage1.sprite = cardOnDeck.cardImage1.sprite;
        cardCost0.text = cardOnDeck.cardCost0.text;
        cardCost1.text = cardOnDeck.cardCost1.text;
        cardName0.text = cardOnDeck.cardName0.text;
        cardName1.text = cardOnDeck.cardName1.text;
        cardDescription0.text = cardOnDeck.cardDescription0.text;
        cardDescription1.text = cardOnDeck.cardDescription1.text;
        showCardPanel.SetActive(true);
    }
}
