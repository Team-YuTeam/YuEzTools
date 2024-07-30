using AmongUs.GameOptions;
using Hazel;
using InnerNet;
using System;
using System.Text;
using UnityEngine;
using static YuEzTools.Translator;

namespace YuEzTools;

static class ExtendedPlayerControl
{
    public const MurderResultFlags SucceededFlags = MurderResultFlags.Succeeded | MurderResultFlags.DecisionByHost;
    public static bool OwnedByHost(this InnerNetObject innerObject)
        => innerObject.OwnerId == AmongUsClient.Instance.HostId;
}