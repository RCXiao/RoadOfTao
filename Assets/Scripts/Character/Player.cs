using UnityEngine;

public class Player : CharacterBase
{
    public IntVariable playerMP;
    public int maxMP;
    public int CurrentMP { get => playerMP.currentValue; set => playerMP.SetValue(value); }

    private void OnEnable()
    {
        playerMP.maxValue = maxMP;
        CurrentMP = playerMP.maxValue;//���ó�ʼ����ֵ
    }

    /// <summary>
    /// �����¼�����
    /// </summary>
    public void NewTurn()
    {
        CurrentMP = maxMP;
    }

    public void UpdataMP(int cost)
    {
        CurrentMP -= cost;
        if (CurrentMP <= 0)
        {
            CurrentMP = 0;
        }
    }

    public void NewGame()
    {
        CurrentHP = MaxHp;
        isDead = false;
        buffRound_StrengthGetEffect.currentValue = buffRound_StrengthGetEffect.maxValue;
        buffRound_StrengthLoseEffect.currentValue = buffRound_StrengthLoseEffect.maxValue;
        NewTurn();
        gameObject.SetActive(false);
    }
}
