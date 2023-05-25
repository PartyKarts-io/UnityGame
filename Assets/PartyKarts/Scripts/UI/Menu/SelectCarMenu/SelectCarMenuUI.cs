using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameBalance;
using TMPro;
using Michsky.UI.Reach;

/// <summary>
/// Select car menu.
/// </summary>
public class SelectCarMenuUI : MonoBehaviour
{
    [SerializeField] ChapterManager CarSelectManager;
    [SerializeField] CarPreset SelectedCar;

    int CurrentCarIndex;
    List<CarPreset> Cars { get { return B.MultiplayerSettings.AvailableCarsForMultiplayer; } }

    private void Start()
    {
        SelectCar(SelectedCar);
    }

    void SelectCar(CarPreset newCar)
    {
        SelectedCar = newCar;
    }

    public void Select()
    {
        CurrentCarIndex = CarSelectManager.currentChapterIndex;
        SelectCar(Cars[CurrentCarIndex]);
        WorldLoading.PlayerCar = SelectedCar;
    }

    public void LoadRace()
    {
        LoadingScreenUI.LoadScene(WorldLoading.LoadingTrack.SceneName, WorldLoading.RegimeSettings.RegimeSceneName);
    }
}
