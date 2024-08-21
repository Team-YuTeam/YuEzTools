using AmongUs.Data;
using HarmonyLib;
using System;
using TMPro;
using UnityEngine;

namespace YuEzTools.Modules;

public class Chat
{
    [HarmonyPatch(typeof(ChatController), nameof(ChatController.Update))]
    public static class ChatControllerUpdatePatch
    {
        public static void Prefix(ChatController __instance)
        {
            if ((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) &&
                Input.GetKeyDown(KeyCode.Tab))
            {
                __instance.freeChatField.textArea.SetText(__instance.freeChatField.textArea.text + "\n");
                return;
            }
        }
        public static void Postfix(ChatController __instance)
        {
            if (!__instance.freeChatField.textArea.hasFocus) return;
            if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && Input.GetKeyDown(KeyCode.C))
                ClipboardHelper.PutClipboardString(__instance.freeChatField.textArea.text);
            
            if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && Input.GetKeyDown(KeyCode.V))
                __instance.freeChatField.textArea.SetText(__instance.freeChatField.textArea.text + GUIUtility.systemCopyBuffer);
            
            if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && Input.GetKeyDown(KeyCode.X))
            {
                ClipboardHelper.PutClipboardString(__instance.freeChatField.textArea.text);
                __instance.freeChatField.textArea.SetText("");
            }
        }
    }
}
// ChatJailbreak
[HarmonyPatch(typeof(ChatController), nameof(ChatController.Update))]
public static class ChatJailbreak_ChatController_Update_Postfix
{
    public static void Postfix(ChatController __instance)
    {
        if (Main.PatchChat.Value)
        { 
            if (!__instance.freeChatField.textArea.hasFocus) return;
            //__instance.freeChatField.textArea.AllowPaste = true;
            //__instance.freeChatField.textArea.AllowSymbols = true;
            __instance.freeChatField.textArea.AllowEmail = true;
        }
    }
}