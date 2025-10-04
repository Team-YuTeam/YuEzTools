using HarmonyLib;
using System.Linq;
using System.Text;
using YuEzTools.Modules;
using YuEzTools.Utils;

namespace YuEzTools.Patches;

[HarmonyPatch(typeof(TaskPanelBehaviour), nameof(TaskPanelBehaviour.SetTaskText))]
class TaskPanelBehaviourPatch
{
    // タスク表示の文章が更新・適用された後に実行される
    public static void Postfix(TaskPanelBehaviour __instance)
    {
        if (!GetPlayer.IsInGame) return;

        PlayerControl player = PlayerControl.LocalPlayer;

        var taskText = __instance.taskText.text;
        if (taskText == "None") return;

        var RoleWithInfo = $"{player.Data.Role.NiceName}:\r\n";
        RoleWithInfo += player.Data.RoleType.GetRoleLInfoForVanilla();

        var AllText = Utils.Utils.ColorString(Utils.Utils.GetRoleColor32(player.Data.RoleType), RoleWithInfo);

        var lines = taskText.Split("\r\n</color>\n")[0].Split("\r\n\n")[0].Split("\r\n");
        StringBuilder sb = new();
        foreach (var eachLine in lines)
        {
            var line = eachLine.Trim();
            if ((line.StartsWith("<color=#FF1919FF>") || line.StartsWith("<color=#FF0000FF>")) && sb.Length < 1 && !line.Contains('(')) continue;
            sb.Append(line + "\r\n");
        }
        if (sb.Length > 1)
        {
            var text = sb.ToString().TrimEnd('\n').TrimEnd('\r');
            if (!player.HasTasks() && sb.ToString().Count(s => (s == '\n')) >= 2)
                text = $"{Utils.Utils.ColorString(Utils.Utils.GetRoleColor32(player.Data.RoleType), GetString("FakeTask"))}\r\n{text}";
            AllText += $"\r\n\r\n<size=85%>{text}</size>";
        }

        AllText += "\n\n<size=60%>" + GetString("PressF3ToShowYourRoleInfo") + "</size>";

        __instance.taskText.text = AllText;

        // RepairSenderの表示
        if (RepairSender.enabled && AmongUsClient.Instance.NetworkMode != NetworkModes.OnlineGame)
            __instance.taskText.text = RepairSender.GetText();
    }
}
class RepairSender
{
    public static bool enabled = false;
    public static bool TypingAmount = false;

    public static int SystemType;
    public static int amount;

    public static void Input(int num)
    {
        if (!TypingAmount)
        {
            //SystemType入力中
            SystemType *= 10;
            SystemType += num;
        }
        else
        {
            //Amount入力中
            amount *= 10;
            amount += num;
        }
    }
    public static void InputEnter()
    {
        if (!TypingAmount)
        {
            //SystemType入力中
            TypingAmount = true;
        }
        else
        {
            //Amount入力中
            Send();
        }
    }
    public static void Send()
    {
        ShipStatus.Instance.RpcUpdateSystem((SystemTypes)SystemType, (byte)amount);
        Reset();
    }
    public static void Reset()
    {
        TypingAmount = false;
        SystemType = 0;
        amount = 0;
    }
    public static string GetText()
    {
        return SystemType.ToString() + "(" + ((SystemTypes)SystemType).ToString() + ")\r\n" + amount;
    }
}