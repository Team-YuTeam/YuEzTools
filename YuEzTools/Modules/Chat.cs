using HarmonyLib;
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



// Allow special characters
[HarmonyPatch(typeof(TextBoxTMP), nameof(TextBoxTMP.IsCharAllowed))]
public static class AllowAllCharacters_TextBoxTMP_IsCharAllowed_Prefix
{
    public static bool Prefix(TextBoxTMP __instance, char i, ref bool __result)
    {
        if (Main.PatchChat.Value)
        {
            __result = !(i == '\b'); // Bugfix: '\b' messing with chat message
            return false;
        }
        else return true;
    }

    public static void Postfix(TextBoxTMP __instance)
    {
        if (Main.PatchChat.Value)
        {
            //__instance.allowAllCharacters = true; // not used by game's code, but I include it anyway
            __instance.AllowEmail = true;
           // __instance.AllowPaste = true;
           // __instance.AllowSymbols = true;
        }
    }
}