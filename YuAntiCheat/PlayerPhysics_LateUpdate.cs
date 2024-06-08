using HarmonyLib;
using UnityEngine;
using YuAntiCheat.Get;

namespace YuAntiCheat;

[HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.LateUpdate))]
public static class PlayerPhysics_LateUpdate
{
    public static void Postfix(PlayerPhysics __instance)
    {
        GetPlayer.playerNametags(__instance);
    }
}

[HarmonyPatch(typeof(ChatBubble), nameof(ChatBubble.SetName))]
public static class ChatBubble_SetName
{
    public static void Postfix(ChatBubble __instance){
        GetPlayer.chatNametags(__instance);
    }
}    