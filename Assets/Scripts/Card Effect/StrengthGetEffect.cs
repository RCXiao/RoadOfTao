using UnityEngine;

[CreateAssetMenu(fileName = "StrengthGetEffect", menuName = "Card Effect/StrengthGetEffect")]
public class StrengthGetEffect : Effect
{
    public override void Execute(CharacterBase from, CharacterBase target)
    {
        switch (targetType)
        {
            case EffectTargetType.Self:
                from.SetupStrength(value, true);
                break;

            case EffectTargetType.Target:
                target.SetupStrength(value, true);
                break;

            case EffectTargetType.enemy:
                foreach (var enemy in GameObject.FindGameObjectsWithTag("Enemy"))
                {
                    enemy.GetComponent<CharacterBase>().SetupStrength(value, true); 
                }

                break;

            case EffectTargetType.ALL:
                foreach (var enemy in GameObject.FindGameObjectsWithTag("Enemy"))
                {
                    enemy.GetComponent<CharacterBase>().SetupStrength(value, true);
                }

                foreach (var player in GameObject.FindGameObjectsWithTag("Player"))
                {
                    player.GetComponent<CharacterBase>().SetupStrength(value, true);
                }
                break;

            default:
                break;

        }
    }
}
