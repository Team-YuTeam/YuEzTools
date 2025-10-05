using InnerNet;
using Hazel;

namespace YuEzTools.Patches;

public static class MurderHacker
{
    public static void murderHacker(PlayerControl target, MurderResultFlags result)
    {
        var HostData = AmongUsClient.Instance.GetHost();
        if (HostData != null)
        {
            foreach (var item in PlayerControl.AllPlayerControls)
            {
                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)RpcCalls.MurderPlayer, SendOption.None, AmongUsClient.Instance.GetClientIdFromCharacter(item));
                writer.WriteNetObject(target);
                writer.Write((int)result);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
            }
        }
    }
}