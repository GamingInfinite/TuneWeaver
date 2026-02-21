using System;
using System.Collections.Generic;
using System.Linq;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using TuneWeaver.Data;
using UnityEngine;

namespace TuneWeaver;

// TODO - adjust the plugin guid as needed
[BepInAutoPlugin(id: "io.github.gaminginfinite.tuneweaver")]
public partial class TuneWeaverPlugin : BaseUnityPlugin
{
    public static ManualLogSource logSource;
    public static Harmony harmony;
    public static List<SongData> AllSongs = [];
    
    public static Action ActivationCheck = () =>
    {
        if (ActiveSongData != null)
        {
            logSource.LogInfo($"{ActiveSongData.id}");
        }
    };

    private static SongData? _activeSongData;
    public static SongData? ActiveSongData
    {
        get => _activeSongData;
        set
        {
            _activeSongData = value ?? AllSongs.FirstOrDefault(s => s.isActive);
        }
    }
    
    private void Awake()
    {
        // Put your initialization logic here
        Logger.LogInfo($"Plugin {Name} ({Id}) has loaded!");

        logSource = Logger;
        harmony = new("voidbaroness.tuneweaver");
        
        harmony.PatchAll();
    }

    public static SongData RegisterSong(string id, AudioClip audioClip)
    {
        SongData newSong = new(id, audioClip);
        AllSongs.Add(newSong);
        return newSong;
    }
}