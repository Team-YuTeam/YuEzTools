using HarmonyLib;
using TMPro;
using UnityEngine;

namespace YuEzTools;

[HarmonyPatch(typeof(AccountTab), nameof(AccountTab.Awake))]
public static class UpdateFriendCodeUIPatch
{
    private static GameObject VersionShower;
    public static void Prefix(AccountTab __instance)
    {

        string credentialsText = "<color=#FFB6C1>YuTeam \u00a9 2025</color>";
        credentialsText += "\t\t\t";
        string versionText = $"<color={Main.ModColor}>{Main.ModName}</color> - <color=#ffff00>v{Main.PluginVersion}</color>";

#if CANARY
        versionText = $"<color=#cdfffd>{Main.ModName}</color> - {ThisAssembly.Git.Commit}";
#endif

#if DEBUG
        versionText = $"<color=#cdfffd>{ThisAssembly.Git.Branch}</color> - {ThisAssembly.Git.Commit}";
#endif

        credentialsText += versionText;

        var friendCode = GameObject.Find("FriendCode");
        if (friendCode != null && VersionShower == null)
        {
            VersionShower = Object.Instantiate(friendCode, friendCode.transform.parent);
            VersionShower.name = "YuET Version Shower";
            VersionShower.transform.localPosition = friendCode.transform.localPosition + new Vector3(2.8f, 0f, 0f);
            VersionShower.transform.localScale *= 1.7f;
            var TMP = VersionShower.GetComponent<TextMeshPro>();
            TMP.alignment = TextAlignmentOptions.Right;
            TMP.fontSize = 30f;
            TMP.SetText(credentialsText);
        }

        var newRequest = GameObject.Find("NewRequest");
        if (newRequest != null)
        {
            newRequest.transform.localPosition -= new Vector3(0f, 0f, 10f);
            newRequest.transform.localScale = new Vector3(0.8f, 1f, 1f);
        }

        var BarSprite = GameObject.Find("BarSprite");
        var SignInStatus = GameObject.Find("SignInStatus");
        var Toggle_Friend_Code_Button = GameObject.Find("Toggle Friend Code Button");
        BarSprite.SetActive(false);
        friendCode.SetActive(false);
        SignInStatus.SetActive(false);
        GameObject.Destroy(Toggle_Friend_Code_Button);
        Toggle_Friend_Code_Button.SetActive(false);
        //Toggle_Friend_Code_Button.transform.localPosition = new Vector3(114514f, 1919810f, 1f);
    }
}