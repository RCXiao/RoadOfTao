using UnityEngine;

[CreateAssetMenu(fileName = "DefenseEffect", menuName = "Card Effect/DefenseEffect")]

public class DefenseEffect : Effect
{
    public override void Execute(CharacterBase from, CharacterBase target)
    {
        switch (targetType)
        {
            case EffectTargetType.Self:
                from.UpdateDefense(value);
                break;

            case EffectTargetType.Target:
                target.UpdateDefense(value);
                break;

            case EffectTargetType.enemy:
                foreach (var enemy in GameObject.FindGameObjectsWithTag("Enemy"))
                {
                    enemy.GetComponent<CharacterBase>().UpdateDefense(value);
                }
                break;

            case EffectTargetType.ALL:
                foreach (var enemy in GameObject.FindGameObjectsWithTag("Enemy"))
                {
                    enemy.GetComponent<CharacterBase>().UpdateDefense(value);
                }

                foreach (var player in GameObject.FindGameObjectsWithTag("Player"))
                {
                    player.GetComponent<CharacterBase>().UpdateDefense(value);
                }
                break;

            default:
                break;
        }
    }
}
