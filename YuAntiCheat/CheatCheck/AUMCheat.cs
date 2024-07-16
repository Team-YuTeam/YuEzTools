using YuAntiCheat.Get;

namespace YuAntiCheat;

public class AUMCheat
{
    public static bool ReceiveInvalidRpc(PlayerControl pc, byte callId)
    {
        switch (callId)
        {
            case 101:
                Main.Logger.LogWarning($"有AmongUsMenu玩家，{"好友编号："+pc.GetClient().FriendCode+"/名字："+pc.GetRealName()+"/ProductUserId："+pc.GetClient().ProductUserId}");
                //Main.PlayerStates[pc.GetClient().Id].IsAUM = true;
                return true;
            
            
            case unchecked((byte)42069):
                Main.Logger.LogWarning($"有AmongUsMenu玩家，{"好友编号："+pc.GetClient().FriendCode+"/名字："+pc.GetRealName()+"/ProductUserId："+pc.GetClient().ProductUserId}");
                //Main.PlayerStates[pc.GetClient().Id].IsAUM = true;
                return true;
        }
        return false;
    }
}