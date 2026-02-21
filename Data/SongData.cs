using UnityEngine;

namespace TuneWeaver.Data;

public class SongData
{
    public string id = "";
    public bool isActive = false;
    public AudioClip audioClip = null;
    public string StandStartAnimName = "";
    public string SitStartAnimName = "";
    public string StandAnimName = "";
    public string SitAnimName = "";

    public SongData(string id, AudioClip audioClip)
    {
        this.id = id;
        this.audioClip = audioClip;
    }

    public void AssignAnims(string startStand = "", string startSit = "", string playStand = "", string playSit = "")
    {
        StandStartAnimName = startStand;
        StandAnimName = playStand;
        SitStartAnimName = startSit;
        SitAnimName = playSit;
    }

    public void Activate()
    {
        isActive = true;
        TuneWeaverPlugin.ActiveSongData = this;
    }

    public void Deactivate()
    {
        isActive = false;
        TuneWeaverPlugin.ActiveSongData = null;
    }
}