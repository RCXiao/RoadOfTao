using UnityEngine;

[CreateAssetMenu(fileName = "HealEffect", menuName = "Card Effect/HealEffect")]

public class HealEffect : Effect
{
    public override void Execute(CharacterBase from, CharacterBase target)
    {
        switch (targetType)
        {
            case EffectTargetType.Self:
                from.HealHealth(value);
                //Debug.Log(from.name + " healed themselves for " + value + " health points.");
                break;

            case EffectTargetType.Target:
                target.HealHealth(value);
                break;

            case EffectTargetType.enemy:
                foreach (var enemy in GameObject.FindGameObjectsWithTag("Enemy"))
                {
                    enemy.GetComponent<CharacterBase>().HealHealth(value);
                }
                break;

            case EffectTargetType.ALL:
                foreach (var enemy in GameObject.FindGameObjectsWithTag("Enemy"))
                {
                    enemy.GetComponent<CharacterBase>().HealHealth(value);
                }

                foreach (var player in GameObject.FindGameObjectsWithTag("Player"))
                {
                    player.GetComponent<CharacterBase>().HealHealth(value);
                }
                break;

            default:
                break;
        }
    }
}
