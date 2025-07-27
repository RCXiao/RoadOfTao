using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using DG.Tweening;

public class Card : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("���")]
    public SpriteRenderer cardSprite0;
    public SpriteRenderer cardSprite1;
    public TextMeshPro costText0, descriptionText0, cardNameText0;
    public TextMeshPro costText1, descriptionText1, cardNameText1;
    public GameObject Card0;
    public GameObject Card1;
    public CardDataSO cardData0;
    public CardDataSO cardData1;
    public Transform targetTransform;

    [Header("ԭʼ����")]
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private int originalLayerOrder;

    private SortingGroup sortingGroup;

    public bool isAnimating;
    public bool isAvailable;
    public int cardState;

    public Player player;

    [Header("�㲥�¼�")]
    public ObjectEventSO discardCardEvent;
    public IntEventSO costEvent;
    public ObjectEventSO clickByMouse1Event;//�Ҽ�����¼�
    public IntEventSO cardUsedEvent;//�㲥ʹ�õĿ��Ƶ���������

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
            Debug.Log("�Ҽ�����");
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

            // ���� sortingOrder ȷ�������ϲ�
            sortingGroup.sortingOrder = 100;

            // ʹ�� DoTween �ÿ���ƽ������
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

        // �ָ�ԭ���� sortingOrder
        sortingGroup.sortingOrder = originalLayerOrder;

        // ʹ�� DoTween �ÿ���ƽ���ص�ԭλ
        targetTransform.DOMove(originalPosition, 0.1f)
            .SetEase(Ease.OutQuad);
    }

    public void ExecuteCardEffects(CharacterBase from, CharacterBase target)
    {
        // ʹ�� DOTween ʵ����Ч��
        Vector3 originalPosition = player.transform.position;
        float shakeOffset = 0.3f; // ��ƫ����
        float shakeDuration = 0.2f; // �𶯳���ʱ��

        // �����𶯲��ص�ԭλ
        player.transform.DOMoveX(originalPosition.x + shakeOffset, shakeDuration)
            .SetLoops(2, LoopType.Yoyo) // ������
            .SetEase(Ease.InOutQuad);  // ƽ���Ļ���

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
        //���������ж�
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
