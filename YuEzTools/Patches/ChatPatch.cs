using Hazel;
using YuEzTools.Utils;

namespace YuEzTools.Patches;

[HarmonyPatch(typeof(ChatController), nameof(ChatController.Update))]
internal class ChatUpdatePatch
{
    public static bool Active = false;
    public static bool DoBlockChat = false;
    public static float chatStop = 3;
    public static void Postfix(ChatController __instance)
    {
        // if (GameStartManagerPatch.roomMode != RoomMode.Plus25) return;
        chatStop = __instance.timeSinceLastMessage;
        Active = __instance.IsOpenOrOpening;
        //__instance.freeChatField.textArea.AllowPaste = true;
        __instance.chatBubblePool.Prefab.Cast<ChatBubble>().TextArea.overrideColorTags = false;
        // Info(__instance.timeSinceLastMessage.ToString(),"ChatPatchDebug");
        if (!AmongUsClient.Instance.AmHost || Main.MessagesToSend.Count < 1 || (Main.MessagesToSend[0].Item2 == byte.MaxValue && __instance.timeSinceLastMessage <= 3f)) return;
        // Info("2","ChatPatchDebug");
        if (DoBlockChat) return;
        // Info("3","ChatPatchDebug");
        var player = Main.AllAlivePlayerControls.OrderBy(x => x.PlayerId).FirstOrDefault() ?? Main.AllPlayerControls.OrderBy(x => x.PlayerId).FirstOrDefault();
        // Info("4","ChatPatchDebug");
        if (player == null) return;
        // Info("5","ChatPatchDebug");
        (string msg, byte sendTo, string title) = Main.MessagesToSend[0];
        Main.MessagesToSend.RemoveAt(0);
        int clientId = sendTo == byte.MaxValue ? -1 : GetPlayer.GetPlayerById(sendTo).GetClientId();
        var name = player.Data.PlayerName;
        if (clientId == -1)
        {
            player.SetName(title);
            DestroyableSingleton<HudManager>.Instance.Chat.AddChat(player, msg);
            player.SetName(name);
        }
        var writer = CustomRpcSender.Create("MessagesToSend", SendOption.None);
        writer.StartMessage(clientId);
        writer.StartRpc(player.NetId, (byte)RpcCalls.SetName)
            .Write(player.Data.NetId)
            .Write(title)
            .EndRpc();
        writer.StartRpc(player.NetId, (byte)RpcCalls.SendChat)
            .Write(msg)
            .EndRpc();
        writer.StartRpc(player.NetId, (byte)RpcCalls.SetName)
            .Write(player.Data.NetId)
            .Write(player.Data.PlayerName)
            .EndRpc();
        writer.EndMessage();
        writer.SendMessage();
        __instance.timeSinceLastMessage = 0f;
    }
}