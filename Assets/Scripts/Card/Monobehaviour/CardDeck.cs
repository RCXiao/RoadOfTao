using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using DG.Tweening;
using System.Collections;

public class CardDeck : MonoBehaviour
{
    public static CardDeck Instance;

    public CardManager cardManager;
    public CardLayoutManager layoutManager;

    public Vector3 deckPosition;

    private List<List<CardDataSO>> drawDeck = new();//抽牌堆
    private List<List<CardDataSO>> discardDeck = new();//弃牌堆
    private List<Card> handCardObjectList = new();//当前手牌（每回合）

    [Header("事件广播")]
    public IntEventSO drawCountEvent;
    public IntEventSO discardCountEvent;
    public ObjectEventSO cardBeenUseEvent;

    public List<List<CardDataSO>> _drawDeck { get => drawDeck; set => drawDeck = value; }
    public List<List<CardDataSO>> _discardDeck { get => discardDeck; set => discardDeck = value; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        InitializeDeck();//TODO:测试用
    }

    public void InitializeDeck()
    {
        drawDeck.Clear();
        int j = 0;
        foreach (var entry in cardManager.currentLibrary.cardLibraryList)
        {
            for (int i = 0; i < entry.amount; i++)
            {
                drawDeck.Add(new List<CardDataSO>());
                drawDeck[j].Add(entry.cardData0);
                drawDeck[j].Add(entry.cardData1);
                j = j + 1;
            }
        }
        j = 0;

        //洗牌（更新抽牌堆和弃牌堆显示的数字）
        ShuffleDeck();

    }

    [ContextMenu("Draw Card")]
    public void TestDrawCard()
    {
        DrawCard(1);
    }

    public void NewTurnDrawCards()
    {
        StartCoroutine(DelayedDrawCard(4));
    }

    private IEnumerator DelayedDrawCard(int amount)
    {
        yield return new WaitForSeconds(0.001f);
        DrawCard(amount);
    }

    public void DrawCard(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            if (drawDeck.Count == 0)
            {
                foreach (var item in discardDeck)
                {
                    drawDeck.Add(item);
                }
                ShuffleDeck();
            }
            CardDataSO currentCardData0 = drawDeck[0][0];
            CardDataSO currentCardData1 = drawDeck[0][1];

            drawDeck.RemoveAt(0);

            //更新UI显示数量，事件广播
            drawCountEvent.RaiseEvent(drawDeck.Count, this);
            cardBeenUseEvent.RaiseEvent(null, this);

            var card = cardManager.GetCardObject().GetComponent<Card>();
            //初始化
            card.Init(currentCardData0, currentCardData1);

            card.transform.position = deckPosition;

            card.targetTransform.localPosition = new Vector3(0, 0, 0);

            handCardObjectList.Add(card);

            var delay = i * 0.2f;
            SetCardLayout(delay);
        }
    }

    //设置手牌布局
    private void SetCardLayout(float delay)
    {
        for (int i = 0; i < handCardObjectList.Count; i++)
        {
            Card currentCard = handCardObjectList[i];

            CardTransform cardTransform = layoutManager.GetCardTransform(i, handCardObjectList.Count);
            cardTransform.pos.z = -i;

            //能量更新
            currentCard.UpdateCardState();

            currentCard.isAnimating = true;

            //currentCard.transform.DOScale(Vector3.one, 0.8f)
            //.SetDelay(delay)
            //.SetEase(Ease.InOutSine);

            //currentCard.transform.DOMove(cardTransform.pos, 0.8f)
            //    .SetDelay(delay)
            //    .SetEase(Ease.InOutSine)
            //    .onComplete = () => currentCard.isAnimating = false;

            Sequence sequence = DOTween.Sequence();
            sequence.Join(currentCard.transform.DOScale(Vector3.one, 0.8f).SetEase(Ease.InOutSine));
            sequence.Join(currentCard.transform.DOMove(cardTransform.pos, 0.8f).SetEase(Ease.InOutSine));
            sequence.AppendCallback(() => {
                currentCard.isAnimating = false;
            });

            //设置卡牌牌序
            currentCard.GetComponent<SortingGroup>().sortingOrder = i+1;
            currentCard.UpdatePositionRotation(cardTransform.pos, cardTransform.rot);
        }
    }

    // 洗牌
    private void ShuffleDeck()
    {
        discardDeck.Clear();

        //更新UI显示数量，事件广播
        drawCountEvent.RaiseEvent(drawDeck.Count, this);
        discardCountEvent.RaiseEvent(discardDeck.Count, this);
        cardBeenUseEvent.RaiseEvent(null, this);

        for (int i = 0; i < drawDeck.Count; i++)
        {
            List<CardDataSO> temp = drawDeck[i];
            int randomIndex = Random.Range(i, drawDeck.Count);
            drawDeck[i] = drawDeck[randomIndex];
            drawDeck[randomIndex] = temp;
        }
    }

    // 弃牌逻辑,事件函数
    public void DiscardCard(object obj)
    {
        Card card = obj as Card;

        List<CardDataSO> newDiscardList = new List<CardDataSO>();
        newDiscardList.Add(card.cardData0);
        newDiscardList.Add(card.cardData1);
        discardDeck.Add(newDiscardList);

        handCardObjectList.Remove(card);

        cardManager.DiscardCard(card.gameObject);

        discardCountEvent.RaiseEvent(discardDeck.Count, this);

        SetCardLayout(0f);
    }

    public void OnPlayerTurnEnd()
    {
        for (int i = 0; i < handCardObjectList.Count; i++)
        {
            List<CardDataSO> newDiscardList = new List<CardDataSO>();
            newDiscardList.Add(handCardObjectList[i].cardData0);
            newDiscardList.Add(handCardObjectList[i].cardData1);
            discardDeck.Add(newDiscardList);

            cardManager.DiscardCard(handCardObjectList[i].gameObject);
        }

        handCardObjectList.Clear();
        discardCountEvent.RaiseEvent(discardDeck.Count, this);
    }

    public void ReleaseAllCards(object obj)
    {
        foreach (var card in handCardObjectList)
        {
            cardManager.DiscardCard(card.gameObject);
        }

        handCardObjectList.Clear();
        InitializeDeck();
    }
}
