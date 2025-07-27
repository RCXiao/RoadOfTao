using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Enemy : CharacterBase
{
    //public EnemyActionDataSO actionDataSO;
    //public EnemyAction currentAction;

    [Header("��������")]
    public List<CardDataSO> intentionPool = new();  // ��ͼ��
    public List<float> weightOfIntention = new();  // ����Ȩ�س�
    [SerializeField]
    private bool intentionState = true;//��ͼ����

    [Header("����ʱ״̬")]
    private Dictionary<CardDataSO, float> _runtimeWeights = new();  // ��ͼ-Ȩ��ӳ��
    private List<CardDataSO> _usedCards = new();  // ��ʹ����ͼ��¼
    private List<float> _initialWeightsBackup;    // ��ʼȨ�ر���

    [SerializeField]
    protected Player player;
    public GameObject intentionOBJ;
    public Image intentionImage;
    [SerializeField]
    private CardDataSO currentIntention;//��ǰ��ͼ

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
    #region ��ͼѡ��
    // ����У�飨��ֹ���ô���
    public void InitializeWeights()
    {
        // ��Ч��У��
        if (intentionPool.Count != weightOfIntention.Count)
        {
            Debug.LogError("���Ƴ���Ȩ�سس��Ȳ�ƥ��");
            return;
        }

        // ����Ȩ���ֵ䲢���ݳ�ʼȨ��
        for (int i = 0; i < intentionPool.Count; i++)
        {
            _runtimeWeights.Add(intentionPool[i], weightOfIntention[i]);
        }
        _initialWeightsBackup = new List<float>(weightOfIntention);
    }

    // �������ѡ���߼�
    public CardDataSO GetRandomIntention()
    {
        // ���ü��
        if (_usedCards.Count >= intentionPool.Count)
        {
            ResetWeights();
            Debug.Log("��ͼ�������꣬����Ȩ��");
        }

        // ������Ȩ��
        float totalWeight = 0f;
        foreach (var pair in _runtimeWeights)
        {
            totalWeight += pair.Value;
        }

        // ��Ȩ���ѡ��
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

        // Ȩ�ص����߼�
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

    // Ȩ�ص�������
    private void AdjustWeights(CardDataSO targetCard)
    {
        float originalWeight = _runtimeWeights[targetCard];
        float deductedWeight = Mathf.Ceil(originalWeight / 4f);

        // ������ʱ�����ϸ���
        var cards = new List<CardDataSO>(_runtimeWeights.Keys);
        int validTargets = cards.Count - 1;

        if (validTargets > 0)
        {
            float delta = deductedWeight / validTargets;

            // ������ʱ����
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

    // ���÷���
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

        //��ͼ��ʧ����
        intentionOBJ.transform.DOScale(Vector3.zero, 1f).OnComplete(() =>
        {
            intentionOBJ.SetActive(false);

            //TODO:Ч��ִ��
            if(intentionState==false)
            {
                //Debug.Log("ʹ�ÿ�����");
                foreach (Effect effect in currentIntention.effects_0)
                {
                    //Debug.Log(effect.name);
                    effect.Execute(this,player);
                }
            }
            else if(intentionState==true)
            {
                //Debug.Log("ʹ�ÿ�����");
                foreach (Effect effect in currentIntention.effects_1)
                {
                    //Debug.Log(effect.name);
                    effect.Execute(this, player);
                }
            }

            
                // ʹ�� DOTween ʵ����Ч��
                Vector3 originalPosition = characterSelf.transform.position;
                float shakeOffset = 0.3f; // ��ƫ����
                float shakeDuration = 0.2f; // �𶯳���ʱ��

                // �����𶯲��ص�ԭλ
                characterSelf.transform.DOMoveX(originalPosition.x - shakeOffset, shakeDuration)
                    .SetLoops(2, LoopType.Yoyo) // ������
                    .SetEase(Ease.InOutQuad);  // ƽ���Ļ���
            
            
        });
    }

    public virtual void OnEnemyTurnEnd()
    {
        currentIntention = GetRandomIntention();
        //Debug.Log("Enemyѡ���ƣ�" + currentIntention.name);

        intentionImage.sprite = currentIntention.cardImage0;
        intentionOBJ.SetActive(true);
        intentionOBJ.transform.DOScale(Vector3.one, 1f);
    }
}
