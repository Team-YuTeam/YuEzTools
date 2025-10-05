using Hazel;
using YuEzTools.Utils;

namespace YuEzTools.AntiCheat;

public class AUMCheat
{
    public static bool ReceiveInvalidRpc(PlayerControl pc, byte callId, MessageReader reader)
    {
        MessageReader sr = MessageReader.Get(reader);
        var AUMChat = sr.ReadString();
        switch (callId)
        {
            case 101:
            case unchecked((byte)42069):
                Warn($"玩家【{pc.GetClientId()}:{pc.GetRealName()}】AUMRPC/Chat，内容：{AUMChat}", "ACFA");
                Warn($"有AmongUsMenu玩家，{"好友编号：" + pc.GetClient().FriendCode + "/名字：" + pc.GetRealName() + "/ProductUserId：" + pc.GetClient().ProductUserId}", "ACFA");
                //Main.PlayerStates[pc.GetClient().Id].IsAUM = true;
                return true;
        }
        return false;
    }
}