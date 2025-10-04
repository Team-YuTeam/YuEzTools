using UnityEngine;
using YuEzTools.UI;

namespace YuEzTools.Patches;

[HarmonyPatch(typeof(MMOnlineManager), nameof(MMOnlineManager.Start))]
internal class MMOnlineManagerStartPatch
{
    public static void Postfix(MMOnlineManager __instance)
    {
        var HostGameButton = GameObject.Find("HostGameButton");
        if (HostGameButton && Toggles.ServerAllHostOrNoHost)
        {
            HostGameButton?.SetActive(false);
            var textObj = Object.Instantiate(HostGameButton.transform.FindChild("Text_TMP").GetComponent<TMPro.TextMeshPro>());
            var parentObj = HostGameButton.transform.parent.gameObject;
            textObj.transform.position = new Vector3(-0.7f, 1.53f ,0f);
            textObj.name = "CanNotHostGame";
            var message = $"<size=2>{Utils.Utils.ColorString(Color.red, GetString("CanNotHostGame"))}</size>";
            new LateTask(() => { textObj.text = message; }, 0.01f, "CanNotHostGame");
        }
        else if (HostGameButton)
        {
            HostGameButton?.SetActive(true);
        }

        var JoinGameButton = GameObject.Find("JoinGameButton");
        if (JoinGameButton && Toggles.EnableAntiCheat && Toggles.ServerAllHostOrNoHost)
        {
            JoinGameButton?.SetActive(false);
            var textObj1 = Object.Instantiate(JoinGameButton.transform.FindChild("Text_TMP").GetComponent<TMPro.TextMeshPro>());
            var parentObj = JoinGameButton.transform.parent.gameObject;
            textObj1.transform.position = new Vector3(-0.7f, -1.53f ,0f);
            textObj1.name = "CanNotJoinGame";
            var message = $"<size=2>{Utils.Utils.ColorString(Color.red, GetString("CanNotJoinGame"))}</size>";
            new LateTask(() => { textObj1.text = message; }, 0.01f, "CanNotJoinGame");
        }
        else if (JoinGameButton)
        {
            JoinGameButton?.SetActive(true);
        }
    }
}