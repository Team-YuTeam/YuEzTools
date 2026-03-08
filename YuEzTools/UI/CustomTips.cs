using System.Collections;
using TMPro;
using UnityEngine;
using BepInEx.Unity.IL2CPP.Utils;

namespace YuEzTools.UI;

public class CustomTips : MonoBehaviour
{
    public static CustomTips Instance;
    private GameObject currentTip;
    private float timeOnScreen;
    private Vector3 targetPosition;
    private Coroutine currentCoroutine;
    
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
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
            currentCoroutine = null;
        }
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
        
        targetPosition = currentTip.transform.localPosition;
        var startPos = targetPosition + new Vector3(0f, 2f, 0f);
        currentTip.transform.localPosition = startPos;
        
        timeOnScreen = 5f;
        currentCoroutine = this.StartCoroutine(AnimateTip());
        Info("Show Tips: " + tx, "CustomTips");
    }
    
    private IEnumerator AnimateTip()
    {
        float duration = 0.3f;
        float elapsed = 0f;
        var startPos = currentTip.transform.localPosition;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            t = 1f - (1f - t) * (1f - t) * (1f - t);
            currentTip.transform.localPosition = Vector3.Lerp(startPos, targetPosition, t);
            yield return null;
        }
        currentTip.transform.localPosition = targetPosition;
        
        while (timeOnScreen > 0f)
        {
            timeOnScreen -= Time.deltaTime;
            yield return null;
        }
        
        if (currentTip != null)
        {
            elapsed = 0f;
            startPos = currentTip.transform.localPosition;
            var endPos = startPos + new Vector3(0f, 2f, 0f);
            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                t = t * t * t;
                currentTip.transform.localPosition = Vector3.Lerp(startPos, endPos, t);
                yield return null;
            }
            
            Destroy(currentTip);
            currentTip = null;
        }
        
        currentCoroutine = null;
    }
}
