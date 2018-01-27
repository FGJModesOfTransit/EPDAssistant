using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreDisplay : MonoBehaviour 
{
	[SerializeField]
	private Text currentInfectedText;

	[SerializeField]
	private Text totalInfectedText;

	void Update()
	{
		if (DiseaseManager.Instance != null) 
		{
			currentInfectedText.text = "Current Infected:\n" + DiseaseManager.Instance.CountCurrentInflicted();
			totalInfectedText.text = "Total Infected:\n" + DiseaseManager.Instance.CountTotalInflicted();
		}
	}
}
