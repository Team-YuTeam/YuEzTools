using Hazel;

namespace YuEzTools.Patches;

public class KickHackerPatch
{
    public static void KickPlayer(PlayerControl pc)
    {
        
        var HostData = AmongUsClient.Instance.GetHost();
        if (HostData != null)
        {
            foreach (var item in PlayerControl.AllPlayerControls)
            {
                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(item.NetId, (byte)RpcCalls.AddVote, SendOption.None, AmongUsClient.Instance.GetClientIdFromCharacter(item));
                AmongUsClient.Instance.FinishRpcImmediately(writer);
            }
        }
    }
}