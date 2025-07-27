using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public GameObject arrowPrefab;
    private GameObject currentArrow;

    private Card currentCard;

    private bool canMove;
    private bool canExecute;

    private CharacterBase targetCharacter;

    private void Awake()
    {
        currentCard = GetComponent<Card>();
    }

    private void OnDisable()
    {
        canMove = false;
        canExecute = false;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!currentCard.isAvailable)
        {
            return;
            //TODO:播放卡牌费用不足无法打出的音效
        }

        if (currentCard.cardState == 0)
        {
            switch (currentCard.cardData0.cardType_0)
            {
                case CardType.Target:
                    currentArrow = Instantiate(arrowPrefab, new Vector3(transform.position.x, transform.position.y + 0.5f, 0), Quaternion.identity);
                    break;
                case CardType.Self:
                case CardType.enemy:
                case CardType.ALL:
                    canMove = true;
                    break;
            }
        }
        else if (currentCard.cardState == 1)
        {
            switch (currentCard.cardData1.cardType_1)
            {
                case CardType.Target:
                    currentArrow = Instantiate(arrowPrefab, new Vector3(transform.position.x, transform.position.y + 0.5f, 0), Quaternion.identity);
                    break;
                case CardType.Self:
                case CardType.enemy:
                case CardType.ALL:
                    canMove = true;
                    break;
            }
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!currentCard.isAvailable)
        {
            return;
        }

        if (canMove)
        {
            currentCard.isAnimating = true;
            Vector3 screenPos = new(Input.mousePosition.x, Input.mousePosition.y, 10);
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);

            currentCard.targetTransform.transform.position = worldPos;
            canExecute = worldPos.y > 1f;
        }
        else
        {
            if (eventData.pointerEnter == null)
            {
                canExecute = false;
                targetCharacter = null;
                return;
            }

            if (eventData.pointerEnter.CompareTag("Enemy") || eventData.pointerEnter.CompareTag("Player"))
            {
                canExecute = true;
                targetCharacter = eventData.pointerEnter.GetComponent<CharacterBase>();
                return;
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!currentCard.isAvailable)
        {
            return;
        }

        if (currentArrow != null)
        {
            Destroy(currentArrow);
        }

        if (canExecute)
        {
            currentCard.ExecuteCardEffects(currentCard.player, targetCharacter);
        }
        else
        {
            currentCard.RestCardTransform();
            
            currentCard.isAnimating = false;

        }
    }
}
