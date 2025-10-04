using AmongUs.GameOptions;
using System;
using InnerNet;

namespace YuEzTools.Utils;

public class PlayerState
{
    public bool IsDead { get; set; }

    public PlayerState(int clientid)
    {
        IsDead = false;
    }
    public void SetDead()
    {
        IsDead = true;
    }
    private static Dictionary<byte, PlayerState> allPlayerStates = new(15);
    public static IReadOnlyDictionary<byte, PlayerState> AllPlayerStates => allPlayerStates;
    public static PlayerState GetByPlayerId(byte playerId) => AllPlayerStates.TryGetValue(playerId, out var state) ? state : null;
}

static class GetPlayer
{
    public static bool IsCanMove => (bool)PlayerControl.LocalPlayer?.CanMove;
    //public static bool IsNotJoined => AmongUsClient.Instance.GameState == AmongUsClient.GameStates.NotJoined;
    public static bool IsVanillaServer
    {
        get
        {
            if (!IsOnlineGame) return false;

            const string Domain = "among.us";

            // From Reactor.gg
            return ServerManager.Instance.CurrentRegion?.TryCast<StaticHttpRegionInfo>() is { } regionInfo &&
                   regionInfo.PingServer.EndsWith(Domain, StringComparison.Ordinal) &&
                   regionInfo.Servers.All(serverInfo => serverInfo.Ip.EndsWith(Domain, StringComparison.Ordinal));
        }
    }

    public static ClientData GetClientById(int id)
    {
        try
        {
            var client = AmongUsClient.Instance.allClients.ToArray().FirstOrDefault(cd => cd.Id == id);
            return client;
        }
        catch
        {
            return null;
        }
    }
    public static string GetColorRole(PlayerControl pc)
    {
        return "<color=" + Utils.GetRoleHtmlColor(pc.Data.RoleType) + ">" + pc.Data.Role.NiceName + "</color>";
    }
    public static string GetRColorName(PlayerControl pc, string name)
    {
        return "<color=" + Utils.GetRoleHtmlColor(pc.Data.RoleType) + ">" + name + "</color>";
    }
    public static string GetNameRole(PlayerControl player)
    {
        return player.GetRealName() + "(" + player.Data.Role.NiceName + ")";
    }
    public static bool IsHideNSeek => GameOptionsManager.Instance.CurrentGameOptions.GameMode is GameModes.HideNSeek or GameModes.SeekFools;
    public static byte GetActiveMapId() => GameOptionsManager.Instance.CurrentGameOptions.MapId;
    public static bool IsExilling => ExileController.Instance != null && !(AirshipIsActive && SpawnInMinigame.Instance.isActiveAndEnabled);
    private static Dictionary<byte, PlayerState> allPlayerStates = new(15);
    public static IReadOnlyDictionary<byte, PlayerState> AllPlayerStates => allPlayerStates;
    public static RoleTeam GetPlayerRoleTeam(this PlayerControl pc)
    {
        if (pc.Data.RoleType is RoleTypes.Crewmate or RoleTypes.Engineer or RoleTypes.CrewmateGhost
            or RoleTypes.Noisemaker or RoleTypes.GuardianAngel or RoleTypes.Scientist or RoleTypes.Tracker)
            return RoleTeam.Crewmate;
        else if (pc.Data.RoleType is RoleTypes.Impostor or RoleTypes.Shapeshifter or RoleTypes.ImpostorGhost
                 or RoleTypes.Phantom)
            return RoleTeam.Impostor;
        return RoleTeam.Error;
    }
    public static ClientData GetClient(this PlayerControl player)
    {
        try
        {
            var client = AmongUsClient.Instance.allClients.ToArray().Where(cd => cd.Character.PlayerId == player.PlayerId).FirstOrDefault();
            return client;
        }
        catch
        {
            return null;
        }
    }

    public static ReferenceDataManager referenceDataManager = DestroyableSingleton<ReferenceDataManager>.Instance;
    public static bool IsShip => ShipStatus.Instance != null;
    public static bool IsLobby => AmongUsClient.Instance.GameState == InnerNetClient.GameStates.Joined;
    public static bool IsOnlineGame => AmongUsClient.Instance.NetworkMode == NetworkModes.OnlineGame;
    public static bool IsLocalGame => AmongUsClient.Instance.NetworkMode == NetworkModes.LocalGame;
    public static bool IsFreePlay => AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay;
    public static bool isPlayer => PlayerControl.LocalPlayer != null;
    public static bool IsHost = AmongUsClient.Instance.AmHost;
    public static bool IsInGame => AmongUsClient.Instance.GameState == InnerNetClient.GameStates.Started && isPlayer;
    public static bool isMeeting => MeetingHud.Instance;
    public static bool isMeetingVoting => isMeeting && MeetingHud.Instance.state is MeetingHud.VoteStates.Voted or MeetingHud.VoteStates.NotVoted;
    public static bool isMeetingProceeding => isMeeting && MeetingHud.Instance.state is MeetingHud.VoteStates.Proceeding;
    public static bool isExiling => ExileController.Instance != null && !(AirshipIsActive && SpawnInMinigame.Instance.isActiveAndEnabled);
    public static bool isNormalGame => GameOptionsManager.Instance.CurrentGameOptions.GameMode == GameModes.Normal;
    public static bool isHideNSeek => GameOptionsManager.Instance.CurrentGameOptions.GameMode == GameModes.HideNSeek;
    public static bool SkeldIsActive => (MapNames)GameOptionsManager.Instance.CurrentGameOptions.MapId == MapNames.Skeld;
    public static bool MiraHQIsActive => (MapNames)GameOptionsManager.Instance.CurrentGameOptions.MapId == MapNames.MiraHQ;
    public static bool PolusIsActive => (MapNames)GameOptionsManager.Instance.CurrentGameOptions.MapId == MapNames.Polus;
    public static bool AirshipIsActive => (MapNames)GameOptionsManager.Instance.CurrentGameOptions.MapId == MapNames.Airship;
    public static bool FungleIsActive => (MapNames)GameOptionsManager.Instance.CurrentGameOptions.MapId == MapNames.Fungle;
    public static int GetImpNums => GameOptionsManager.Instance.CurrentGameOptions.NumImpostors;
    //public static bool IsCountDown => GameStartManager.InstanceExists && GameStartManager.Instance.startState == GameStartManager.StartingStates.Countdown;
    public static PlayerControl GetPlayerById(int PlayerId)
    {
        return Main.AllPlayerControls.Where(pc => pc.PlayerId == PlayerId).FirstOrDefault();
    }
    public static bool IsMeeting => IsInGame && MeetingHud.Instance;
    public static bool IsCountDown => GameStartManager.InstanceExists && GameStartManager.Instance.startState == GameStartManager.StartingStates.Countdown;
    public static string GetRealName(this PlayerControl player, bool isMeeting = false)
    {
        return isMeeting ? player?.Data?.PlayerName : player?.name;
    }
    public static int GetClientId(this PlayerControl player)
    {
        if (player == null) return -1;
        var client = player.GetClient();
        return client == null ? -1 : client.Id;
    }
    public static int numImpostors = 0;
    public static int numCrewmates = 0;

}
