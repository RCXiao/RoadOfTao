using UnityEngine;

[CreateAssetMenu(fileName = "StrengthLoseEffect", menuName = "Card Effect/StrengthLoseEffect")]
public class StrengthLoseEffect : Effect
{
    public override void Execute(CharacterBase from, CharacterBase target)
    {
        switch (targetType)
        {
            case EffectTargetType.Self:
                from.SetupStrength(value, false);
                break;

            case EffectTargetType.Target:
                target.SetupStrength(value, false);
                break;

            case EffectTargetType.enemy:
                foreach (var enemy in GameObject.FindGameObjectsWithTag("Enemy"))
                {
                    enemy.GetComponent<CharacterBase>().SetupStrength(value, false); 
                }

                break;

            case EffectTargetType.ALL:
                foreach (var enemy in GameObject.FindGameObjectsWithTag("Enemy"))
                {
                    enemy.GetComponent<CharacterBase>().SetupStrength(value, false);
                }

                foreach (var player in GameObject.FindGameObjectsWithTag("Player"))
                {
                    player.GetComponent<CharacterBase>().SetupStrength(value, false);
                }
                break;

            default:
                break;

        }
    }
}
