using InnerNet;
using AmongUs.GameOptions;
using Hazel;
using System;
using System.Linq;
using HarmonyLib;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using InnerNet;
using UnityEngine;
using YuEzTools;
using System.Net.Http;  
using System.Threading.Tasks;
using YuEzTools.Get;
using YuEzTools.Modules;

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