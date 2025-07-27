using UnityEngine;
using DG.Tweening;

public class CharacterBase : MonoBehaviour
{
    public int maxHp;

    public GameObject characterSelf;
    public AudioSource audioSource;
    public AudioClip hitSound;
    public AudioClip defenseSound;

    public IntVariable hp;

    public IntVariable defense;

    public IntVariable buffRound_StrengthGetEffect;
    public IntVariable buffRound_StrengthLoseEffect;

    public int CurrentHP { get => hp.currentValue; set => hp.SetValue(value); }

    public int MaxHp { get => hp.maxValue; }

    protected Animator animator;

    public bool isDead;

    //力量
    public float baseStrength = 1f;
    private float strengthGetEffect = 0.5f;
    private float strengthLoseEffect = 0.3f;

    [Header("广播")]
    public ObjectEventSO characterDeadEvent;

    protected virtual void Awake()
    {

    }

    protected virtual void Start()
    {
        hp.maxValue = maxHp;
        CurrentHP = MaxHp;
        buffRound_StrengthGetEffect.currentValue = buffRound_StrengthGetEffect.maxValue;
        buffRound_StrengthLoseEffect.currentValue = buffRound_StrengthLoseEffect.maxValue;

        ResetDefense();
    }

    protected virtual void Update()
    {

    }

    public virtual void TakeDamage(int damage)
    {
        var currentDamage = (damage - defense.currentValue) >= 0 ? (damage - defense.currentValue) : 0;
        var currentDefense = (damage - defense.currentValue) >= 0 ? 0 : (defense.currentValue - damage);

        if(currentDefense > 0)
        {
            audioSource.PlayOneShot(defenseSound);
        }
        else if(currentDefense == 0)
        {
            audioSource.PlayOneShot(hitSound);

            if (this is Player)
            {
                // 使用 DOTween 实现震动效果
                Vector3 originalPosition = characterSelf.transform.position;
                float shakeOffset = 0.3f; // 震动偏移量
                float shakeDuration = 0.2f; // 震动持续时间

                // 向左震动并回到原位
                characterSelf.transform.DOMoveX(originalPosition.x - shakeOffset, shakeDuration)
                    .SetLoops(2, LoopType.Yoyo) // 往返震动
                    .SetEase(Ease.InOutQuad);  // 平滑的缓动
            }
            else if (this is Enemy)
            {
                // 使用 DOTween 实现震动效果
                Vector3 originalPosition = characterSelf.transform.position;
                float shakeOffset = 0.2f; // 震动偏移量
                float shakeDuration = 0.2f; // 震动持续时间

                // 向左震动并回到原位
                characterSelf.transform.DOMoveX(originalPosition.x + shakeOffset, shakeDuration)
                    .SetLoops(2, LoopType.Yoyo) // 往返震动
                    .SetEase(Ease.InOutQuad);  // 平滑的缓动
            }
        }

        defense.SetValue(currentDefense);

        if (CurrentHP > currentDamage)
        {
            CurrentHP -= currentDamage;
        }
        else
        {
            CurrentHP = 0;
            //当前人物死亡
            isDead = true;
            characterDeadEvent.RaiseEvent(this, this);//推送死亡事件
            Debug.Log("当前人物死亡");
        }
    }

    public void UpdateDefense(int amount)
    {
        var value = defense.currentValue + amount;
        defense.SetValue(value);
    }

    public void ResetDefense()
    {
        defense.SetValue(0);
    }

    public void HealHealth(int amount)
    {
        CurrentHP += amount;
        CurrentHP = Mathf.Min(CurrentHP, MaxHp);
        //TODO:buff特效 buff.SetActive(true);
    }

    public void SetupStrength(int round, bool isPositive)
    {
        if (isPositive)
        {
            if(buffRound_StrengthGetEffect.currentValue ==0)
            {
                baseStrength = baseStrength + strengthGetEffect;
            }
            var currentRound = buffRound_StrengthGetEffect.currentValue + round;
            buffRound_StrengthGetEffect.SetValue(currentRound);
            //TODO:buff特效 buff.SetActive(true);
        }
        else
        {
            if (buffRound_StrengthLoseEffect.currentValue == 0)
            {
                baseStrength = baseStrength - strengthLoseEffect;
            }
            var currentRound = buffRound_StrengthLoseEffect.currentValue + round;
            buffRound_StrengthLoseEffect.SetValue(currentRound);
            //TODO:buff特效 debuff.SetActive(true);
        }
    }

    //回合转换事件的函数
    public void TurnEndUpdateStrengthRound()
    {
        if(buffRound_StrengthGetEffect.currentValue==1)
        {

            baseStrength = baseStrength - strengthGetEffect;
        }

        buffRound_StrengthGetEffect.SetValue(buffRound_StrengthGetEffect.currentValue - 1);
        if (buffRound_StrengthGetEffect.currentValue <= 0)
        {
            buffRound_StrengthGetEffect.SetValue(0);
        }


        if (buffRound_StrengthLoseEffect.currentValue == 1)
        {
            baseStrength = baseStrength + strengthLoseEffect;
        }

        buffRound_StrengthLoseEffect.SetValue(buffRound_StrengthLoseEffect.currentValue - 1);
        if (buffRound_StrengthLoseEffect.currentValue <= 0)
        {
            buffRound_StrengthLoseEffect.SetValue(0);
        }
    }
}
