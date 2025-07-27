using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using static UnityEngine.Rendering.DebugUI;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;
using System.Collections.Concurrent;
using System.Threading;
using System.Collections.Generic;

public class HealthBarController : MonoBehaviour
{
    private CharacterBase currentCharacter;

    [Header("UI")]
    public Image healthBar;
    public GameObject hpText;
    private Image healthBarImage;
    public TextMeshProUGUI defenseAmountText;
    public GameObject defenseObj;

    [Header("Buff")]
    public GameObject strengthGetBuff;
    public TextMeshProUGUI strengthGetRound;
    public GameObject strengthLoseBuff;
    public TextMeshProUGUI strengthLoseRound;

    [SerializeField]
    private bool isStart=false;

    HashSet<GameObject> BuffHash = new();

    //private Enemy enemy;
    //private VisualElement intentSprite;
    //private Label intentAmount;


    private void Awake()
    {
        currentCharacter = GetComponent<CharacterBase>();

        //enemy = GetComponent<Enemy>();
    }

    private void OnEnable()
    {
        // 获取进度条的Image组件
        healthBarImage = healthBar.GetComponent<Image>();
        InitHealthBar();
    }

    private void Start()
    {
        IntoEnemyRoom();
    }

    private void OnDisable()
    {
        isStart = false;
    }

    [ContextMenu("Get UI Position")]
    public void InitHealthBar()
    {
        defenseObj.SetActive(false);
    }

    private void Update()
    {
        //UpdateHealthBar();
        //TODO:性能开支过大，后续优化成数据产生变化时调起事件，监听到相关事件才调用
    }

    public void IntoEnemyRoom()
    {
        isStart = true;
        UpdateHealthBar();
    }

    public void UpdateHealthBar()
    {
        if (!isStart)
            return;

        if (healthBar != null)
        {
            float healthPercentage = (float)currentCharacter.CurrentHP / (float)currentCharacter.MaxHp;
            healthBarImage.DOKill(); // 停止之前的动画
            healthBarImage.DOFillAmount(healthPercentage, 0.5f).From().SetUpdate(true);

            // 判断血量百分比并应用颜色过渡
            if (healthPercentage < 0.6f)
            {
                // 在50%到20%之间，从白色渐变到红色
                Color targetColor = healthPercentage < 0.2f ? Color.red : Color.Lerp(Color.white, Color.red, (0.6f - healthPercentage) / 0.4f);

                // 使用DOTween做颜色过渡
                healthBarImage.DOKill();
                healthBarImage.DOColor(targetColor, 0.5f);
            }
            else
            {
                // 如果血量大于50%，设置为白色
                healthBarImage.DOKill();
                healthBarImage.DOColor(Color.white, 0.5f);
            }

            hpText.GetComponent<TextMeshProUGUI>().text = $"{currentCharacter.CurrentHP}/{currentCharacter.MaxHp}";
        }

        //更新defense
        if (currentCharacter.defense.currentValue > 0)
        {
            defenseObj.SetActive(true);
            defenseAmountText.GetComponent<TextMeshProUGUI>().text = currentCharacter.defense.currentValue.ToString();
        }
        else
        {
            defenseObj.SetActive(false);
        }

        //buff回合更新
        if (currentCharacter.buffRound_StrengthGetEffect.currentValue != 0)
        {
            // 直接尝试添加，若成功返回true
            if (BuffHash.Add(strengthGetBuff))
            {
                strengthGetBuff.transform.SetAsLastSibling();
            }

            strengthGetBuff.SetActive(true);
            strengthGetRound.text = currentCharacter.buffRound_StrengthGetEffect.currentValue.ToString();
        }
        else
        {
            strengthGetBuff.SetActive(false);
            BuffHash.Remove(strengthGetBuff);
        }


        if (currentCharacter.buffRound_StrengthLoseEffect.currentValue != 0)
        {
            // 直接尝试添加，若成功返回true
            if (BuffHash.Add(strengthLoseBuff))
            {
                strengthLoseBuff.transform.SetAsLastSibling();
            }

            strengthLoseBuff.SetActive(true);
            strengthLoseRound.text = currentCharacter.buffRound_StrengthLoseEffect.currentValue.ToString();
        }
        else
        {
            strengthLoseBuff.SetActive(false);
            BuffHash.Remove(strengthLoseBuff);
        }

    }

    /// <summary>
    /// 在玩家回合开始时调用，显示敌人行动的意图
    /// </summary>
    //public void SetIntentElement()
    //{
    //    intentSprite.style.display = DisplayStyle.Flex;
    //    intentSprite.style.backgroundImage = new StyleBackground(enemy.currentAction.intentSprite);

    //    //判断是否是攻击
    //    var value = enemy.currentAction.effect.value;
    //    if (enemy.currentAction.effect.GetType() == typeof(DamageEffect))
    //    {
    //        value = (int)math.round(enemy.currentAction.effect.value * enemy.baseStrength);
    //    }

    //    intentAmount.text = value.ToString();
    //}

    /// <summary>
    /// 敌人回合结束后
    /// </summary>
    public void HideIntentElement()
    {
        //intentSprite.style.display = DisplayStyle.None;
    }
}
