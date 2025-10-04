using InnerNet;

namespace YuEzTools;

static class ExtendedPlayerControl
{
    public const MurderResultFlags SucceededFlags = MurderResultFlags.Succeeded | MurderResultFlags.DecisionByHost;
    public static bool OwnedByHost(this InnerNetObject innerObject)
        => innerObject.OwnerId == AmongUsClient.Instance.HostId;
}