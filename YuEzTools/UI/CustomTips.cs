using TMPro;
using UnityEngine;

namespace YuEzTools.UI;

public class CustomTips : MonoBehaviour
{
    public static CustomTips Instance;
    private GameObject currentTip;
    private float timeOnScreen;
    
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    
    public static void Show(string tx, TipsCode tc = TipsCode.ModLogo, Sprite sr = null)
    {
        if (sr == null) sr = LoadSprite("YuEzTools.Resources.Yu-Logo-tm.png", 200f);
        switch (tc)
        {
            case TipsCode.Error:
                sr = LoadSprite("YuEzTools.Resources.RedWarning.png", 200f);
                break;
            case TipsCode.Warn:
                sr = LoadSprite("YuEzTools.Resources.YellowWarning.png", 200f);
                break;
            case TipsCode.Info:
                sr = LoadSprite("YuEzTools.Resources.Tips.png", 200f);
                break;
            case TipsCode.AntiCheat:
                sr = LoadSprite("YuEzTools.Resources.AntiCheatLogo.png", 200f);
                break;
        }
        Instance.ShowTip(tx, sr);
    }
    
    private void ShowTip(string tx, Sprite sr)
    {
        if (currentTip != null)
        {
            Destroy(currentTip);
            currentTip = null;
        }

        var origingoj = FriendsListManager.Instance.transform.Find("FriendListNotification").gameObject;
        
        currentTip = Instantiate(origingoj);
        currentTip.name = "YuETCustomTip";
        Destroy(currentTip.transform.GetComponent<FriendListNotification>());
        
        var text = currentTip.transform.Find("Text").GetComponent<TextMeshPro>();
        text.text = tx;
        
        var icon = currentTip.transform.Find("IconHolder").Find("Icon").GetComponent<SpriteRenderer>();
        icon.sprite = sr;
        currentTip.SetActive(true);
        timeOnScreen = 5f;
        Info("Show Tips: " + tx, "CustomTips");
    }
    
    private void Update()
    {
        if (currentTip == null || timeOnScreen <= 0f) return;
        
        timeOnScreen -= Time.deltaTime;
        if (timeOnScreen <= 0f)
        {
            if (currentTip != null)
            {
                Destroy(currentTip);
                currentTip = null;
            }
        }
    }
}
