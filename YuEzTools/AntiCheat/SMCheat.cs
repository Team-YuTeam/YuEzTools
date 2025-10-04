using YuEzTools.Utils;

namespace YuEzTools.AntiCheat;

public class SMCheat
{
    public static bool ReceiveInvalidRpc(PlayerControl pc, byte callId)
    {
        switch (callId)
        {
            case unchecked((byte)420):
            case 168:
                Warn($"有SickoMenu玩家，{"好友编号："+pc.GetClient().FriendCode+"/名字："+pc.GetRealName()+"/ProductUserId："+pc.GetClient().ProductUserId}", "ACFA");
                //Main.PlayerStates[pc.GetClient().Id].IsSM = true;
                return true;
        }
        return false;
    }
}