using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GameBalance;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Michsky.UI.Reach;

public class TrackSelectManager : MonoBehaviour
{
    [SerializeField] ChapterManager TrackSelectMgr;

    bool IsMultiplayer;
    TrackPreset CurrentTrackPreset;
    int CurrentTrackIndex = 0;
    bool SubmitIsPressed = true;
    bool HorizontalIsPressed = true;
    const string LapCaption = "Lap";
    const string LapsCaption = "Laps";

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

                // return Tracks.FirstOrDefault(t => t.TrackName == "Epstein Island");
            case 2:
                return Tracks.FirstOrDefault(t => t.TrackName == "Iceland Festival");
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
