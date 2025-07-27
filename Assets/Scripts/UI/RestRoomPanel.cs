using System;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class RestRoomPanel : MonoBehaviour
{
    public Button healBtn;
    public Button smeltBtn;
    public Button backToMapBtn;
    public Button startSmeltBtn;
    public Button cardYang;
    public Button cardYin;

    public GameObject grid;
    public GameObject cardPrefab;
    public Image imageYang;
    public Image imageYin;

    public ObjectEventSO loadMapEvent;

    public GameObject smeltPanel;

    private CardOnSmelt currentCardYang;//选中的阳卡
    private CardOnSmelt currentCardYin;//选中的阴卡
    private int currentYinYang = 0;

    //卡牌显示
    public Image cardImage0;
    public Image cardImage1;
    public TextMeshProUGUI cardName0;
    public TextMeshProUGUI cardName1;
    public TextMeshProUGUI cardCost0;
    public TextMeshProUGUI cardCost1;
    public TextMeshProUGUI cardDescription0;
    public TextMeshProUGUI cardDescription1;
    public GameObject card0;
    public GameObject card1;

    private void OnEnable()
    {
        healBtn.onClick.AddListener(OnHealBtnClick);
        smeltBtn.onClick.AddListener(OnSmeltBtnClick);
        backToMapBtn.onClick.AddListener(OnBackToMapBtnClick);
        startSmeltBtn.onClick.AddListener(OnStartSmeltBtnClick);
        cardYang.onClick.AddListener(OnCardYangClick);
        cardYin.onClick.AddListener(OnCardYinClick);
        healBtn.transform.gameObject.SetActive(true);
        smeltBtn.transform.gameObject.SetActive(true);
    }


    private void OnHealBtnClick()
    {
        healBtn.transform.gameObject.SetActive(false);
        smeltBtn.transform.gameObject.SetActive(false);

        float currentHP = GameManager.Instance.player.CurrentHP;
        float maxHP = GameManager.Instance.player.MaxHp;
        
        if(currentHP < maxHP/2)
        {
            GameManager.Instance.player.HealHealth((int)(maxHP - currentHP) / 2);
            Debug.Log("Healed1");
        }
        else if(currentHP > maxHP/2)
        {
            GameManager.Instance.player.HealHealth((int)(currentHP / maxHP * (maxHP - currentHP)));

            Debug.Log("Healed2");
        }
    }

    private void OnSmeltBtnClick()
    {
        healBtn.transform.gameObject.SetActive(false);
        smeltBtn.transform.gameObject.SetActive(false);
        smeltPanel.SetActive(true);

        ShowCardOnSmelt();
    }

    private void OnBackToMapBtnClick()
    {
        loadMapEvent.RaiseEvent(null,this);
    }

    private void OnStartSmeltBtnClick()
    {
        if(currentCardYang!= null && currentCardYin!= null)
        {
            SmeltCard(currentCardYang, currentCardYin);

            currentYinYang = 0;
            currentCardYang=null;
            currentCardYin=null;
            imageYang.gameObject.SetActive(false);
            imageYin.gameObject.SetActive(false);
            smeltPanel.SetActive(false);
        }
    }

    private void OnCardYangClick()
    {
        imageYang.gameObject.SetActive(true);
        imageYin.gameObject.SetActive(false);
        currentYinYang = 1;
    }

    private void OnCardYinClick()
    {
        imageYang.gameObject.SetActive(false);
        imageYin.gameObject.SetActive(true);
        currentYinYang = 2;
    } 

    public void ShowCardOnSmelt()
    {
        if (grid.transform.childCount > 0)
        {
            foreach (Transform child in grid.transform)
            {
                // 销毁子物体
                Destroy(child.gameObject);
            }
        }

        foreach (var cardList in CardManager.Instance.currentLibrary.cardLibraryList)
        {
            int cardAmount = cardList.amount;
            while(cardAmount>0)
            {
                GameObject card = Instantiate(cardPrefab, Vector3.zero, quaternion.identity, grid.transform);
                CardOnSmelt cardOnSmelt = card.GetComponent<CardOnSmelt>();
                cardOnSmelt.cardImage0.sprite = cardList.cardData0.cardImage0;
                cardOnSmelt.cardImage1.sprite = cardList.cardData1.cardImage1;
                cardOnSmelt.cardCost0.text = cardList.cardData0.cardCost_0.ToString();
                cardOnSmelt.cardCost1.text = cardList.cardData1.cardCost_1.ToString();
                cardOnSmelt.cardName0.text = cardList.cardData0.cardName0;
                cardOnSmelt.cardName1.text = cardList.cardData1.cardName1;
                cardOnSmelt.cardDescription0.text = cardList.cardData0.cardDescription_0;
                cardOnSmelt.cardDescription1.text = cardList.cardData1.cardDescription_1;
                cardOnSmelt.cardID0 = cardList.cardData0.cardID0;
                cardOnSmelt.cardID1 = cardList.cardData1.cardID1;
                cardOnSmelt.cardData0 = cardList.cardData0;
                cardOnSmelt.cardData1 = cardList.cardData1;

                cardAmount = cardAmount - 1;
            }

        }
    }

    //事件监听函数，监听到单击卡牌事件
    public void CardBeenChoose(object obj)
    {
        CardOnSmelt cardOnSmelt = obj as CardOnSmelt;
        if (currentYinYang == 0)
            return;
        else if (currentYinYang == 1)
        {
            if(currentCardYin != cardOnSmelt)
            {
                currentCardYang = cardOnSmelt;

                cardImage0.sprite = cardOnSmelt.cardImage0.sprite;
                cardName0.text = cardOnSmelt.cardName0.text;
                cardCost0.text = cardOnSmelt.cardCost0.text;
                cardDescription0.text = cardOnSmelt.cardDescription0.text;

                card0.SetActive(true);
            }         
        }    
        else if (currentYinYang == 2)
        {
            if(currentCardYang != cardOnSmelt)
            {
                currentCardYin = cardOnSmelt;

                cardImage1.sprite = cardOnSmelt.cardImage1.sprite;
                cardName1.text = cardOnSmelt.cardName1.text;
                cardCost1.text = cardOnSmelt.cardCost1.text;
                cardDescription1.text = cardOnSmelt.cardDescription1.text;

                card1.SetActive(true);
            }
        }
    }

    public void SmeltCard(CardOnSmelt newCardData1, CardOnSmelt newCardData2)
    {
        // 确保在主线程上调用
        if (!Application.isPlaying || System.Threading.Thread.CurrentThread.ManagedThreadId != 1)
        {
            Debug.LogError("SmeltCard must be called from the main thread.");
            return;
        }

        // 使用Dictionary来辅助查找和更新卡片数量
        Dictionary<List<CardDataSO>, CardLibraryEntry> cardLibraryDict = new Dictionary<List<CardDataSO>, CardLibraryEntry>();

        foreach (var entry in CardManager.Instance.currentLibrary.cardLibraryList)
        {
            List<CardDataSO> key = new List<CardDataSO> { entry.cardData0, entry.cardData1};
            cardLibraryDict[key] = entry;
        }

        // 反向查询 newCardData1 和 newCardData2 对应的 CardLibraryEntry
        List<CardDataSO> currentNewCardKey = new List<CardDataSO> { newCardData1.cardData0, newCardData2.cardData1 };

        // 创建新卡牌的 CardLibraryEntry
        var newCardData0Data = newCardData1.cardData0;
        var newCardData1Data = newCardData2.cardData1;

        // 检查是否已经存在对应的新卡牌
        var existingIndex = CardManager.Instance.currentLibrary.cardLibraryList.FindIndex(
            t => t.cardData0.cardID0 == newCardData0Data.cardID0 && t.cardData1.cardID1 == newCardData1Data.cardID1);

        if (existingIndex != -1)
        {
            // 如果存在，增加数量
            var entry = CardManager.Instance.currentLibrary.cardLibraryList[existingIndex];
            entry.amount++;
            CardManager.Instance.currentLibrary.cardLibraryList[existingIndex] = entry;

        }
        else
        {
            // 如果不存在，创建新的 CardLibraryEntry
            var newCard = new CardLibraryEntry
            {
                cardData0 = newCardData0Data,
                cardData1 = newCardData1Data,
                amount = 1
            };
            CardManager.Instance.currentLibrary.cardLibraryList.Add(newCard);
        }

        //查找被熔炼的两张牌并减少数量
        var existingIndex1 = CardManager.Instance.currentLibrary.cardLibraryList.FindIndex(
            t => t.cardData0.cardID0 == newCardData1.cardID0 && t.cardData1.cardID1 == newCardData1.cardID1);

        if (existingIndex1 != -1)
        {
            // 如果存在，减少数量
            var entry1 = CardManager.Instance.currentLibrary.cardLibraryList[existingIndex1];
            entry1.amount--;
            CardManager.Instance.currentLibrary.cardLibraryList[existingIndex1] = entry1;

            // 如果原卡牌数量为 0，从列表中移除
            if (entry1.amount <= 0)
            {
                CardManager.Instance.currentLibrary.cardLibraryList.Remove(entry1);
            }
        }

        var existingIndex2 = CardManager.Instance.currentLibrary.cardLibraryList.FindIndex(
            t => t.cardData0.cardID0 == newCardData2.cardID0 && t.cardData1.cardID1 == newCardData2.cardID1);

        if (existingIndex2 != -1)
        {
            // 如果存在，减少数量
            var entry2 = CardManager.Instance.currentLibrary.cardLibraryList[existingIndex2];
            entry2.amount--;
            CardManager.Instance.currentLibrary.cardLibraryList[existingIndex2] = entry2;

            // 如果原卡牌数量为 0，从列表中移除
            if (entry2.amount <= 0)
            {
                CardManager.Instance.currentLibrary.cardLibraryList.Remove(entry2);
            }
        }
    }
}
