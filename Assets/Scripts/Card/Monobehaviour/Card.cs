using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using DG.Tweening;

public class Card : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("组件")]
    public SpriteRenderer cardSprite0;
    public SpriteRenderer cardSprite1;
    public TextMeshPro costText0, descriptionText0, cardNameText0;
    public TextMeshPro costText1, descriptionText1, cardNameText1;
    public GameObject Card0;
    public GameObject Card1;
    public CardDataSO cardData0;
    public CardDataSO cardData1;
    public Transform targetTransform;

    [Header("原始数据")]
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private int originalLayerOrder;

    private SortingGroup sortingGroup;

    public bool isAnimating;
    public bool isAvailable;
    public int cardState;

    public Player player;

    [Header("广播事件")]
    public ObjectEventSO discardCardEvent;
    public IntEventSO costEvent;
    public ObjectEventSO clickByMouse1Event;//右键点击事件
    public IntEventSO cardUsedEvent;//广播使用的卡牌的阴阳属性

    private bool isUnderMouse;

    private void Awake()
    {
        sortingGroup = GetComponent<SortingGroup>();
        Init(cardData0, cardData1);
        isAvailable=true;
    }

    private void Start()
    {
        
    }

    private void Update()
    {

        if (Input.GetMouseButtonDown(1) && isUnderMouse)
        {
            clickByMouse1Event.RaiseEvent(this, this);
            Debug.Log("右键单击");
        }
    }

    public void Init(CardDataSO data0, CardDataSO data1)
    {
        cardData0 = data0;
        cardData1 = data1;
        cardState = Random.Range(0, 2);

        if (cardState == 0)
        {
            Card1.SetActive(false);
            Card0.SetActive(true);
            costText0.text = data0.cardCost_0.ToString();
            descriptionText0.text = data0.cardDescription_0;
            cardSprite0.sprite = data0.cardImage0;
            cardNameText0.text = data0.cardName0;
        }
        else
        {
            Card0.SetActive(false);
            Card1.SetActive(true);
            costText1.text = data1.cardCost_1.ToString();
            descriptionText1.text = data1.cardDescription_1;
            cardSprite1.sprite = data1.cardImage1;
            cardNameText1.text = data1.cardName1;
        }

        player = GameObject.FindWithTag("Player").GetComponent<Player>();
    }

    public void UpdatePositionRotation(Vector3 position, Quaternion rotation)
    {
        originalPosition = position;
        originalRotation = rotation;
        originalLayerOrder = GetComponent<SortingGroup>().sortingOrder;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isAnimating) return;

        isUnderMouse = true;

            // 提升 sortingOrder 确保在最上层
            sortingGroup.sortingOrder = 100;

            // 使用 DoTween 让卡牌平滑上移
            targetTransform.DOMove(originalPosition + Vector3.up * 0.5f, 0.1f)
                .SetEase(Ease.OutQuad);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isAnimating) return;
        //CardManager.Instance.cardsBeenClicked.Remove(this);
        RestCardTransform();
    }

    public void RestCardTransform()
    {
        isUnderMouse = false;

        // 恢复原来的 sortingOrder
        sortingGroup.sortingOrder = originalLayerOrder;

        // 使用 DoTween 让卡牌平滑回到原位
        targetTransform.DOMove(originalPosition, 0.1f)
            .SetEase(Ease.OutQuad);
    }

    public void ExecuteCardEffects(CharacterBase from, CharacterBase target)
    {
        // 使用 DOTween 实现震动效果
        Vector3 originalPosition = player.transform.position;
        float shakeOffset = 0.3f; // 震动偏移量
        float shakeDuration = 0.2f; // 震动持续时间

        // 向左震动并回到原位
        player.transform.DOMoveX(originalPosition.x + shakeOffset, shakeDuration)
            .SetLoops(2, LoopType.Yoyo) // 往返震动
            .SetEase(Ease.InOutQuad);  // 平滑的缓动

        if (cardState == 0)
        {
            costEvent.RaiseEvent(cardData0.cardCost_0, this);
            cardUsedEvent.RaiseEvent(0, this);
        }
        else if (cardState == 1)
        {
            costEvent.RaiseEvent(cardData1.cardCost_1, this);
            cardUsedEvent.RaiseEvent(1, this);
        }

        discardCardEvent.RaiseEvent(this, this);

        if(cardState == 0)
            foreach (var effect in cardData0.effects_0)
        {
            effect.Execute(from, target);
        } 

        else if(cardState == 1)
            foreach (var effect in cardData1.effects_1)
        {
            effect.Execute(from, target);
        }
    }

    public void UpdateCardState()
    {
        //卡牌能量判断
        if (cardState == 0)
        {
            var cardCost = cardData0.cardCost_0;
            isAvailable = cardCost <= player.CurrentMP;
        }
        else if (cardState == 1)
        {
            var cardCost = cardData0.cardCost_1;
            isAvailable = cardCost <= player.CurrentMP;
        }
    }
}
