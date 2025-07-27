using UnityEngine;

[CreateAssetMenu(fileName = "GetEnergyEffect", menuName = "Card Effect/Get Energy Effect")]

public class GetEnergyEffect : Effect
{
    public override void Execute(CharacterBase from, CharacterBase target)
    {
        Player player = from as Player;

        player.CurrentMP += value;
    }
}
