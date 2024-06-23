using AmongUs.QuickChat;
using HarmonyLib;
using UnityEngine;

namespace YuAntiCheat.Patches;

[HarmonyPatch(typeof(ChatBubble))]
public static class ChatBubblePatch
{
    //private static bool IsModdedMsg(string name) => name.EndsWith('\0');

    // [HarmonyPatch(nameof(ChatBubble.SetName)), HarmonyPostfix]
    // public static void SetName_Postfix(ChatBubble __instance)
    // {
    //     if (GameStates.IsInGame && __instance.playerInfo.PlayerId == PlayerControl.LocalPlayer.PlayerId)
    //         __instance.NameText.color = PlayerControl.LocalPlayer.GetRoleColor();
    // }
    public static string ColorString(Color32 color, string str) => $"<color=#{color.r:x2}{color.g:x2}{color.b:x2}{color.a:x2}>{str}</color>";

    [HarmonyPatch(nameof(ChatBubble.SetText)), HarmonyPrefix]
    public static void SetText_Prefix(ChatBubble __instance, ref string chatText)
    {
        var sr = __instance.transform.FindChild("Background").GetComponent<SpriteRenderer>();
        sr.color = new Color(0, 0, 0,255);// : new Color(1, 1, 1);
        //if (modded)
        //{
            chatText = ColorString(Color.white, chatText.TrimEnd('\0'));
            //  __instance.SetLeft();  //如果需要靠左
        //}
    }
}
// [HarmonyPatch(typeof(FreeChatInputField))]
// public static class FreeChatInputFieldPatch
// {
//     //private static bool IsModdedMsg(string name) => name.EndsWith('\0');
//
//     // [HarmonyPatch(nameof(ChatBubble.SetName)), HarmonyPostfix]
//     // public static void SetName_Postfix(ChatBubble __instance)
//     // {
//     //     if (GameStates.IsInGame && __instance.playerInfo.PlayerId == PlayerControl.LocalPlayer.PlayerId)
//     //         __instance.NameText.color = PlayerControl.LocalPlayer.GetRoleColor();
//     // }
//     public static string ColorString(Color32 color, string str) => $"<color=#{color.r:x2}{color.g:x2}{color.b:x2}{color.a:x2}>{str}</color>";
//
//     [HarmonyPatch(nameof(FreeChatInputField.Start)), HarmonyPrefix]
//     public static void SetText_Prefix(ChatBubble __instance, ref string chatText)
//     {
//         var sr = __instance.transform.FindChild("Background").GetComponent<SpriteRenderer>();
//         sr.color = new Color(0, 0, 0);// : new Color(1, 1, 1);
//         //if (modded)
//         //{
//         chatText = ColorString(Color.white, chatText.TrimEnd('\0'));
//         //  __instance.SetLeft();  //如果需要靠左
//         //}
//     }
// }
// [HarmonyPatch(typeof(ButtonRolloverHandler))]
// class ButtonRolloverHandlerPatch
// {
//     [HarmonyPatch(nameof(ButtonRolloverHandler.DoMouseOver)), HarmonyPrefix]
//     public static void DoMouseOver_Prefix(ButtonRolloverHandler __instance)
//     {
//         if (__instance.OverColor == new Color(0, 1, 0, 1) || __instance.OverColor == Palette.AcceptedGreen)
//             __instance.OverColor = new Color32(47,79,79, 192);
//     }
//     [HarmonyPatch(nameof(ButtonRolloverHandler.DoMouseOut)), HarmonyPrefix]
//     public static void DoMouseOut_Prefix(ButtonRolloverHandler __instance)
//     {
//         __instance.OutColor = new Color32(0,0,0, 192);
//     }
//     [HarmonyPatch(nameof(ButtonRolloverHandler.ChangeOutColor)), HarmonyPrefix]
//     public static void ChangeOutColor_Prefix(ButtonRolloverHandler __instance, ref Color color)
//     {
//         if (color.r == 0 && color.g == 1 && color.b is > 0.163f and < 0.165f && color.a == 1)
//             color = new Color32(	255,215,0, 255);
//     }
// }
// [HarmonyPatch(typeof(Palette))]
// class PalettePath
// {
//     [HarmonyPatch(nameof(Palette.AcceptedGreen), MethodType.Getter), HarmonyPrefix]
//     public static bool Get_AcceptedGreen_Prefix(ref Color __result)
//     {
//         __result = new Color32(158, 239, 255, (byte)(__result.a * 255));
//         return false;
//     }
// }