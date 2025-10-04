using UnityEngine;
using YuEzTools.UI;

namespace YuEzTools.Patches;

[HarmonyPatch(typeof(ChatBubble))]
public static class ChatBubblePatch
{
    public static string ColorString(Color32 color, string str) => $"<color=#{color.r:x2}{color.g:x2}{color.b:x2}{color.a:x2}>{str}</color>";

    [HarmonyPatch(nameof(ChatBubble.SetText)), HarmonyPrefix]
    public static void SetText_Prefix(ChatBubble __instance, ref string chatText)
    {
        var sr = __instance.transform.FindChild("Background").GetComponent<SpriteRenderer>();
        if (Toggles.DarkMode) sr.color = new Color(0, 0, 0, 255);// : new Color(1, 1, 1);
        if (Main.isChatCommand && !Toggles.DarkMode) sr.color = new Color(0, 0, 0, 255);
        else if (Main.isChatCommand && Toggles.DarkMode) sr.color = new Color(255, 255, 255, 255);
        //if (modded)
        //{
        if (chatText.Contains("░") ||
            chatText.Contains("▄") ||
            chatText.Contains("█") ||
            chatText.Contains("▌") ||
            chatText.Contains("▒") ||
            chatText.Contains("习近平") ||
            chatText.Contains("毛泽东") ||
            chatText.Contains("周恩来") ||
            chatText.Contains("邓小平") ||
            chatText.Contains("江泽民") ||
            chatText.Contains("胡锦涛") ||
            chatText.Contains("温家宝") ||
            chatText.Contains("台湾") ||
            chatText.Contains("台独") ||
            chatText.Contains("共产党")) // 游戏名字屏蔽词)
        {
            if (Toggles.DarkMode) chatText = $"<color=#FF0000>[{GetString("SuspectedViolationMessage")}]</color>\n" + ColorString(Color.white, chatText.TrimEnd('\0'));
            else chatText = $"<color=#FF0000>[{GetString("SuspectedViolationMessage")}]</color>\n" + ColorString(Color.black, chatText.TrimEnd('\0'));
        }
        else if (Main.isChatCommand)
        {
            if (Toggles.DarkMode) chatText = $"<color=#1E90FF>[{GetString("MessgaeFromYuET")}]</color>\n" + ColorString(Color.black, chatText.TrimEnd('\0'));
            else chatText = $"<color=#008B8B>[{GetString("MessgaeFromYuET")}]</color>\n" + ColorString(Color.white, chatText.TrimEnd('\0'));
            Main.isChatCommand = false;
        }
        else
        {
            if (Toggles.DarkMode) chatText = ColorString(Color.white, chatText.TrimEnd('\0'));
            else chatText = ColorString(Color.black, chatText.TrimEnd('\0'));
        }
        Msg($"来自 {__instance.playerInfo.PlayerName} 的信息 {chatText}", "ChatBubble");
        //  __instance.SetLeft();  //如果需要靠左
        //}
    }
}
[HarmonyPatch(typeof(ChatBubble), nameof(ChatBubble.SetRight))]
class ChatBubbleSetRightPatch
{
    public static void Postfix(ChatBubble __instance)
    {
        if (Main.isChatCommand) __instance.SetLeft();
    }
}
