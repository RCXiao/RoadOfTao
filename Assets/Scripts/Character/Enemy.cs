using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Enemy : CharacterBase
{
    //public EnemyActionDataSO actionDataSO;
    //public EnemyAction currentAction;

    [Header("卡牌配置")]
    public List<CardDataSO> intentionPool = new();  // 意图池
    public List<float> weightOfIntention = new();  // 独立权重池
    [SerializeField]
    private bool intentionState = true;//意图阴阳

    [Header("运行时状态")]
    private Dictionary<CardDataSO, float> _runtimeWeights = new();  // 意图-权重映射
    private List<CardDataSO> _usedCards = new();  // 已使用意图记录
    private List<float> _initialWeightsBackup;    // 初始权重备份

    [SerializeField]
    protected Player player;
    public GameObject intentionOBJ;
    public Image intentionImage;
    [SerializeField]
    private CardDataSO currentIntention;//当前意图

    public Button showCardBtn;
    public GameObject showCardPanel;
    public Image cardImage0;
    public Image cardImage1;
    public TextMeshProUGUI cardName0;
    public TextMeshProUGUI cardName1;
    public TextMeshProUGUI cardCost0;
    public TextMeshProUGUI cardCost1;
    public TextMeshProUGUI cardDescription0;
    public TextMeshProUGUI cardDescription1;

    protected override void Awake()
    {
        //base.Awake();
        showCardBtn.onClick.AddListener(ShowCard);

    }

    protected override void Start()
    {
        base.Start();
        InitializeWeights();
    }
    #region 意图选择
    // 数据校验（防止配置错误）
    public void InitializeWeights()
    {
        // 有效性校验
        if (intentionPool.Count != weightOfIntention.Count)
        {
            Debug.LogError("卡牌池与权重池长度不匹配");
            return;
        }

        // 创建权重字典并备份初始权重
        for (int i = 0; i < intentionPool.Count; i++)
        {
            _runtimeWeights.Add(intentionPool[i], weightOfIntention[i]);
        }
        _initialWeightsBackup = new List<float>(weightOfIntention);
    }

    // 核心随机选择逻辑
    public CardDataSO GetRandomIntention()
    {
        // 重置检测
        if (_usedCards.Count >= intentionPool.Count)
        {
            ResetWeights();
            Debug.Log("意图池已用完，重置权重");
        }

        // 计算总权重
        float totalWeight = 0f;
        foreach (var pair in _runtimeWeights)
        {
            totalWeight += pair.Value;
        }

        // 加权随机选择
        float randomPoint = Random.Range(0, totalWeight);
        CardDataSO selectedCard = null;
        float cumulative = 0f;

        foreach (var pair in _runtimeWeights)
        {
            cumulative += pair.Value;
            if (cumulative >= randomPoint)
            {
                selectedCard = pair.Key;
                break;
            }
        }

        // 权重调整逻辑
        if (selectedCard != null)
        {
            AdjustWeights(selectedCard);

            if(!_usedCards.Contains(selectedCard))
            {
                _usedCards.Add(selectedCard);
            }
        }

        return selectedCard;
    }

    // 权重调整方法
    private void AdjustWeights(CardDataSO targetCard)
    {
        float originalWeight = _runtimeWeights[targetCard];
        float deductedWeight = Mathf.Ceil(originalWeight / 4f);

        // 创建临时键集合副本
        var cards = new List<CardDataSO>(_runtimeWeights.Keys);
        int validTargets = cards.Count - 1;

        if (validTargets > 0)
        {
            float delta = deductedWeight / validTargets;

            // 遍历临时集合
            foreach (var card in cards)
            {
                if (card != targetCard)
                {
                    _runtimeWeights[card] += delta;
                }
            }
        }

        _runtimeWeights[targetCard] = originalWeight - deductedWeight;
    }

    // 重置方法
    private void ResetWeights()
    {
        for (int i = 0; i < intentionPool.Count; i++)
        {
            if (i < _initialWeightsBackup.Count)
            {
                _runtimeWeights[intentionPool[i]] = _initialWeightsBackup[i];
            }
        }
        _usedCards.Clear();
    }
    #endregion

    private void ShowCard()
    {
        cardImage0.sprite = currentIntention.cardImage0;
        cardImage1.sprite = currentIntention.cardImage1;
        cardName0.text = currentIntention.cardName0;
        cardName1.text = currentIntention.cardName1;
        cardDescription0.text = currentIntention.cardDescription_0;
        cardDescription1.text = currentIntention.cardDescription_1;
        cardCost0.text = currentIntention.cardCost_0.ToString();
        cardCost1.text = currentIntention.cardCost_1.ToString();

        showCardPanel.SetActive(true);
    }

    public void ChangedIntentionState(int state)
    {
        //Debug.Log(state);
        if (state < 50)
        {
            intentionState = false;
        }

        else if(state > 50)
        {
            intentionState = true;
        }

        else if(state == 50)
        {
            intentionState = UnityEngine.Random.Range(0, 2) == 0;

        }
    }

    public virtual void OnPlayerTurnBegin()
    {
        
    }

    public virtual void OnEnemyTurnBegin()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

        ResetDefense();

        //意图消失动画
        intentionOBJ.transform.DOScale(Vector3.zero, 1f).OnComplete(() =>
        {
            intentionOBJ.SetActive(false);

            //TODO:效果执行
            if(intentionState==false)
            {
                //Debug.Log("使用卡牌阳");
                foreach (Effect effect in currentIntention.effects_0)
                {
                    //Debug.Log(effect.name);
                    effect.Execute(this,player);
                }
            }
            else if(intentionState==true)
            {
                //Debug.Log("使用卡牌阴");
                foreach (Effect effect in currentIntention.effects_1)
                {
                    //Debug.Log(effect.name);
                    effect.Execute(this, player);
                }
            }

            
                // 使用 DOTween 实现震动效果
                Vector3 originalPosition = characterSelf.transform.position;
                float shakeOffset = 0.3f; // 震动偏移量
                float shakeDuration = 0.2f; // 震动持续时间

                // 向左震动并回到原位
                characterSelf.transform.DOMoveX(originalPosition.x - shakeOffset, shakeDuration)
                    .SetLoops(2, LoopType.Yoyo) // 往返震动
                    .SetEase(Ease.InOutQuad);  // 平滑的缓动
            
            
        });
    }

    public virtual void OnEnemyTurnEnd()
    {
        currentIntention = GetRandomIntention();
        //Debug.Log("Enemy选择卡牌：" + currentIntention.name);

        intentionImage.sprite = currentIntention.cardImage0;
        intentionOBJ.SetActive(true);
        intentionOBJ.transform.DOScale(Vector3.one, 1f);
    }
}
