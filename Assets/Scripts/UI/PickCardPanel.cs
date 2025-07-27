using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PickCardPanel : MonoBehaviour
{
    public GameObject cardPrefab;
    public GameObject pickCardPanel;
    public GameObject cardChooseBox;
    private CardDataSO currentCardData;
    public Button chooseCardBtn;

    private void OnEnable()
    {
        if(cardChooseBox.transform.childCount > 0)
        {
            Destroy(cardChooseBox.transform.GetChild(0).gameObject);          
            Destroy(cardChooseBox.transform.GetChild(1).gameObject);          
            Destroy(cardChooseBox.transform.GetChild(2).gameObject);
        }

        var data = CardManager.Instance.GetNewCardData();
        for(int i = 0; i < 3; i++)
        {
            GameObject cardInstance = Instantiate(cardPrefab, cardChooseBox.transform.position, Quaternion.identity);
            cardInstance.transform.SetParent(cardChooseBox.transform);

            cardInstance.GetComponent<ShowCardInChooseBox>().cardImage0.sprite = data[i].cardImage0;
            cardInstance.GetComponent<ShowCardInChooseBox>().cardImage1.sprite = data[i].cardImage1;
            cardInstance.GetComponent<ShowCardInChooseBox>().cardName0.text = data[i].cardName0;
            cardInstance.GetComponent<ShowCardInChooseBox>().cardName1.text = data[i].cardName1;
            cardInstance.GetComponent<ShowCardInChooseBox>().cardCost0.text = data[i].cardCost_0.ToString();
            cardInstance.GetComponent<ShowCardInChooseBox>().cardCost1.text = data[i].cardCost_1.ToString();
            cardInstance.GetComponent<ShowCardInChooseBox>().cardDescription0.text = data[i].cardDescription_0;
            cardInstance.GetComponent<ShowCardInChooseBox>().cardDescription1.text = data[i].cardDescription_1;
            cardInstance.GetComponent<ShowCardInChooseBox>().cardData = data[i];

        }
    }

    private void Awake()
    {
        chooseCardBtn.onClick.AddListener(() =>
        {
            if(currentCardData == null)
            {
                Debug.Log("并未选取任何卡牌");
            }
            else
            {
                CardManager.Instance.UnlockCard(currentCardData);
                currentCardData = null;
            }
            pickCardPanel.SetActive(false);
        });
    }

    public void OnCardBeenChoose(object obj)
    {
        if(obj == null)
        {
            currentCardData = null;
        }
        else
        {
            ShowCardInChooseBox card = (ShowCardInChooseBox)obj;
            currentCardData = card.cardData;
        }
    }
}
