using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System;
using System.Threading.Tasks;
using Thirdweb;
using Photon.Pun;
using Newtonsoft.Json;
using System.Xml;
using System.Numerics;

[Serializable]
public class PlayerRaceResults
{
    public string Name;
    public string RaceTime;
    public int FinishPosition;
    public int LapCount;
}

public class RaceEndGameStatisticsUI : MonoBehaviour
{
    [SerializeField] string MainMenuSceneName = "MainMenuScene";
    [SerializeField] Animator EndGameStatisticHolder;
    [SerializeField] Button RestartGameButton;
    [SerializeField] Button ExitToMainMenuButton;

    [SerializeField] TextMeshProUGUI RaceTimeText;
    [SerializeField] TextMeshProUGUI PrevRaceTimeText;

    [SerializeField] TextMeshProUGUI BestLapText;
    [SerializeField] TextMeshProUGUI PrevBestLapText;

    [SerializeField] TextMeshProUGUI MoneyCaptionText;

    [SerializeField] Color ResultWorseColor;
    [SerializeField] Color ResultBetterColor;

    [SerializeField] List<RaceRatingPlayerUI> Players = new List<RaceRatingPlayerUI>();

    GameController GameController { get { return GameController.Instance; } }
    RaceEntity RaceEntity { get { return GameController.RaceEntity as RaceEntity; } }
    CarStatistic PlayerStatistics { get { return RaceEntity.PlayerStatistics; } }
    private bool _awaitingTxn = false;

    private List<PlayerRaceResults> raceResults = new();

    public void Update()
    {
        ExitToMainMenuButton.interactable = !_awaitingTxn;
    }

    public void Init()
    {
        ExitToMainMenuButton.onClick.AddListener(Exit);
        GameController.OnEndGameAction += OnEndGame;

        if (WorldLoading.IsMultiplayer)
        {
            RestartGameButton.interactable = false;
        }
        else
        {
            RestartGameButton.onClick.AddListener(RestartGame);
        }

        gameObject.SetActive(false);
    }

    IEnumerator ShowEndGameCoroutine()
    {
        if (!WorldLoading.HasLoadingParams)
        {
            yield break;
        }

        while (GameController.AllCars.Any(c => c != null && !c.PositioningCar.IsFinished))
        {
            yield return null;
        }

        Players.ForEach(p => p.SetActive(false));

        RaceEntity.CheckRatingOfPlayers();

        var orderedCarsStatistics = RaceEntity.CarsStatistics.OrderBy(c => c.TotalRaceTime).ToList();

        float firstPlayerTime = orderedCarsStatistics[0].TotalRaceTime;

        // Show rating of players.
        for (int i = 0; i < orderedCarsStatistics.Count; i++)
        {
            if (i < Players.Count)
            {
                var carStat = orderedCarsStatistics[i];
                Players[i].SetActive(true);
                string raceTime;
                if (i == 0)
                {
                    // record the winners total race time, and show all others as differentials to it
                    raceTime = carStat.TotalRaceTime.ToStringTime();
                }
                else
                {
                    raceTime = string.Format("+{0}", (carStat.TotalRaceTime - firstPlayerTime).ToStringTime());
                }

                // updates the menu game object for each player with their name, position and time
                //todo - store this in a mapping to use in calculating the rewards!

                int finishPosition = i + 1;

                raceResults.Add(new()
                {
                    Name = carStat.PlayerName,
                    FinishPosition = finishPosition,
                    RaceTime = carStat.TotalRaceTime.ToStringTime(),
                    LapCount = carStat.LapsCount
                });

                Players[i].UpdateData(carStat.PlayerName, i + 1, raceTime);
                Debug.Log(raceResults[i].Name);
                Debug.Log(raceResults[i].RaceTime);
                Debug.Log(raceResults[i].FinishPosition);
                Debug.Log(raceResults[i].LapCount);
            }
        }


        int money = 0;

        if (WorldLoading.HasLoadingParams)
        {
            var playerStatistics = RaceEntity.PlayerStatistics;
            var playerPosition = orderedCarsStatistics.IndexOf(RaceEntity.PlayerStatistics);
            var positionsCount = orderedCarsStatistics.Count;
            money = (((float)(positionsCount - playerPosition) / (float)positionsCount) * WorldLoading.LoadingTrack.MoneyForFirstPlace).ToInt();

            money = Mathf.RoundToInt(money * 0.01f) * 100; //* 0.01f and * 100, Rounding money to hundreds

            //If the first place is taken, then mark the track as completed
            if (playerPosition == 0)
            {
                PlayerProfile.SetTrackAsComplited(WorldLoading.LoadingTrack);
            }
        }

        //Inc money
        PlayerProfile.Money += money;

        var carCaption = WorldLoading.PlayerCar ? WorldLoading.PlayerCar.CarCaption : GameController.PlayerCar.gameObject.name;

        var prevBestRaceTime = PlayerProfile.GetRaceTimeForTrack(WorldLoading.LoadingTrack);
        var prevBestLapTime = PlayerProfile.GetBestLapForTrack(WorldLoading.LoadingTrack);

        float bestRaceTimeDiff = PlayerStatistics.TotalRaceTime - prevBestRaceTime;
        float bestLapTimeDiff = PlayerStatistics.BestLapTime - prevBestLapTime;

        bool prevRaceTimeIsZero = Mathf.Approximately(prevBestRaceTime, 0);
        bool prevLapTimeIsZero = Mathf.Approximately(prevBestLapTime, 0);

        //Get prev best of the race time result and save current if better.
        PrevRaceTimeText.text = string.Format("{0}\n{1}{2}",
                                                    prevBestRaceTime.ToStringTime(),
                                                    bestRaceTimeDiff > 0 && !prevRaceTimeIsZero ? "+" : "-",
                                                    bestRaceTimeDiff.ToStringTime()
        );

        if (bestRaceTimeDiff < 0 || prevRaceTimeIsZero)
        {
            PlayerProfile.SetRaceTimeForTrack(WorldLoading.LoadingTrack, PlayerStatistics.TotalRaceTime);
            PrevRaceTimeText.color = ResultBetterColor;
        }
        else
        {
            PrevRaceTimeText.color = ResultWorseColor;
        }

        //Get prev best of the lap time result and save current if better.
        PrevBestLapText.text = string.Format("{0}\n{1}{2}",
                                                    prevBestLapTime.ToStringTime(),
                                                    bestLapTimeDiff > 0 && !prevLapTimeIsZero ? "+" : "-",
                                                    bestLapTimeDiff.ToStringTime()
        );

        //Check race time
        if (bestLapTimeDiff < 0 || prevLapTimeIsZero)
        {
            PlayerProfile.SetBestLapForTrack(WorldLoading.LoadingTrack, PlayerStatistics.BestLapTime);
            PrevBestLapText.color = ResultBetterColor;
        }
        else
        {
            PrevBestLapText.color = ResultWorseColor;
        }


        PlayerProfile.RaceTime += Mathf.RoundToInt(PlayerStatistics.TotalRaceTime);

        //Get prev total score best result and save current if better.
        var distance = (int)PositioningSystem.PositioningAndAiPath.Length * WorldLoading.LapsCount;
        PlayerProfile.TotalDistance += distance;

        //Show on screen current statistics
        MoneyCaptionText.text = string.Format("+${0}", money);
        RaceTimeText.text = PlayerStatistics.TotalRaceTime.ToStringTime();
        BestLapText.text = PlayerStatistics.BestLapTime.ToStringTime();
    }

    void OnEndGame()
    {
        gameObject.SetActive(true);
        ExitToMainMenuButton.Select();
        EndGameStatisticHolder.SetTrigger(C.ShowTrigger);
        StartCoroutine(ShowEndGameCoroutine());
    }

    void RestartGame()
    {
        LoadingScreenUI.ReloadCurrentScene();
    }

    async void Exit()
    {
        if (!WorldLoading.IsMultiplayer)
        {
            LoadingScreenUI.LoadScene(MainMenuSceneName);
        }
        else
        {
            Dictionary<int, int> rewardTable = new();

            int playersInRace = raceResults.Count;

            // Pay out based on lobby size - Contract always keeps 5% of the pot
            switch (playersInRace)
            {
                case 1:
                    rewardTable[1] = 95;
                    break;
                case 2:
                    rewardTable[1] = 70;
                    rewardTable[2] = 25;
                    break;
                default:
                    rewardTable[1] = 60;
                    rewardTable[2] = 25;
                    rewardTable[3] = 10;
                    break;
            }

            int myFinishPosition = raceResults.Where(r => r.Name == PhotonNetwork.NickName).FirstOrDefault().FinishPosition;
            int myRewardPercentage = myFinishPosition > 3 ? 0 : rewardTable[myFinishPosition];
            BigInteger playerCount = BigInteger.Parse(PhotonNetwork.CurrentRoom.MaxPlayers.ToString());
            BigInteger entryAmount = ThirdwebManager.Instance.ConvertEtherToWei(PhotonNetwork.CurrentRoom.CustomProperties[C.EntryFee].ToString());

            BigInteger pot = BigInteger.Multiply(playerCount, entryAmount);

            BigInteger share = BigInteger.Multiply(pot, myRewardPercentage) / 100; // this converts reward table to a % => 75 == 75%

            TransactionResult submitResultsTxn = await SubmitRaceResultsTransaction(PhotonNetwork.CurrentRoom.Name, share);

            if (submitResultsTxn.isSuccessful())
            {
                LoadingScreenUI.LoadScene(MainMenuSceneName);
            }
        }
    }

    private async Task<TransactionResult> SubmitRaceResultsTransaction(
    string raceId,
    BigInteger amount
    )
    {
        _awaitingTxn = true;

        Contract contract = ThirdwebManager.Instance.pkRaceContract;
        Debug.Log("TRANSACTION INITIATED");


        TransactionResult txnResult = await contract.Write(
                "collectRaceRewards",
                raceId,
                $"{amount}"
        );

        _awaitingTxn = false;

        return txnResult;
    }

    void OnDestroy()
    {
        if (GameController.Instance != null)
        {
            GameController.OnEndGameAction -= OnEndGame;
        }
    }
}
