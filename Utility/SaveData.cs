using ProtoBuf;
using UnityEngine;

[System.Serializable]
[ProtoContract]
public class SaveData
{
    [ProtoMember(1)]
    public int unlockedLevels = 0;
    [ProtoMember(2)]
    public int playerXp = 0;

    [ProtoMember(3)] 
    public bool isFullscreen = true;

    [ProtoMember(4)] 
    public float masterVolume = 1f;
    [ProtoMember(5)] 
    public float musicVolume = 1f;
    [ProtoMember(6)] 
    public float soundVolume = 1f;

    [ProtoMember(9)] public bool isMasterMuted;
    [ProtoMember(10)] public bool isMusicMuted;
    [ProtoMember(11)] public bool isSfxMuted;
    
    [ProtoMember(7)] 
    public int xResolution = 1920;
    [ProtoMember(8)] 
    public int yResolution = 1080;

    [ProtoMember(12)]
    public bool tutorialFinished = false;
    [ProtoMember(13)]
    public bool demoMode = false;
}  