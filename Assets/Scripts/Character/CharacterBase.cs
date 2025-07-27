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

    //����
    public float baseStrength = 1f;
    private float strengthGetEffect = 0.5f;
    private float strengthLoseEffect = 0.3f;

    [Header("�㲥")]
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
                // ʹ�� DOTween ʵ����Ч��
                Vector3 originalPosition = characterSelf.transform.position;
                float shakeOffset = 0.3f; // ��ƫ����
                float shakeDuration = 0.2f; // �𶯳���ʱ��

                // �����𶯲��ص�ԭλ
                characterSelf.transform.DOMoveX(originalPosition.x - shakeOffset, shakeDuration)
                    .SetLoops(2, LoopType.Yoyo) // ������
                    .SetEase(Ease.InOutQuad);  // ƽ���Ļ���
            }
            else if (this is Enemy)
            {
                // ʹ�� DOTween ʵ����Ч��
                Vector3 originalPosition = characterSelf.transform.position;
                float shakeOffset = 0.2f; // ��ƫ����
                float shakeDuration = 0.2f; // �𶯳���ʱ��

                // �����𶯲��ص�ԭλ
                characterSelf.transform.DOMoveX(originalPosition.x + shakeOffset, shakeDuration)
                    .SetLoops(2, LoopType.Yoyo) // ������
                    .SetEase(Ease.InOutQuad);  // ƽ���Ļ���
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
            //��ǰ��������
            isDead = true;
            characterDeadEvent.RaiseEvent(this, this);//���������¼�
            Debug.Log("��ǰ��������");
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
        //TODO:buff��Ч buff.SetActive(true);
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
            //TODO:buff��Ч buff.SetActive(true);
        }
        else
        {
            if (buffRound_StrengthLoseEffect.currentValue == 0)
            {
                baseStrength = baseStrength - strengthLoseEffect;
            }
            var currentRound = buffRound_StrengthLoseEffect.currentValue + round;
            buffRound_StrengthLoseEffect.SetValue(currentRound);
            //TODO:buff��Ч debuff.SetActive(true);
        }
    }

    //�غ�ת���¼��ĺ���
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
