using UnityEngine;
using UnityEngine.UI;

public class GameWinPanel : MonoBehaviour
{
    public Button pickCardButton;
    public Button backToMapButton;

    [Header("ÊÂ¼þ¹ã²¥")]
    public ObjectEventSO loadMapEvent;
    //public ObjectEventSO pickCardEvent;

    private void Awake()
    {
        pickCardButton.gameObject.SetActive(true);

        backToMapButton.onClick.AddListener(() =>
        {
            loadMapEvent.RaiseEvent(null, this);
        });

        pickCardButton.onClick.AddListener(() =>
        {
            //pickCardEvent.RaiseEvent(null, this);
        });
    }
}
