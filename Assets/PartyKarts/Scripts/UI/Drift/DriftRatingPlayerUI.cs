using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DriftRatingPlayerUI :MonoBehaviour
{
	[SerializeField] TextMeshProUGUI PlayerNameText;
	[SerializeField] TextMeshProUGUI ScoreText;

	public RectTransform Rect { get; private set; }

	void Awake ()
	{
		Rect = transform as RectTransform;
	}

	public void UpdateData (string playerName, int score)
	{
		PlayerNameText.text = playerName;
		ScoreText.text = score.ToString();
	}
}
