using UnityEngine;

[CreateAssetMenu(fileName = "DrawCardEffect", menuName = "Card Effect/Draw Card Effect")]

public class DrawCardEffect : Effect
{
    public IntEventSO drawCardEvent;

    public override void Execute(CharacterBase from, CharacterBase target)
    {
        drawCardEvent?.RaiseEvent(value, this);
    }
}
