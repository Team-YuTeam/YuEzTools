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
using Sentry.Internal.Extensions;
using UnityEngine;

namespace YuAntiCheat.Get;

public class PlayerState
{
    public bool IsSM { get; set; }
    public bool IsAUM { get; set; }
    public bool IsHacker { get; set; }
    public bool IsDead { get; set; }

    public PlayerState(byte playerId)
    {
        IsSM = false;
        IsAUM = false;
        IsHacker = false;
        IsDead = false;
    }
}

static class GetPlayer
{
    public static bool InGame = false;
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
    public static string getNameTag(PlayerControl player, string playerName, bool isChat = false){
        string nameTag = playerName;
        
        if (isChat){
            if(Main.PlayerStates[player.PlayerId].IsAUM)
                nameTag = $"<color=#{ColorUtility.ToHtmlStringRGB(player.Data.Role.TeamColor)}><size=70%>[AUM]</size> {nameTag}</color>";
            if(Main.PlayerStates[player.PlayerId].IsSM)
                nameTag = $"<color=#{ColorUtility.ToHtmlStringRGB(player.Data.Role.TeamColor)}><size=70%>[SM]</size> {nameTag}</color>";
            if(Main.PlayerStates[player.PlayerId].IsHacker)
                nameTag = $"<color=#{ColorUtility.ToHtmlStringRGB(player.Data.Role.TeamColor)}><size=70%>[Hacker]</size> {nameTag}</color>";
            return nameTag;
        }
        if(Main.PlayerStates[player.PlayerId].IsAUM)
            nameTag = $"<color=#{ColorUtility.ToHtmlStringRGB(player.Data.Role.TeamColor)}><size=70%>[AUM]</size>\r\n{nameTag}</color>";
        if(Main.PlayerStates[player.PlayerId].IsSM)
            nameTag = $"<color=#{ColorUtility.ToHtmlStringRGB(player.Data.Role.TeamColor)}><size=70%>[SM]</size>\r\n{nameTag}</color>";
        if(Main.PlayerStates[player.PlayerId].IsHacker)
            nameTag = $"<color=#{ColorUtility.ToHtmlStringRGB(player.Data.Role.TeamColor)}><size=70%>[Hacker]</size>\r\n{nameTag}</color>";

        return nameTag;
    }
    public static void playerNametags(PlayerPhysics playerPhysics)
    {
        try{
            if (!playerPhysics.myPlayer.Data.IsNull() && !playerPhysics.myPlayer.Data.Disconnected && !playerPhysics.myPlayer.CurrentOutfit.IsNull())
            {
                playerPhysics.myPlayer.cosmetics.SetName(getNameTag(playerPhysics.myPlayer, playerPhysics.myPlayer.CurrentOutfit.PlayerName));
            }
        }catch{}
    }

    public static void chatNametags(ChatBubble chatBubble)
    {
        try{

            // Update the player's nametag appropriately
            chatBubble.NameText.text = getNameTag(chatBubble.playerInfo.Object, chatBubble.NameText.text, true);
            
            // Adjust the chatBubble's size to the new nametag to prevent issues
            chatBubble.NameText.ForceMeshUpdate(true, true);
            chatBubble.Background.size = new Vector2(5.52f, 0.2f + chatBubble.NameText.GetNotDumbRenderedHeight() + chatBubble.TextArea.GetNotDumbRenderedHeight());
            chatBubble.MaskArea.size = chatBubble.Background.size - new Vector2(0f, 0.03f);

        }catch{}
    }
    public static PlayerControl GetPlayerById(int PlayerId)
    {
        return Main.AllPlayerControls.Where(pc => pc.PlayerId == PlayerId).FirstOrDefault();
    }
    public static bool IsInGame => InGame;
    public static bool IsMeeting => InGame && MeetingHud.Instance;
    public static bool IsLobby => AmongUsClient.Instance.GameState == AmongUsClient.GameStates.Joined;
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
}