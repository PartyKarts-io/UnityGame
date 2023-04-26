using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// The best user results. TODO Add player statistics and achievements.
/// </summary>
public class BestResultsUI :WindowWithShowHideAnimators
{

	[SerializeField] TextMeshProUGUI BestTotalScoreText;
	[SerializeField] TextMeshProUGUI BestScoreText;
	[SerializeField] TextMeshProUGUI BestRaceTimeText;
	[SerializeField] TextMeshProUGUI TotalDistanceText;

	public override void Open ()
	{
		base.Open ();

		//All results load with car name.
		BestTotalScoreText.text = PlayerProfile.TotalScore.ToString();
		BestScoreText.text = string.Format ("{0}: {1}", PlayerProfile.BestScoreCar, PlayerProfile.BestScore);
		BestRaceTimeText.text = PlayerProfile.RaceTime.ToStringTime ();
		TotalDistanceText.text = PlayerProfile.TotalDistance.ToString();
	}

}
