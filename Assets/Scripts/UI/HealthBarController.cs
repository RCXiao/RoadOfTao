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
        // ��ȡ��������Image���
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
        //TODO:���ܿ�֧���󣬺����Ż������ݲ����仯ʱ�����¼�������������¼��ŵ���
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
            healthBarImage.DOKill(); // ֹ֮ͣǰ�Ķ���
            healthBarImage.DOFillAmount(healthPercentage, 0.5f).From().SetUpdate(true);

            // �ж�Ѫ���ٷֱȲ�Ӧ����ɫ����
            if (healthPercentage < 0.6f)
            {
                // ��50%��20%֮�䣬�Ӱ�ɫ���䵽��ɫ
                Color targetColor = healthPercentage < 0.2f ? Color.red : Color.Lerp(Color.white, Color.red, (0.6f - healthPercentage) / 0.4f);

                // ʹ��DOTween����ɫ����
                healthBarImage.DOKill();
                healthBarImage.DOColor(targetColor, 0.5f);
            }
            else
            {
                // ���Ѫ������50%������Ϊ��ɫ
                healthBarImage.DOKill();
                healthBarImage.DOColor(Color.white, 0.5f);
            }

            hpText.GetComponent<TextMeshProUGUI>().text = $"{currentCharacter.CurrentHP}/{currentCharacter.MaxHp}";
        }

        //����defense
        if (currentCharacter.defense.currentValue > 0)
        {
            defenseObj.SetActive(true);
            defenseAmountText.GetComponent<TextMeshProUGUI>().text = currentCharacter.defense.currentValue.ToString();
        }
        else
        {
            defenseObj.SetActive(false);
        }

        //buff�غϸ���
        if (currentCharacter.buffRound_StrengthGetEffect.currentValue != 0)
        {
            // ֱ�ӳ�����ӣ����ɹ�����true
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
            // ֱ�ӳ�����ӣ����ɹ�����true
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
    /// ����һغϿ�ʼʱ���ã���ʾ�����ж�����ͼ
    /// </summary>
    //public void SetIntentElement()
    //{
    //    intentSprite.style.display = DisplayStyle.Flex;
    //    intentSprite.style.backgroundImage = new StyleBackground(enemy.currentAction.intentSprite);

    //    //�ж��Ƿ��ǹ���
    //    var value = enemy.currentAction.effect.value;
    //    if (enemy.currentAction.effect.GetType() == typeof(DamageEffect))
    //    {
    //        value = (int)math.round(enemy.currentAction.effect.value * enemy.baseStrength);
    //    }

    //    intentAmount.text = value.ToString();
    //}

    /// <summary>
    /// ���˻غϽ�����
    /// </summary>
    public void HideIntentElement()
    {
        //intentSprite.style.display = DisplayStyle.None;
    }
}
