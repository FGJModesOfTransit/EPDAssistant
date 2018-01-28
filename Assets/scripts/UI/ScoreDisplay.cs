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

    [SerializeField]
    private Image lostBackground;
    [SerializeField]
    private Image lostMeter;
    private float m_LostBackgroundStep;

    private void Start()
    {
        Vector2 oldSize = lostMeter.rectTransform.sizeDelta;
        lostMeter.rectTransform.sizeDelta = new Vector2(0f, oldSize.y);
        oldSize = lostBackground.rectTransform.sizeDelta;

        lostBackground.SetNativeSize();
        Vector4 border = lostBackground.sprite.border;
        m_LostBackgroundStep = lostBackground.rectTransform.sizeDelta.x - border.x - border.z;
    }
    void Update()
	{
		if (DiseaseManager.Instance != null) 
		{
            int current = DiseaseManager.Instance.CountCurrentInflicted();

            currentInfectedText.text = "Current Infected:\n" + current;
			totalInfectedText.text = "Total Infected:\n" + DiseaseManager.Instance.CountTotalInflicted();

            if (current > 0)
            {
                diseaseNameText.text = DiseaseManager.Instance.GetDiseaseName();
            } else
            {
                diseaseNameText.text = "";
            }

            int maxLost = Mathf.CeilToInt(GraphManager.Instance.GetActiveNodes().Length / 2f);

            Vector4 border = lostBackground.sprite.border;
            Vector2 oldSize = lostBackground.rectTransform.sizeDelta;
            Vector2 newSize = new Vector2(border.x + border.z + maxLost * m_LostBackgroundStep, oldSize.y);
            LeanTween.size(lostBackground.rectTransform, newSize, 0.04f * Mathf.Abs(oldSize.x - newSize.x)); //.setEase(LeanTweenType.easeInOutSine);
            //lostBackground.rectTransform.sizeDelta = new Vector2( 60f +  active * 30f, lostBackground.rectTransform.sizeDelta.y);

            oldSize = lostMeter.rectTransform.sizeDelta;
            newSize = new Vector2(maxLost * m_LostBackgroundStep, oldSize.y);
            LeanTween.size(lostMeter.rectTransform, newSize, 0.02f * Mathf.Abs(oldSize.x - newSize.x)); //.setEase(LeanTweenType.easeInOutSine);
            //lostMeter.rectTransform.sizeDelta = new Vector2(active * 30f, oldSize.y);
            lostMeter.fillAmount = (maxLost - DiseaseManager.Instance.LostCount) / (float)maxLost;
        }
    }
}
