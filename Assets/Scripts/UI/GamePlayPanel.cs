using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GamePlayPanel : MonoBehaviour
{
    [Header("事件广播")]
    public ObjectEventSO playerTurnEndEvent;
    public IntEventSO yinBarFillAmountEvent;

    public TextMeshProUGUI energyAmountText, drawAmountText, discardAmountText;
    public Button nextTurnButton;
    private bool isPlayerTurn;//TODO:初始化
    public Image yinBar;

    public GameObject showCardPanel;
    public Image cardImage0;
    public Image cardImage1;
    public TextMeshProUGUI cardName0;
    public TextMeshProUGUI cardName1;
    public TextMeshProUGUI cardCost0;
    public TextMeshProUGUI cardCost1;
    public TextMeshProUGUI cardDescription0;
    public TextMeshProUGUI cardDescription1;


    private void OnEnable()
    {
        drawAmountText.text = "0";
        discardAmountText.text = "0";
        energyAmountText.text = "0";
        nextTurnButton.transform.rotation = Quaternion.Euler(0, 0, 0);

        nextTurnButton.onClick.AddListener(() =>
        {
            if(isPlayerTurn)
            {
                playerTurnEndEvent.RaiseEvent(null,this);
            }
        });
    }

    public void UpdateDrawDeckAmount(int amount)
    {
        drawAmountText.text = amount.ToString();
    }

    public void UpdateDiscardDeckAmount(int amount)
    {
        discardAmountText.text = amount.ToString();
    }

    public void UpdateEnergyAmount(int amount)
    {
        energyAmountText.text = amount.ToString();
    }

    public void UpdateYinBar(int state)
    {
        if(state == 0 && yinBar.fillAmount > 0)
        {
            yinBar.DOFillAmount(yinBar.fillAmount - 0.125f, 0.5f).SetEase(Ease.Linear).OnComplete(() =>
            {
                yinBarFillAmountEvent.RaiseEvent((int)(yinBar.fillAmount * 100), this);
            });
        }

        else if(state == 1 && yinBar.fillAmount < 1)
        {
            yinBar.DOFillAmount(yinBar.fillAmount + 0.125f, 0.5f).SetEase(Ease.Linear).OnComplete(() =>
            {
                yinBarFillAmountEvent.RaiseEvent((int)(yinBar.fillAmount * 100), this);
            });
        }
    }

    public void PlayerTurnBegin()
    {
        nextTurnButton.transform.rotation = Quaternion.Euler(0, 0, 180);

        // 创建动画序列
        Sequence seq = DOTween.Sequence();

        // 旋转动画
        seq.Append(nextTurnButton.transform.DORotate(new Vector3(0, 0, 0), 1f, RotateMode.FastBeyond360)
            .SetEase(Ease.OutQuad));

        // 放大动画
        seq.Join(nextTurnButton.transform.DOScale(0.45f, 0.5f).SetEase(Ease.OutQuad));

        // 复原大小
        seq.Append(nextTurnButton.transform.DOScale(0.4f, 0.5f).SetEase(Ease.InQuad));

        isPlayerTurn = true;

        yinBar.DOFillAmount(0.5f, 1.5f).SetEase(Ease.Linear).OnComplete(() =>
        {
            yinBarFillAmountEvent.RaiseEvent((int)(yinBar.fillAmount * 100), this);
        });
    }

    public void EnemyTurnBegin()
    {
        isPlayerTurn = false;

        // 创建动画序列
        Sequence seq = DOTween.Sequence();

        // 旋转动画
        seq.Append(nextTurnButton.transform.DORotate(new Vector3(0, 0, -180), 1f, RotateMode.FastBeyond360)
            .SetEase(Ease.OutQuad));

        // 放大动画
        seq.Join(nextTurnButton.transform.DOScale(0.45f, 0.5f).SetEase(Ease.OutQuad));

        // 复原大小
        seq.Append(nextTurnButton.transform.DOScale(0.4f, 0.5f).SetEase(Ease.InQuad));
    }

    public void ShowCardPanel(object obj)
    {
        Card card = (Card)obj;

        cardImage0.sprite = card.cardData0.cardImage0;
        cardImage1.sprite = card.cardData1.cardImage1;
        cardName0.text = card.cardData0.cardName0;
        cardName1.text = card.cardData1.cardName1;
        cardDescription0.text = card.cardData0.cardDescription_0;
        cardDescription1.text = card.cardData1.cardDescription_1;
        cardCost0.text = card.cardData0.cardCost_0.ToString();
        cardCost1.text = card.cardData1.cardCost_1.ToString();

        showCardPanel.SetActive(true);
    }
}
