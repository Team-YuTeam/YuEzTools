using InnerNet;
using Hazel;
using HarmonyLib;

namespace YuAntiCheat;

public static class SentYuacRpc
{
    [HarmonyPatch(typeof(PingTracker), nameof(PingTracker.Update))]
    public static void sentYuacRpc()
    {
        var HostData = AmongUsClient.Instance.GetHost();
        if (HostData != null)
        {
            foreach (var item in PlayerControl.AllPlayerControls)
            {
                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, 250, SendOption.None, AmongUsClient.Instance.GetClientIdFromCharacter(item));
                writer.WriteNetObject(item);
                //writer.Write();
                AmongUsClient.Instance.FinishRpcImmediately(writer);
            }
        }
    }
}