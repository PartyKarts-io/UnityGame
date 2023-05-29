using System;
using System.Collections.Generic;
using System.Linq;
using GameBalance;
using UnityEngine;
using Michsky.UI.Reach;

public class TrackSelectManager : MonoBehaviour
{
    [SerializeField] ChapterManager TrackSelectMgr;

    TrackPreset CurrentTrackPreset;
    int CurrentTrackIndex = 0;

    public Action<TrackPreset> OnSelectTrackAction { get; set; }

    List<TrackPreset> Tracks { get { return WorldLoading.IsMultiplayer ? B.MultiplayerSettings.AvailableTracksForMultiplayer : B.GameSettings.Tracks; } }

    TrackPreset GetTrackByIndex(int index)
    {
        switch(index)
        {
            case 0:
                return Tracks.FirstOrDefault(t => t.TrackName == "Party Park");
            case 1:
                return Tracks.FirstOrDefault(t => t.TrackName == "Iceland Festival");
            case 2:
                return Tracks.FirstOrDefault(t => t.TrackName == "Epstein Island");
            default:
                return Tracks.FirstOrDefault(t => t.TrackName == "Party Park");
        }
    }

    public void OnPanelChanged()
    {
        SelectTrack();
    }

    public void SelectTrack()
    {
        CurrentTrackIndex = TrackSelectMgr.currentChapterIndex;
        CurrentTrackPreset = GetTrackByIndex(CurrentTrackIndex);
        WorldLoading.LoadingTrack = CurrentTrackPreset;
    }
}
