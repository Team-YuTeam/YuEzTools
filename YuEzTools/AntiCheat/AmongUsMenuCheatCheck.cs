using Hazel;
using InnerNet;
using YuEzTools.Modules;
using YuEzTools.Utils;

namespace YuEzTools.AntiCheat;

public class AmongUsMenuAndAffiliatedCheatCheck
{
    public static bool Check(PlayerControl pc, byte callId, MessageReader reader)
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
                if (Toggles.AutoReportHacker)
                {
                    AmongUsClient.Instance.ReportPlayer(pc.GetClientId(),ReportReasons.Cheating_Hacking);
                    Info($"已尝试向Among Us官方举报【{pc.GetRealName()}-{pc.GetClient().ProductUserId}】","AntiCheatForAll");
                }
                return true;
            case unchecked((byte)420):
            case 168:
                Warn($"有SickoMenu玩家，{"好友编号：" + pc.GetClient().FriendCode + "/名字：" + pc.GetRealName() + "/ProductUserId：" + pc.GetClient().ProductUserId}", "ACFA");
                //Main.PlayerStates[pc.GetClient().Id].IsSM = true;
                if (Toggles.AutoReportHacker)
                {
                    AmongUsClient.Instance.ReportPlayer(pc.GetClientId(),ReportReasons.Cheating_Hacking);
                    Info($"已尝试向Among Us官方举报【{pc.GetRealName()}-{pc.GetClient().ProductUserId}】","AntiCheatForAll");
                }
                return true;
        }
        return false;
    }
}