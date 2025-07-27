using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using DG.Tweening;
using TMPro;

public class CardManager : MonoBehaviour
{
    public static CardManager Instance { get; private set; }

    public PoolTool poolTool_CardInGame;
    public List<CardDataSO> cardDataList;

    [Header("卡牌库")]
    public CardLibrarySO newGameCardLibrary;//初始卡牌库
    public CardLibrarySO currentLibrary;//玩家当前卡牌库

    private int previousIndex;

    public Vector3 discardPosition;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        InitializeCardDataList();

        //开局初始化牌库（awake执行一次）TODO:开局初始化牌库应该在游戏开始时执行，而不是awake
        foreach (var item in newGameCardLibrary.cardLibraryList)
        {
            currentLibrary.cardLibraryList.Add(item);
        }
    }

    private void OnDisable()
    {
        currentLibrary.cardLibraryList.Clear();
    }

    #region 卡牌获取
    //初始化获得所有卡牌数据
    private void InitializeCardDataList()
    {
        Addressables.LoadAssetsAsync<CardDataSO>("CardData", null).Completed += OnCardDataLoaded;
    }

    //回调函数，加载所有卡牌数据
    private void OnCardDataLoaded(AsyncOperationHandle<IList<CardDataSO>> handle)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            cardDataList = new List<CardDataSO>(handle.Result);
        }
        else
        {
            Debug.LogError("No CardData Found!");
        }
    }
    #endregion

    //抽卡时调用，获得卡牌GameObject
    public GameObject GetCardObject()
    {
        var cardObj = poolTool_CardInGame.GetObjectFromPool();
        cardObj.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        return cardObj;

    }

    public void DiscardCard(GameObject cardObj)
    {
        // 首先获取到卡牌的 Transform
        Transform cardMoveTransform = cardObj.transform.GetChild(0);
        Transform cardTransform = cardObj.transform;

        // 创建一个新的 Sequence
        Sequence sequence = DOTween.Sequence();

        // 添加缩放动画（从当前缩放到0.5）
        sequence.Join(cardTransform.DOScale(0.5f, 0.5f));

        // 添加移动动画（移动到目标位置）
        sequence.Join(cardMoveTransform.DOMove(discardPosition, 0.5f));

        // 动画完成后回收卡牌
        sequence.OnComplete(() => {
            poolTool_CardInGame.ReturnObjectToPool(cardObj);
        });

        // 播放动画
        sequence.Play();
    }

    public List<CardDataSO> GetNewCardData()
    {

        // 生成索引池并洗牌
        List<int> indices = new List<int>(cardDataList.Count);
        for (int i = 0; i < cardDataList.Count; i++)
        {
            indices.Add(i);
        }

        // 洗牌操作‌
        for (int i = 0; i < indices.Count; i++)
        {
            int swapIndex = UnityEngine.Random.Range(i, indices.Count);
            int temp = indices[i];
            indices[i] = indices[swapIndex];
            indices[swapIndex] = temp;
        }

        // 提取前三个元素
        List<CardDataSO> result = new List<CardDataSO>();
        for (int i = 0; i < 3; i++)
        {
            result.Add(cardDataList[indices[i]]);
        }

        return result;
    }

    public void UnlockCard(CardDataSO newCardData)
    {
        // 确保在主线程上调用
        if (!Application.isPlaying || System.Threading.Thread.CurrentThread.ManagedThreadId != 1)
        {
            Debug.LogError("UnlockCard must be called from the main thread.");
            return;
        }

        // 确保newCardData不为空
        if (newCardData == null)
        {
            Debug.LogError("newCardData cannot be null.");
            return;
        }

        // 使用Dictionary来辅助查找和更新卡片数量
        Dictionary<List<CardDataSO>, CardLibraryEntry> cardLibraryDict = new Dictionary<List<CardDataSO>, CardLibraryEntry>();

        foreach (var entry in currentLibrary.cardLibraryList)
        {
            List<CardDataSO> key = new List<CardDataSO> { entry.cardData0, entry.cardData1 };
            cardLibraryDict[key] = entry;
        }

        // 创建新卡牌的 CardLibraryEntry
        var newCardData0Data = newCardData;
        var newCardData1Data = newCardData;

        // 检查是否已经存在对应的新卡牌
        var existingIndex = currentLibrary.cardLibraryList.FindIndex(
            t => t.cardData0.cardID0 == newCardData0Data.cardID0 && t.cardData1.cardID1 == newCardData1Data.cardID1);

        if (existingIndex != -1)
        {
            // 如果存在，增加数量
            var entry = currentLibrary.cardLibraryList[existingIndex];
            entry.amount++;
            currentLibrary.cardLibraryList[existingIndex] = entry;

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
            currentLibrary.cardLibraryList.Add(newCard);
        }
    }
}
