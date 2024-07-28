/*using InnerNet;
using Hazel;
using HarmonyLib;

namespace YuEzTools;

[HarmonyPatch(typeof(PingTracker), nameof(PingTracker.Update))]
public static class SentYuETRpc
{
    [HarmonyPostfix]
    public static void Postfix()
    {
        //Main.Logger.LogInfo("发YuETRPC");
        var HostData = AmongUsClient.Instance.GetHost();
        if (HostData != null)
        {
            foreach (var item in PlayerControl.AllPlayerControls)
            {
                if (AmongUsClient.Instance.GetClientIdFromCharacter(item) == AmongUsClient.Instance.ClientId)
                {
                    //Main.Logger.LogInfo($"发送对象是自己 已拦截");
                    break;
                }
                //Main.Logger.LogInfo($"发YuETRPC给{AmongUsClient.Instance.GetClientIdFromCharacter(item)}");
                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, 250, SendOption.Reliable, AmongUsClient.Instance.GetClientIdFromCharacter(item));
                writer.WriteNetObject(item);
                writer.Write(true);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
            }
        }
    }
}*/