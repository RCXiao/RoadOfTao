using UnityEngine;

[CreateAssetMenu(fileName = "DamageEffect", menuName = "Card Effect/DamageEffect")]

public class DamageEffect : Effect
{
    public override void Execute(CharacterBase from, CharacterBase target)
    {
        switch (targetType)
        {
            case EffectTargetType.Self:
                from.TakeDamage((int)Mathf.Floor(from.baseStrength * value));
                Debug.Log("Damage effect to self");
                break;

            case EffectTargetType.Target:
                target.TakeDamage((int)Mathf.Floor(from.baseStrength * value));
                Debug.Log("Damage effect to target");
                break;

            case EffectTargetType.enemy:
                foreach (var enemy in GameObject.FindGameObjectsWithTag("Enemy"))
                {
                    enemy.GetComponent<CharacterBase>().TakeDamage((int)Mathf.Floor(from.baseStrength * value));
                }
                break;

            case EffectTargetType.ALL:
                foreach (var enemy in GameObject.FindGameObjectsWithTag("Enemy"))
                {
                    enemy.GetComponent<CharacterBase>().TakeDamage((int)Mathf.Floor(from.baseStrength * value));
                }
                foreach (var player in GameObject.FindGameObjectsWithTag("Player"))
                {
                    player.GetComponent<CharacterBase>().TakeDamage((int)Mathf.Floor(from.baseStrength * value));
                }
                break;

            default:
                break;
        }
    }
}