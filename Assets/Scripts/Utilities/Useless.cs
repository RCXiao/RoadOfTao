//TODO:public Player player;

//[Header("¹ã²¥ÊÂ¼þ")]
//public ObjectEventSO discardCardEvent;
//TODO:public IntEventSO costEvent;

//TODO:player = GameObject.FindWithTag("Player").GetComponent<Player>();


//TODO:

//public void ExecuteCardEffects(CharacterBase from, CharacterBase target)
//{
//    costEvent.RaiseEvent(cardData.cardCost, this);
//    discardCardEvent.RaiseEvent(this, this);

//    foreach (var effect in cardData.effects)
//    {
//        effect.Execute(from, target);
//    }
//}

//public void UpdataCardState()
//{
//    isAvailiable = cardData.cardCost <= player.CurrentMana;
//    costText.color = isAvailiable ? Color.green : Color.red;
//}

//public void UpdatePositionRotation(Vector3 position, Quaternion rotation)
//{
//    originalPosition = position;
//    originalRotation = rotation;
//    originalLayerOrder = GetComponent<SortingGroup>().sortingOrder;
//}

//public void OnPointerEnter(PointerEventData eventData)
//{
//    if (isAnimating) return;
//    targetTransform.position = originalPosition + Vector3.up;
//    targetTransform.rotation = Quaternion.identity;
//    GetComponent<SortingGroup>().sortingOrder = 100;
//}

//public void OnPointerExit(PointerEventData eventData)
//{
//    if (isAnimating) return;

//    RestCardTransfrom();
//}

//public void RestCardTransfrom()
//{
//    targetTransform.SetPositionAndRotation(originalPosition, originalRotation);
//    GetComponent<SortingGroup>().sortingOrder = originalLayerOrder;
//}