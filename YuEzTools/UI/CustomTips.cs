using TMPro;
using UnityEngine;

namespace YuEzTools.UI;

public class CustomTips : MonoBehaviour
{
    public static CustomTips Instance; // 单例
    private GameObject tip; // 改为实例变量
    private float timeOnScreen; // 改为实例变量
    
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    
    public static void Show(string tx,TipsCode tc = TipsCode.ModLogo,Sprite sr = null)
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
        // 通过单例调用实例方法
        Instance.ShowTip(tx, sr);
    }
    
    private void ShowTip(string tx, Sprite sr)
    {
        // var popup = FriendsListManager.Instance.transform.Find("FriendListConfirmScreen");
        // var tmp = popup.Find("Dialogue_TMP").GetComponent<TextMeshPro>();
        // tmp.text = "卧槽";
        // popup.gameObject.SetActive(true);

        var origingoj = FriendsListManager.Instance.transform.Find("FriendListNotification").gameObject;
        
        tip = Object.Instantiate(origingoj);
        tip.gameObject.name = "YuETCustomTip";
        Destroy(tip.transform.GetComponent<FriendListNotification>()); 
        
        var text = tip.transform.Find("Text").GetComponent<TextMeshPro>();
        text.text = tx;
        
        var icon = tip.transform.Find("IconHolder").Find("Icon").GetComponent<SpriteRenderer>();
        icon.sprite = sr;
        tip.SetActive(true);
        timeOnScreen = 5f;
        Info("Show Tips" + text,"CustomTips");
    }
    
    private void Update()
    {
        if (timeOnScreen <= 0f) return;
        
        timeOnScreen -= Time.deltaTime;
        if (timeOnScreen <= 0f)
        {
            tip?.SetActive(false);
        }
    }
}
