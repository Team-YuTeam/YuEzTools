using TMPro;
using YuEzTools.Modules;
using YuEzTools.Utils;

namespace YuEzTools.Patches;

[HarmonyPatch(typeof(LobbyInfoPane), nameof(LobbyInfoPane.Update))]
public class LobbyInfoPanePatch
{
    private static void Postfix(LobbyInfoPane __instance)
    {
        var aspectSize = __instance.CopyCodeButton.transform.parent.parent;
        var modeLabel_TMP = aspectSize.Find("ModeLabel").Find("Text_TMP").GetComponent<TextMeshPro>();
        var modeValue_TMP = aspectSize.Find("ModeValue").Find("GameModeText").GetComponent<TextMeshPro>();
        modeLabel_TMP.text = GetString(Toggles.ShowServer ? "ServerLabel" : "ModeLabel");
        if (Toggles.ShowServer)
            modeValue_TMP.text =
                GetPlayer.IsOnlineGame ? PingTrackerUpdatePatch.ServerName : $"<color=#D3D3D3>{GetString("Local")}</color>";
    }
}