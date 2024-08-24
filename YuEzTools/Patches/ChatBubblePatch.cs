using AmongUs.QuickChat;
using HarmonyLib;
using UnityEngine;
using YuEzTools.Get;
using YuEzTools.Modules;
using YuEzTools.UI;
using YuEzTools.Utils;

namespace YuEzTools.Patches;

[HarmonyPatch(typeof(ChatBubble))]
public static class ChatBubblePatch
{
    public static string ColorString(Color32 color, string str) => $"<color=#{color.r:x2}{color.g:x2}{color.b:x2}{color.a:x2}>{str}</color>";
    
    [HarmonyPatch(nameof(ChatBubble.SetText)), HarmonyPrefix]
    public static void SetText_Prefix(ChatBubble __instance, ref string chatText)
    {
        var sr = __instance.transform.FindChild("Background").GetComponent<SpriteRenderer>();
        if(Toggles.DarkMode) sr.color = new Color32(0, 0, 0,255);// : new Color(1, 1, 1);
        
        if (Main.isChatCommand && !Toggles.DarkMode) sr.color = new Color32(0, 0, 0,255);
        else if(Main.isChatCommand && Toggles.DarkMode)sr.color = new Color32(255, 255, 255,255);

        if (GetPlayer.IsInGame && __instance.playerInfo.PlayerId.GetPlayerDataById().IsDead)
        {
            if (!Toggles.DarkMode) sr.color = new Color32(61, 255, 141,255);
            else if(Toggles.DarkMode)sr.color = new Color32(70, 61, 255,255);
        }
        //if (modded)
        //{
        if (chatText.CheckBanWord() || chatText.CheckDllBanWord()) // 游戏名字屏蔽词)
        {
            if (Toggles.shieldForbiddenWords)
            { 
                Logger.Msg($"来自 {__instance.playerInfo.PlayerName} 的敏感信息 {chatText}","ChatBubble");
                chatText = $"<color=#FF0000>{Translator.GetString("ShieldSuspectedViolationMessage")}</color>";
                //else chatText = $"<color=#FF0000>[{Translator.GetString("ShieldSuspectedViolationMessage")}]</color>\n" + ColorString(Color.black, chatText.TrimEnd('\0'));
            }
            else
            {
                if(Toggles.DarkMode) chatText = $"<color=#FF0000>[{Translator.GetString("SuspectedViolationMessage")}]</color>\n" + ColorString(Color.white, chatText.TrimEnd('\0'));
                else chatText = $"<color=#FF0000>[{Translator.GetString("SuspectedViolationMessage")}]</color>\n" + ColorString(Color.black, chatText.TrimEnd('\0'));
            }

        }
        else if (Main.isChatCommand)
        {
            if(Toggles.DarkMode) chatText = $"<color=#1E90FF>[{Translator.GetString("MessgaeFromYuET")}]</color>\n" + ColorString(Color.black, chatText.TrimEnd('\0'));
            else chatText = $"<color=#008B8B>[{Translator.GetString("MessgaeFromYuET")}]</color>\n" + ColorString(Color.white, chatText.TrimEnd('\0'));
            Main.isChatCommand = false;
        }
        else
        {
            if(Toggles.DarkMode) chatText = ColorString(Color.white, chatText.TrimEnd('\0'));
            else chatText = ColorString(Color.black, chatText.TrimEnd('\0'));
        }
        Logger.Msg($"来自 {__instance.playerInfo.PlayerName} 的信息 {chatText}","ChatBubble");
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
