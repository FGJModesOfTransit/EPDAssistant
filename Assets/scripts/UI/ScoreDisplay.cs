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

    [SerializeField]
    private Text diseaseNameText;

    void Update()
	{
		if (DiseaseManager.Instance != null) 
		{
            int current = DiseaseManager.Instance.CountCurrentInflicted();

            currentInfectedText.text = "Current Infected:\n" + current;
			totalInfectedText.text = "Total Infected:\n" + DiseaseManager.Instance.CountTotalInflicted();

            if (current > 0)
            {
                diseaseNameText.text = "Current outbreak:" + DiseaseManager.Instance.GetDiseaseName();
            } else
            {
                diseaseNameText.text = "";
            }
		}
	}
}
