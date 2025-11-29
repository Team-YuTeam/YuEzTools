using System;
using TMPro;
using UnityEngine;

namespace YuEzTools.UI;

[HarmonyPatch(typeof(ProgressTracker))]
public static class TaskLevel
{

    [HarmonyPatch(nameof(ProgressTracker.FixedUpdate))]
    public static void Postfix()
    {
        // if(GetPlayer.isMeeting) return;
        // Info("try","ProgressTracker");
        var TaskLevel = GameObject.Find("TaskDisplay");
        var progressTracker = TaskLevel.transform.FindChild("ProgressTracker");
        var titleText_TMP = progressTracker.transform.FindChild("TitleText_TMP").GetComponent<TextMeshPro>();
        GameData instance = GameData.Instance;
        // 检查游戏数据是否存在且总任务数大于0
        if (instance && instance.TotalTasks > 0)
        {
            // Info("0","ProgressTracker");
            // 计算应该显示任务进度的玩家数量
            // 在教程模式下为1，否则为(总玩家数 - 内鬼数量)
            int num = DestroyableSingleton<TutorialManager>.InstanceExists
                ? 1
                : (instance.AllPlayers.Count - GameManager.Instance.LogicOptions.NumImpostors);
            // 减去已断开连接的玩家数量
            // 使用传统的foreach循环
            int disconnectedCount = 0;
            foreach (var player in instance.AllPlayers)
            {
                if (player.Disconnected)
                    disconnectedCount++;
            }

            double num2 = Math.Round(instance.CompletedTasks / (float)instance.TotalTasks * 100, 2);

            switch (GameManager.Instance.LogicOptions.GetTaskBarMode())
            {
                case TaskBarMode.Normal:
                    // Info("1","ProgressTracker");
                    var comms = IsActive(SystemTypes.Comms);
                    titleText_TMP.text = comms ? GetString("TaskBarMode.Comms") : GetString("TasksProgress") + $"{num2}%";
                    break;
                case TaskBarMode.MeetingOnly:
                    // Info("2", "ProgressTracker");

                    if (MeetingHud.Instance)
                    {
                        titleText_TMP.text = GetString("TaskBarMode.Meeting") + $"{num2}%";
                    }
                    break;
                case TaskBarMode.Invisible:
                    // Info("3", "ProgressTracker");

                    titleText_TMP.text = GetString("TaskBarMode.Invisible");
                    break;
                default:
                    // Info("4", "ProgressTracker");

                    titleText_TMP.text = GetString("TaskBarMode.Default");
                    break;
            }
        }
    }
}